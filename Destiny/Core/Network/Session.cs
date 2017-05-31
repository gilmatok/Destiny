using Destiny.Core.IO;
using Destiny.Core.Security;
using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public abstract class Session
    {
        public string Host { get; private set; }
        public bool IsAlive { get; private set; }

        private readonly Socket mSocket;

        private MapleCryptograph mSendCipher;
        private MapleCryptograph mRecvCipher;

        private bool mHeader;
        private int mOffset;
        private byte[] mBuffer;

        private object mLocker;

        protected abstract void Dispatch(byte[] buffer);
        protected abstract void Terminate();

        public Session(Socket socket)
        {
            mSocket = socket;
            mSocket.NoDelay = true;
            mSocket.SendBufferSize = 0xFFFF;
            mSocket.ReceiveBufferSize = 0xFFFF;

            mLocker = new object();

            this.Host = (mSocket.RemoteEndPoint as IPEndPoint).Address.ToString();
            this.IsAlive = true;

            mSendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            mRecvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            this.WaitForData(true, 4);
        }

        private void WaitForData(bool header, int length)
        {
            if (!this.IsAlive)
            {
                return;
            }

            mHeader = header;
            mOffset = 0;
            mBuffer = new byte[length];

            this.BeginRead(mBuffer.Length);
        }

        private void BeginRead(int length)
        {
            SocketError errorCode;

            mSocket.BeginReceive(mBuffer, mOffset, length, SocketFlags.None, out errorCode, this.ReadCallback, null);

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
            int length = mSocket.EndReceive(asyncResult, out errorCode);

            if (errorCode != SocketError.Success || length == 0)
            {
                this.Close();

                return;
            }

            mOffset += length;

            if (mOffset == mBuffer.Length)
            {
                this.HandleStream();
            }
            else
            {
                this.BeginRead(mBuffer.Length - mOffset);
            }
        }

        private void HandleStream()
        {
            if (mHeader)
            {
                int length = MapleCryptograph.GetPacketLength(mBuffer);

                if (length > mSocket.ReceiveBufferSize || !mRecvCipher.CheckServerPacket(mBuffer, 0))
                {
                    this.Close();

                    return;
                }

                this.WaitForData(false, length);
            }
            else
            {
                mRecvCipher.Transform(mBuffer);

                this.Dispatch(mBuffer);

                this.WaitForData(true, 4);
            }
        }

        public void Send(OutPacket oPacket)
        {
            this.Send(oPacket.ToArray());
        }

        public void Send(params byte[][] buffers)
        {
            if (!this.IsAlive)
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
            if (!this.IsAlive)
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
            if (!this.IsAlive)
            {
                return;
            }

            this.IsAlive = false;

            mSocket.Shutdown(SocketShutdown.Both);
            mSocket.Close();

            if (mSendCipher != null)
            {
                mSendCipher.Dispose();
            }

            if (mRecvCipher != null)
            {
                mRecvCipher.Dispose();
            }

            mOffset = 0;
            mBuffer = null;
            mSendCipher = null;
            mRecvCipher = null;

            this.Terminate();
        }
    }
}
