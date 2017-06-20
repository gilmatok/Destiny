using System;
using System.Net;
using System.Net.Sockets;

namespace Destiny.Core.Network
{
    public sealed class Acceptor
    {
        private const int Backlog = 2500;

        private readonly short mPort;
        private Socket mSocket;
        private bool mActive;
        private bool mDisposed;
        private Action<Socket> mClientAcceptedEvent;

        public short Port => mPort;
        public bool Active => mActive;

        public Acceptor(short port, Action<Socket> clientAcceptedEvent)
        {
            mPort = port;
            mActive = false;
            mDisposed = false;
            mClientAcceptedEvent = clientAcceptedEvent;
        }

        public void Start()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (mActive)
            {
                throw new InvalidOperationException();
            }

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mSocket.Bind(new IPEndPoint(IPAddress.Any, mPort));
            mSocket.Listen(Backlog);

            mActive = true;

            mSocket.BeginAccept(this.EndAccept, null);
        }

        public void Stop()
        {
            if (mDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (mActive == false)
            {
                throw new InvalidOperationException();
            }

            mActive = false;

            mSocket.Dispose();
        }

        private void EndAccept(IAsyncResult iar)
        {
            if (!mDisposed && mActive)
            {
                Socket socket = mSocket.EndAccept(iar);

                if (mClientAcceptedEvent != null)
                {
                    mClientAcceptedEvent(socket);
                }

                if (!mDisposed && mActive)
                {
                    mSocket.BeginAccept(this.EndAccept, null);
                }
            }
        }

        public void Dispose()
        {
            if (!mDisposed)
            {
                if (mActive)
                {
                    this.Stop();
                }

                mDisposed = true;
            }
        }
    }
}
