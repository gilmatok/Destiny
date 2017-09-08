using Destiny.IO;
using Destiny.Security;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Destiny.Network
{
    public abstract class ServerHandler<TReceiveOP, TSendOP, TCryptograph>
        : NetworkConnector<TReceiveOP, TSendOP, TCryptograph>, IDisposable where TCryptograph : Cryptograph, new()
    {
        protected string Title { get; private set; }

        protected abstract void StopServer();
        protected virtual void Initialize(params object[] args) { }

        public ServerHandler(IPEndPoint remoteEP, string title = "Server", params object[] args)
        {
            this.Title = title;

            Log.Inform("Connecting to {0} at {1}...", this.Title.ToLower(), remoteEP.Address);

            this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Cryptograph = new TCryptograph();
            this.IsAlive = true;

            this.Prepare();

            bool connected = false;
            int tries = 0;

            while (!connected && tries < 5)
            {
                try
                {
                    if (tries > 0)
                    {
                        Thread.Sleep(2000);
                    }

                    this.Socket.Connect(remoteEP);
                    connected = true;
                }
                catch
                {
                    Log.Warn("Could not connect to {0} at {1}.", this.Title.ToLower(), remoteEP.ToString());
                    tries++;
                }
            }

            if (!connected)
            {
                throw new NetworkException(string.Format("{0} connection failed.", this.Title));
            }

            Log.Success("Connected to {0} on thread {1}.", this.Title.ToLower(), Thread.CurrentThread.ManagedThreadId);

            this.Initialize(args);
        }

        public void Loop()
        {
            while (this.IsAlive)// && this.IsServerAlive)
            {
                this.ReceiveDone.Reset();

                ByteBuffer buffer = new ByteBuffer();

                this.Socket.BeginReceive(buffer.Array, buffer.Position, buffer.Capacity, SocketFlags.None, new AsyncCallback(this.OnReceive), buffer);

                this.ReceiveDone.WaitOne();
            }

            this.Dispose();

            this.StopServer();
        }

        protected void OnReceive(IAsyncResult ar)
        {
            if (this.IsAlive)
            {
                using (ByteBuffer buffer = (ByteBuffer)ar.AsyncState)
                {
                    buffer.Position = 0;

                    try
                    {
                        buffer.Limit = this.Socket.EndReceive(ar);

                        if (buffer.Limit == 0)
                        {
                            this.Stop();
                        }
                        else
                        {
                            using (Packet inPacket = new Packet(this.Cryptograph.Decrypt(buffer.GetContent())))
                            {
                                if (Enum.IsDefined(typeof(TReceiveOP), inPacket.OperationCode))
                                {
                                    switch (Packet.LogLevel)
                                    {
                                        case LogLevel.Name:
                                            Log.Inform("Received {0} packet.", Enum.GetName(typeof(TReceiveOP), inPacket.OperationCode));
                                            break;

                                        case LogLevel.Full:
                                            Log.Hex("Received {0} packet: ", inPacket.Array, Enum.GetName(typeof(TReceiveOP), inPacket.OperationCode));
                                            break;
                                    }
                                }
                                else
                                {
                                    Log.Hex("Received unknown (0x{0:X2}) packet: ", inPacket.Array, inPacket.OperationCode);
                                }

                                this.Dispatch(inPacket);
                            }
                        }

                        this.ReceiveDone.Set();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Uncatched fatal error on {0}: ", e, this.Title.ToLower());
                        this.Stop();
                    }
                }
            }
        }

        public void Send(Packet Packet)
        {
            Packet.SafeFlip();
            this.Socket.Send(this.Cryptograph.Encrypt(Packet.GetContent()));

            if (Enum.IsDefined(typeof(TSendOP), Packet.OperationCode))
            {
                switch (Packet.LogLevel)
                {
                    case LogLevel.Name:
                        Log.Inform("Sent {0} packet.", Enum.GetName(typeof(TSendOP), Packet.OperationCode));
                        break;

                    case LogLevel.Full:
                        Log.Hex("Sent {0} packet: ", Packet.GetContent(), Enum.GetName(typeof(TSendOP), Packet.OperationCode));
                        break;
                }
            }
            else
            {
                Log.Hex("Sent unknown ({0:X2}) packet: ", Packet.Array, Packet.OperationCode);
            }
        }

        public void Dispose()
        {
            this.Terminate();

            this.Socket.Dispose();

            this.Cryptograph.Dispose();
            this.ReceiveDone.Dispose(); // TODO: Figure out why this crashes.

            this.CustomDispose();

            Log.Inform("{0} disposed.", this.Title);
        }
    }
}
