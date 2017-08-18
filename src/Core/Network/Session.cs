using Destiny.Core.IO;
using Destiny.Core.Security;
using Destiny.Packets;
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

        public string Host { get; private set; }

        public bool IsAlive { get; private set; }

        protected abstract void Dispatch(InPacket iPacket);
        protected abstract void Terminate();

        public Session(Socket socket)
        {
            mSocket = socket;

            mSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            mSendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            mRecvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            mBuffer = BufferPool.Get();
            mPacket = BufferPool.Get();

            mLocker = new object();

            this.Host = (mSocket.RemoteEndPoint as IPEndPoint).Address.ToString();

            this.IsAlive = true;
        }

        private void BeginRead()
        {
            if (!this.IsAlive)
            {
                return;
            }

            SocketError errorCode = SocketError.Success;

            mSocket.BeginReceive(mBuffer, 0, mBuffer.Length, SocketFlags.None, out errorCode, this.ReadCallback, null);

            if (errorCode != SocketError.Success)
            {
                this.Close();
            }
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            if (!this.IsAlive)
            {
                return;
            }

            SocketError errorCode;
            int received = mSocket.EndReceive(asyncResult, out errorCode);

            if (errorCode != SocketError.Success || received == 0)
            {
                this.Close();

                return;
            }

            this.Append(received);
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

                Array.Resize(ref mPacket, newSize);
            }

            Buffer.BlockCopy(mBuffer, 0, mPacket, mOffset, length);

            mOffset += length;
        }

        private void ManipulateBuffer()
        {
            while (mOffset > Session.HeaderSize)
            {
                int packetSize = MapleCryptograph.GetPacketLength(mPacket);

                if (mOffset < packetSize + Session.HeaderSize)
                {
                    break;
                }

                byte[] buffer = new byte[packetSize];

                Buffer.BlockCopy(mPacket, Session.HeaderSize, buffer, 0, packetSize);

                mRecvCipher.Transform(buffer);

                mOffset -= packetSize + Session.HeaderSize;

                if (mOffset > 0)
                {
                    Buffer.BlockCopy(mPacket, packetSize + Session.HeaderSize, mPacket, 0, mOffset);
                }

                InPacket iPacket = new InPacket(buffer);

                if (Enum.IsDefined(typeof(ClientOperationCode), iPacket.OperationCode))
                {
                    switch (InPacket.LogLevel)
                    {
                        case LogLevel.Name:
                            Log.Inform("Received {0} packet from {1}.", Enum.GetName(typeof(ClientOperationCode), iPacket.OperationCode), this.Host);
                            break;

                        case LogLevel.Full:
                            Log.Hex("Received {0} packet from {1}: ", iPacket.ToArray(), Enum.GetName(typeof(ClientOperationCode), iPacket.OperationCode), this.Host);
                            break;
                    }
                }
                else
                {
                    Log.Hex("Received unknown (0x{0:X2}) packet from {1}: ", iPacket.ToArray(), (short)iPacket.OperationCode, this.Host);
                }

                try
                {
                    this.Dispatch(iPacket);
                }
                catch (Exception ex)
                {
                    Log.Error(ex); // TODO: Add a detailed message.
                }
            }
        }

        public void Handshake()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CreateNewCharacterResult)) // NOTE: I don't like empty constructors. The first short is size, so...
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                this.SendRaw(oPacket.ToArray());
            }

            this.BeginRead();
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
                SocketError errorCode = SocketError.Success;
                int sent = mSocket.Send(buffer, offset, buffer.Length - offset, SocketFlags.None, out errorCode);

                if (errorCode != SocketError.Success || sent == 0)
                {
                    this.Close();

                    return;
                }

                offset += sent;
            }
        }

        public void Close()
        {
            if (!this.IsAlive)
            {
                return;
            }

            this.IsAlive = false;

            mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();

            BufferPool.Put(mBuffer);
            BufferPool.Put(mPacket);

            this.Terminate();
        }
    }
}
