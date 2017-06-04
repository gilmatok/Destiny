using Destiny.Core.IO;
using Destiny.Core.Security;
using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public abstract class Session
    {
        private const int HeaderSize = sizeof(int);

        private readonly Socket mSocket;

        private readonly MapleCryptograph mSendCipher;
        private readonly MapleCryptograph mRecvCipher;

        private int mOffset;

        private byte[] mBuffer;
        private byte[] mPacket;

        private object mLocker;

        public string Host { get; }
        public bool IsAlive { get; private set; }

        protected abstract void Dispatch(byte[] buffer);
        protected abstract void Terminate();

        public Session(Socket suckit)
        {
            this.Host = (suckit.RemoteEndPoint as IPEndPoint).Address.ToString();
            this.IsAlive = true;

            suckit.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true); // bye nagle
            suckit.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true); // ty rajan

            mSocket = suckit;

            mBuffer = BufferPool.Get();
            mPacket = BufferPool.Get();

            mSendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            mRecvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            mLocker = new object();

            BeginRead();
        }

        private void BeginRead()
        {
            if (!this.IsAlive)
            {
                return;
            }

            SocketError outError = SocketError.Success;

            mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, out outError, ReadCallback, null);

            if (outError != SocketError.Success)
            {
                Close();
            }
        }

        private void ReadCallback(IAsyncResult iar)
        {
            if (!this.IsAlive)
            {
                return;
            }

            SocketError error;
            int received = mSocket.EndReceive(iar, out error);

            if (received == 0 || error != SocketError.Success)
            {
                Close();
                return;
            }

            Append(received);
            ManipulateBuffer();
            BeginRead();
        }

        private void Append(int length)
        {
            if (mPacket.Length - mOffset < length)
            {
                int newSize = mPacket.Length * 2;

                while (newSize < mOffset + length)
                    newSize *= 2;

                Array.Resize<byte>(ref mPacket, newSize);
            }

            Buffer.BlockCopy(mBuffer, 0, mPacket, mOffset, length);

            mOffset += length;
        }

        private void ManipulateBuffer()
        {
            while (mOffset > HeaderSize) //header room
            {
                int packetSize = MapleCryptograph.GetPacketLength(mPacket);

                if (mOffset < packetSize + HeaderSize) //header + packet room
                    break;

                byte[] packetBuffer = new byte[packetSize];
                Buffer.BlockCopy(mPacket, HeaderSize, packetBuffer, 0, packetSize); //copy packet

                mRecvCipher.Transform(packetBuffer); //decrypt

                mOffset -= packetSize + HeaderSize; //fix len

                if (mOffset > 0) //move reamining bytes
                    Buffer.BlockCopy(mPacket, packetSize + HeaderSize, mPacket, 0,mOffset);

                Dispatch(packetBuffer); // we done fam
            }
        }

        public void Handshake()
        {
            using (OutPacket oPacket = new OutPacket(14, 16))
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteMapleString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                this.SendRaw(oPacket.ToArray());
            }
                            }

        public void Send(OutPacket oPacket)
        {
            this.Send(oPacket.ToArray());
        }

        public void Send(byte[] buffer)
        {
            if (!this.IsAlive)
            {
                return;
            }

            lock (mLocker)
            {
                byte[] final = new byte[buffer.Length + 4];

                mSendCipher.GetHeaderToClient(final, 0, buffer.Length);
                mSendCipher.Transform(buffer);

                Buffer.BlockCopy(buffer, 0, final, 4, buffer.Length);

                this.SendRaw(final);
            }
        }

        public void SendRaw(byte[] buffer)
        {
            if (!this.IsAlive)
            {
                return;
            }

            int offset = 0;

            while (offset < buffer.Length)
            {
                SocketError outError = SocketError.Success;
                int sent = mSocket.Send(buffer, offset, buffer.Length - offset, SocketFlags.None, out outError);

                if (sent == 0 || outError != SocketError.Success)
                {
                    Close();
                    return;
                }

                offset += sent;
            }
        }

        public void Close()
        {
            if (IsAlive)
            {
                IsAlive = false;

                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();

                BufferPool.Put(mBuffer); BufferPool.Put(mPacket);

                //fkurjan

                Terminate();
            }
        }
    }
}
