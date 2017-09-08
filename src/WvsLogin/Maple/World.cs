using Destiny.Collections;
using System.Collections.ObjectModel;

namespace Destiny.Maple
{
    public sealed class World : KeyedCollection<byte, Channel>
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public ushort Port { get; private set; }
        public ushort ShopPort { get; private set; }
        public byte Channels { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; private set; }
        public bool AllowMultiLeveling { get; private set; }
        public int DefaultCreationSlots { get; private set; }
        public bool DisableCharacterCreation { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; set; }
        public int PartyQuestExperienceRate { get; set; }
        public int MesoRate { get; set; }
        public int DropRate { get; set; }

        // NOTE: Unless there's a max amount of users set, this is useless.
        public WorldStatus Status
        {
            get
            {
                return WorldStatus.Normal;
            }
        }

        public int Population
        {
            get
            {
                int population = 0;

                foreach (Channel loopChannel in this)
                {
                    population += loopChannel.Population;
                }

                return population;
            }
        }


        public Channel RandomChannel
        {
            get
            {
                return this[Application.Random.Next(this.Count)];
            }
        }

        public World(byte id)
        {
            this.ID = id;

            // TODO: Get from settings based on ID.

            this.Name = "Scania";
            this.Port = 8585;
            this.ShopPort = 9000;
            this.Channels = 2;
            this.Flag = WorldFlag.New;
            this.EventMessage = "Welcome to Scania!";
            this.TickerMessage = "Welcome to Scania (Game)!";
            this.AllowMultiLeveling = true;
            this.DefaultCreationSlots = 3;
            this.DisableCharacterCreation = false;
            this.ExperienceRate = 1;
            this.QuestExperienceRate = 1;
            this.PartyQuestExperienceRate = 1;
            this.MesoRate = 1;
            this.DropRate = 1;
        }

        protected override void InsertItem(int index, Channel item)
        {
            base.InsertItem(index, item);

            Log.Success("Registered Channel {0}-{1}.", this.Name, item.ID);
        }

        protected override void RemoveItem(int index)
        {
            Channel item = base.Items[index];

            base.RemoveItem(index);

            foreach (Channel loopChannel in this)
            {
                if (loopChannel.ID > index)
                {
                    loopChannel.ID--;
                }
            }

            Log.Warn("Unregistered Channel {0}-{1}.", this.Name, item.ID);
        }

        protected override byte GetKeyForItem(Channel item)
        {
            return item.ID;
        }
    }
}
