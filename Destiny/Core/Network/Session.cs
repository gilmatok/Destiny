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

        private readonly Socket m_socket;

        private readonly MapleCryptograph m_sendCipher;
        private readonly MapleCryptograph m_recvCipher;

        private int m_offset;

        private byte[] m_buffer;
        private byte[] m_packet;

        private object m_locker;
       
        public string Host { get; private set; }

        public bool IsAlive { get; private set; }

        protected abstract void Dispatch(byte[] buffer);
        protected abstract void Terminate();

        public Session(Socket socket)
        {
            m_socket = socket;

            m_socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            m_sendCipher = new MapleCryptograph(Constants.Version, Constants.SIV, TransformDirection.Encrypt);
            m_recvCipher = new MapleCryptograph(Constants.Version, Constants.RIV, TransformDirection.Decrypt);

            m_buffer = BufferPool.Get();
            m_packet = BufferPool.Get();

            m_locker = new object();

            this.Host = (m_socket.RemoteEndPoint as IPEndPoint).Address.ToString();

            this.IsAlive = true;
        }

        private void BeginRead()
        {
            if (!this.IsAlive)
            {
                return;
            }

            SocketError errorCode = SocketError.Success;

            m_socket.BeginReceive(m_buffer, 0, m_buffer.Length, SocketFlags.None, out errorCode, this.ReadCallback, null);

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
            int received = m_socket.EndReceive(asyncResult, out errorCode);

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
            if (m_packet.Length - m_offset < length)
            {
                int newSize = m_packet.Length * 2;

                while (newSize < m_offset + length)
                {
                    newSize *= 2;
                }

                Array.Resize(ref m_packet, newSize);
            }

            Buffer.BlockCopy(m_buffer, 0, m_packet, m_offset, length);

            m_offset += length;
        }

        private void ManipulateBuffer()
        {
            while (m_offset > Session.HeaderSize)
            {
                int packetSize = MapleCryptograph.GetPacketLength(m_packet);

                if (m_offset < packetSize + Session.HeaderSize)
                {
                    break;
                }

                byte[] buffer = new byte[packetSize];

                Buffer.BlockCopy(m_packet, Session.HeaderSize, buffer, 0, packetSize);

                m_recvCipher.Transform(buffer);

                m_offset -= packetSize + Session.HeaderSize;

                if (m_offset > 0)
                {
                    Buffer.BlockCopy(m_packet, packetSize + Session.HeaderSize, m_packet, 0, m_offset);
                }

                this.Dispatch(buffer);
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

            lock (m_locker)
            {
                byte[] final = new byte[buffer.Length + 4];

                m_sendCipher.GetHeaderToClient(final, 0, buffer.Length);
                m_sendCipher.Transform(buffer);

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
                int sent = m_socket.Send(buffer, offset, buffer.Length - offset, SocketFlags.None, out errorCode);

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

            m_socket.Shutdown(SocketShutdown.Both);
            m_socket.Close();

            BufferPool.Put(m_buffer);
            BufferPool.Put(m_packet);

            this.Terminate();
        }
    }
}
