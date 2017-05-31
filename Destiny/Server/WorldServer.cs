using Destiny.Utility;

namespace Destiny.Server
{
    public sealed class WorldServer
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public ChannelServer[] Channels { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }
        public MigrationRegistery Migrations { get; private set; }

        public WorldStatus Status
        {
            get
            {
                return WorldStatus.Normal; // NOTE: Unless someone wants to impose a maximum registered users, this is useless.
            }
        }

        public WorldServer(CWorld config)
        {
            this.ID = config.ID;
            this.Name = config.Name;
            this.Channels = new ChannelServer[config.Channels];

            short port = config.Port;

            for (byte id = 0; id < config.Channels; id++)
            {
                this.Channels[id] = new ChannelServer(id, this.ID, port);

                port++;
            }

            this.Flag = config.Flag;
            this.EventMessage = config.EventMessage;
            this.TickerMessage = config.TickerMessage;
            this.Migrations = new MigrationRegistery();
        }

        public void Start()
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Start();
            }

            Logger.Write(LogLevel.Info, "WorldServer {0} started with {1} channels.", this.Name, this.Channels.Length);
        }

        public void Stop()
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Stop();
            }

            Logger.Write(LogLevel.Info, "WorldServer {0} stopped.", this.Name);
        }

        public void Notify(string message, NoticeType type)
        {
            foreach (ChannelServer channel in this.Channels)
            {
                channel.Notify(message, type);
            }
        }
    }
}
