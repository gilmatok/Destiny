using System.Collections.ObjectModel;

namespace Destiny.Server
{
    public sealed class WorldServer : KeyedCollection<byte, ChannelServer>
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }

        public WorldServer()
            : base()
        {
            this.ID = 0;
            this.Name = "Scania";
            this.Flag = WorldFlag.New;
            this.EventMessage = "Welcome to #rDestiny#k!";
            this.TickerMessage = "Welcome to Destiny!";
            this.ExperienceRate = 1;
            this.QuestExperienceRate = 1;
            this.PartyQuestExperienceRate = 1;
            this.MesoRate = 1;
            this.DropRate = 1;

            byte channels = 2;

            for (byte i = 0; i < channels; i++)
            {
                this.Add(new ChannelServer(i, this, (short)(8585 + i)));
            }
        }

        public void Start()
        {
            foreach (ChannelServer channel in this)
            {
                channel.Start();
            }
        }

        public void Stop()
        {
            foreach (ChannelServer channel in this)
            {
                channel.Stop();
            }
        }

        protected override byte GetKeyForItem(ChannelServer item)
        {
            return item.ID;
        }
    }
}
