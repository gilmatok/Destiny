using Destiny.Core.IO;
using Destiny.Security;
using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public abstract class Session
    {
        public string Host { get; private set; }
        public bool IsConnected { get; private set; }

        private readonly Socket mSocket;

        private readonly MapleCryptograph mSendCipher;
        private readonly MapleCryptograph mRecvCipher;
        
        protected abstract void Dispatch(InPacket iPacket);
        protected abstract void Terminate();

        private int mOffset;
        private byte[] mBuffer;
        private byte[] mPacket;

        private object mLocker;

        public Session(Socket socket)
        {
            mSocket = socket;

            this.Host = (mSocket.RemoteEndPoint as IPEndPoint).Address.ToString();

            mSendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            mRecvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            mBuffer = BufferPool.Get();
            mPacket = BufferPool.Get();

            mLocker = new object();

            this.IsConnected = true;

            this.BeginRead();
        }

        private void BeginRead()
        {
            if (this.IsConnected)
            {
                return;
            }

            SocketError errorCode;

            mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, out errorCode, this.ReadCallback, null);

            if (errorCode != SocketError.Success)
            {
                this.Close();
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            if (!this.IsConnected)
            {
                return;
            }

            SocketError errorCode;
            int length = mSocket.EndReceive(asyncResult, out errorCode);

            if (errorCode != SocketError.Success || length == 0)
            {
                this.Close();

                return;
            }

            this.Append(length);
            this.ManipulateBuffer();
            this.BeginRead();
        }

        private void Append(int length)
        {
            if (mPacket.Length - mOffset < length)
            {
                int newSize = mPacket.Length * 2;

                while (newSize < mOffset + length)
                {
                    newSize *= 2;
                }

                Array.Resize<byte>(ref mPacket, newSize);
            }

            Buffer.BlockCopy(mBuffer, 0, mPacket, mOffset, length);

            mOffset += length;
        }

        private void ManipulateBuffer()
        {
            while (mOffset > 4)
            {
                int length = MapleCryptograph.GetPacketLength(mBuffer);

                if (mOffset < length + 4)
                {
                    break;
                }

                byte[] buffer = new byte[length];

                Buffer.BlockCopy(mPacket, 4, buffer, 0, length);

                mRecvCipher.Transform(buffer);

                mOffset -= length + 4;

                if (mOffset > 0)
                {
                    Buffer.BlockCopy(mPacket, length + 4, mPacket, 0, mOffset);
                }

                this.Dispatch(new InPacket(buffer));
            }
        }

        public void Send(OutPacket oPacket)
        {
            this.Send(oPacket.ToArray());
        }

        public void Send(params byte[][] buffers)
        {
            if (!this.IsConnected)
            {
                return;
            }

            lock (mLocker)
            {
                int length = 0;
                int offset = 0;

                foreach (byte[] buffer in buffers)
                {
                    length += 4;
                    length += buffer.Length;
                }

                byte[] final = new byte[length];

                foreach (byte[] buffer in buffers)
                {
                    mSendCipher.GetHeaderToClient(final, offset, buffer.Length);

                    offset += 4;

                    mSendCipher.Transform(buffer);

                    Buffer.BlockCopy(buffer, 0, final, offset, buffer.Length);

                    offset += buffer.Length;
                }

                this.SendRaw(final);
            }
        }

        public void SendRaw(byte[] buffer)
        {
            if (!this.IsConnected)
            {
                return;
            }

            int offset = 0;

            while (offset < buffer.Length)
            {
                SocketError errorCode;
                int length = mSocket.Send(buffer, offset, buffer.Length - offset, SocketFlags.None, out errorCode);

                if (errorCode != SocketError.Success || length == 0)
                {
                    this.Close();

                    return;
                }

                offset += length;
            }
        }

        public void Close()
        {
            if (!this.IsConnected)
            {
                return;
            }

            this.IsConnected = false;

            mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();

            mOffset = 0;

            BufferPool.Put(mBuffer);

            if (mPacket.Length == BufferPool.BufferSize)
            {
                BufferPool.Put(mPacket);
            }
            else
            {
                mPacket = null;
            }

            this.Terminate();
        }
    }
}
