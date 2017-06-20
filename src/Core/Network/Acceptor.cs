using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public sealed class Acceptor
    {
        private const int Backlog = 2500;

        private readonly short m_port;
        private Socket m_socket;
        private bool m_active;
        private bool m_disposed;
        private Action<Socket> m_clientAcceptedEvent;

        public short Port => m_port;
        public bool Active => m_active;

        public Acceptor(short port, Action<Socket> clientAcceptedEvent)
        {
            m_port = port;
            m_active = false;
            m_disposed = false;
            m_clientAcceptedEvent = clientAcceptedEvent;
        }

        public void Start()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (m_active)
            {
                throw new InvalidOperationException();
            }

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Bind(new IPEndPoint(IPAddress.Any, m_port));
            m_socket.Listen(Backlog);

            m_active = true;

            m_socket.BeginAccept(this.EndAccept, null);
        }

        public void Stop()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (m_active == false)
            {
                throw new InvalidOperationException();
            }

            m_active = false;

            m_socket.Dispose();
        }

        private void EndAccept(IAsyncResult iar)
        {
            if (!m_disposed && m_active)
            {
                Socket socket = m_socket.EndAccept(iar);

                if (m_clientAcceptedEvent != null)
                {
                    m_clientAcceptedEvent(socket);
                }

                if (!m_disposed && m_active)
                {
                    m_socket.BeginAccept(this.EndAccept, null);
                }
            }
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                if (m_active)
                {
                    this.Stop();
                }

                m_disposed = true;
            }
        }
    }
}
