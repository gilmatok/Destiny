using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public sealed class Acceptor
    {
        public short Port { get; private set; }

        private readonly TcpListener m_listener;

        private bool m_disposed;

        public Action<Socket> OnClientAccepted;

        public Acceptor(short port)
            : this(IPAddress.Any, port)
        {
        }

        public Acceptor(IPAddress ip, short port)
        {
            Port = port;
            m_listener = new TcpListener(IPAddress.Any, port);
            OnClientAccepted = null;
            m_disposed = false;
        }

        public void Start()
        {
            m_listener.Start(50);
            m_listener.BeginAcceptSocket(this.EndAccept, null);
        }

        public void Stop()
        {
            this.Dispose();
        }

        private void EndAccept(IAsyncResult iar)
        {
            if (m_disposed) { return; }

            Socket socket = m_listener.EndAcceptSocket(iar);

            if (this.OnClientAccepted != null)
            {
                this.OnClientAccepted(socket);
            }
            
            m_listener.BeginAcceptSocket(this.EndAccept, null);

        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;
                m_listener.Server.Close();
            }
        }
    }
}
