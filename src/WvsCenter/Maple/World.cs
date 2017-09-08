using Destiny.Network;
using System.Collections.ObjectModel;
using System.Linq;

namespace Destiny.Maple
{
    public sealed class World : KeyedCollection<byte, CenterClient>
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public ushort Port { get; private set; }
        public ushort ShopPort { get; private set; }
        public byte Channels { get; private set; }
        public string TickerMessage { get; private set; }
        public bool AllowMultiLeveling { get; private set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }

        private CenterClient shop;

        public CenterClient Shop
        {
            get
            {
                return shop;
            }
            set
            {
                shop = value;

                if (value != null)
                {
                    this.Shop.Port = this.ShopPort;
                }
            }
        }

        public CenterClient RandomChannel
        {
            get
            {
                return this[Application.Random.Next(this.Count)];
            }
        }

        public bool IsFull
        {
            get
            {
                return this.Count == this.Channels;
            }
        }

        public bool HasShop
        {
            get
            {
                return this.Shop != null;
            }
        }

        internal World() : base() { }

        internal World(Packet inPacket)
            : this()
        {
            this.ID = inPacket.ReadByte();
            this.Name = inPacket.ReadString();
            this.Port = inPacket.ReadUShort();
            this.ShopPort = inPacket.ReadUShort();
            this.Channels = inPacket.ReadByte();
            this.TickerMessage = inPacket.ReadString();
            this.AllowMultiLeveling = inPacket.ReadBool();
            this.ExperienceRate = inPacket.ReadInt();
            this.QuestExperienceRate = inPacket.ReadInt();
            this.PartyQuestExperienceRate = inPacket.ReadInt();
            this.MesoRate = inPacket.ReadInt();
            this.DropRate = inPacket.ReadInt();
        }

        protected override void InsertItem(int index, CenterClient item)
        {
            item.ID = (byte)index;
            item.Port = (ushort)(this.Port + index);

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            foreach (CenterClient loopChannel in this)
            {
                if (loopChannel.ID > index)
                {
                    loopChannel.ID--;

                    using (Packet Packet = new Packet(InteroperabilityOperationCode.UpdateChannelID))
                    {
                        Packet.WriteByte(loopChannel.ID);

                        loopChannel.Send(Packet);
                    }
                }
            }
        }

        protected override byte GetKeyForItem(CenterClient item)
        {
            return item.ID;
        }
    }
}
