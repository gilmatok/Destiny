using Destiny.IO;
using Destiny.Security;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Destiny.Network
{
    public abstract class ClientHandler<TReceiveOP, TSendOP, TCryptograph>
        : NetworkConnector<TReceiveOP, TSendOP, TCryptograph>, IDisposable where TCryptograph : Cryptograph, new()
    {
        protected string Title { get; set; }

        protected virtual void Register() { }
        protected virtual void Unregister() { }

        public ClientHandler(Socket socket, string title = "Client", params object[] args)
        {
            this.Title = title;

            Log.Inform("Preparing {0}...", this.Title.ToLower());

            this.Socket = socket;
            this.Cryptograph = new TCryptograph();
            this.ReceivalBuffer = new ByteBuffer() { Limit = 0 };
            this.IsAlive = true;

            this.Prepare(args);

            Log.Success(string.Format("{0} connected from {1}.", this.Title, this.RemoteEndPoint.Address));

            this.Initialize();

            this.Register();

            while (this.IsAlive && this.IsServerAlive)
            {
                this.ReceiveDone.Reset();

                try
                {
                    this.Socket.BeginReceive(this.ReceivalBuffer.Array, this.ReceivalBuffer.Limit, this.ReceivalBuffer.Capacity - this.ReceivalBuffer.Limit, SocketFlags.None, new AsyncCallback(this.OnReceive), null);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    this.Stop();
                }

                this.ReceiveDone.WaitOne();
            }

            this.Dispose();
        }

        private ByteBuffer ReceivalBuffer { get; set; }

        private void Handle(byte[] rawPacket)
        {
            using (Packet inPacket = new Packet(this.Cryptograph.Decrypt(rawPacket)))
            {
                if (Enum.IsDefined(typeof(TReceiveOP), inPacket.OperationCode))
                {
                    switch (Packet.LogLevel)
                    {
                        case LogLevel.Name:
                            Log.Inform("Received {0} packet from {1}.", Enum.GetName(typeof(TReceiveOP), inPacket.OperationCode), this.Title);
                            break;

                        case LogLevel.Full:
                            Log.Hex("Received {0} packet from {1}: ", inPacket.Array, Enum.GetName(typeof(TReceiveOP), inPacket.OperationCode), this.Title);
                            break;
                    }
                }
                else
                {
                    Log.Hex("Received unknown (0x{0:X2}) packet from {1}: ", inPacket.Array, inPacket.OperationCode, this.Title);
                }

                this.Dispatch(inPacket);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            if (this.IsAlive)
            {
                try
                {
                    int received = this.Socket.EndReceive(ar);

                    if (received == 0)
                    {
                        this.Stop();
                    }
                    else
                    {
                        this.ReceivalBuffer.Limit += received;

                        if (this.Cryptograph is MapleCryptograph)
                        {
                            int processed = 0;
                            bool reset = false;

                            if (this.ReceivalBuffer.Remaining < 4)
                            {
                                Log.Error("TODO: Remaining < 4!");
                            }

                            while (this.ReceivalBuffer.Remaining >= 4)
                            {
                                int length = AesCryptograph.RetrieveLength(this.ReceivalBuffer.ReadBytes(4));

                                if (this.ReceivalBuffer.Remaining < length)
                                {
                                    this.ReceivalBuffer.Position -= 4;

                                    Buffer.BlockCopy(this.ReceivalBuffer.Array, this.ReceivalBuffer.Position, this.ReceivalBuffer.Array, 0, this.ReceivalBuffer.Remaining);

                                    reset = true;

                                    break;
                                }
                                else
                                {
                                    this.ReceivalBuffer.Position -= 4;

                                    this.Handle(this.ReceivalBuffer.ReadBytes(length + 4));

                                    processed += (length + 4);
                                }
                            }

                            this.ReceivalBuffer.Limit -= processed;

                            if (reset)
                            {
                                this.ReceivalBuffer.Position = 0;
                            }
                            else
                            {
                                this.ReceivalBuffer.Position = this.ReceivalBuffer.Limit;
                            }
                        }
                        else
                        {
                            this.Handle(this.ReceivalBuffer.GetContent());
                            this.ReceivalBuffer.Limit = 0;
                            this.ReceivalBuffer.Position = 0;
                        }
                    }

                    this.ReceiveDone.Set();
                }
                catch (Exception e)
                {
                    Log.Error("Uncatched fatal error on {0}: ", e, this.Title.ToLower(), Thread.CurrentThread.ManagedThreadId);
                    this.Stop();
                }
            }
        }

        public void Send(Packet Packet)
        {
            lock (this)
            {
                if (this.IsAlive)
                {
                    Packet.SafeFlip();
                    this.Socket.Send(this.Cryptograph.Encrypt(Packet.GetContent()));

                    if (Enum.IsDefined(typeof(TSendOP), Packet.OperationCode))
                    {
                        switch (Packet.LogLevel)
                        {
                            case LogLevel.Name:
                                Log.Inform("Sent {0} packet to {1}.", Enum.GetName(typeof(TSendOP), Packet.OperationCode), this.Title);
                                break;

                            case LogLevel.Full:
                                Log.Hex("Sent {0} packet to {1}: ", Packet.GetContent(), Enum.GetName(typeof(TSendOP), Packet.OperationCode), this.Title);
                                break;
                        }
                    }
                    else
                    {
                        Log.Hex("Sent unknown (0x{0:X2}) packet to {1}: ", Packet.Array, Packet.OperationCode, this.Title);
                    }
                }
                else
                {
                    //Log.Warn("Tried to send {0} packet to dead client.", Enum.GetName(typeof(TSendOP), Packet.OperationCode));
                }
            }
        }

        public void Dispose()
        {
            try
            {
                this.Terminate();
            }
            catch (Exception e)
            {
                Log.Error("Termination error on {0}: ", e, this.Title);
            }

            this.Socket.Dispose();
            this.Cryptograph.Dispose();
            //this.ReceiveDone.Dispose(); // TODO: Figure why this crashes.
            this.ReceivalBuffer.Dispose();

            this.CustomDispose();

            this.Unregister();

            Log.Inform("{0} disposed.", this.Title);
        }
    }
}
