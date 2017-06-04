using Destiny.Core.IO;
using Destiny.Core.Security;
using System;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public abstract class Session : NetworkStream
    {
        private readonly MapleCryptograph mSendCipher;
        private readonly MapleCryptograph mRecvCipher;

        private byte[] mBuffer;

        private object mLocker;

        public string Host { get; }
        public bool IsAlive { get; private set; }

        protected abstract void Dispatch(byte[] buffer);
        protected abstract void Terminate();

        public Session(Socket socket)
            : base(socket)
        {
            mSendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            mRecvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            mLocker = new object();

            this.Host = "127.0.0.1"; //lolwut
            this.IsAlive = true;
        }

        private async void Receive()
        {
            while (this.IsAlive)
            {
                if (!this.DataAvailable)
                {
                    continue;
                }

                int length = 4;

                mBuffer = new byte[length];

                if (await this.ReadAsync(mBuffer, 0, length) == length)
                {
                    length = MapleCryptograph.GetPacketLength(mBuffer);
                }

                if (!mRecvCipher.CheckServerPacket(mBuffer, 0))
                {
                    this.Close();

                    return;
                }

                mBuffer = new byte[length];

                if (await this.ReadAsync(mBuffer, 0, length) == length)
                {
                    mRecvCipher.Transform(mBuffer);

                    this.Dispatch(mBuffer);
                }
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

            this.Receive();
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

        public async void SendRaw(byte[] buffer)
        {
            if (!this.IsAlive)
            {
                return;
            }

            await this.WriteAsync(buffer, 0, buffer.Length);
        }

        public new void Close()
        {
            if (!this.IsAlive)
            {
                return;
            }

            this.IsAlive = false;

            this.Terminate();

            base.Close();
        }
    }
}
