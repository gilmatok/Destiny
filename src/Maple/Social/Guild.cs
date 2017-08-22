using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using System;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Social
{
    public sealed class Guild : KeyedCollection<int, GuildMember>
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Notice { get; set; }
        public string Rank1 { get; set; }
        public string Rank2 { get; set; }
        public string Rank3 { get; set; }
        public string Rank4 { get; set; }
        public string Rank5 { get; set; }
        public short Logo { get; set; }
        public byte LogoColor { get; set; }
        public short Background { get; set; }
        public byte BackgroundColor { get; set; }

        public bool IsFull
        {
            get
            {
                return this.Count == 10; // TODO: Use capacity value.
            }
        }

        public Guild(Datum datum)
            : base()
        {
            this.ID = (int)datum["ID"];
            this.Name = (string)datum["Name"];
            this.Notice = (string)datum["Notice"];
            this.Rank1 = (string)datum["Rank1"];
            this.Rank2 = (string)datum["Rank2"];
            this.Rank3 = (string)datum["Rank3"];
            this.Rank4 = (string)datum["Rank4"];
            this.Rank5 = (string)datum["Rank5"];
            this.Logo = (short)datum["Logo"];
            this.LogoColor = (byte)datum["LogoColor"];
            this.Background = (short)datum["Background"];
            this.BackgroundColor = (byte)datum["BackgroundColor"];
        }

        public void Broadcast(OutPacket oPacket, GuildMember ignored = null)
        {
            foreach (GuildMember member in this)
            {
                if (member.Character != null)
                {
                    if (member != ignored)
                    {
                        member.Character.Client.Send(oPacket);
                    }
                }
            }
        }

        public void Show(Character member)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
            {
                oPacket
                    .WriteByte((byte)GuildResult.Info)
                    .WriteBool(true)
                    .WriteBytes(this.ToByteArray());

                member.Client.Send(oPacket);
            }
        }

        public byte[] ToByteArray()
        {
            using (OutPacket oPacket = new OutPacket())
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteMapleString(this.Name)
                    .WriteMapleString(this.Rank1)
                    .WriteMapleString(this.Rank2)
                    .WriteMapleString(this.Rank3)
                    .WriteMapleString(this.Rank4)
                    .WriteMapleString(this.Rank5)
                    .WriteByte((byte)this.Count);

                foreach (GuildMember member in this)
                {
                    oPacket.WriteInt(member.ID);
                }

                foreach (GuildMember member in this)
                {
                    oPacket.WriteBytes(member.ToByteArray());
                }

                oPacket
                    .WriteInt(10) // TODO: Capacity.
                    .WriteShort(this.Background)
                    .WriteByte(this.BackgroundColor)
                    .WriteShort(this.Logo)
                    .WriteByte(this.LogoColor)
                    .WriteMapleString(this.Notice)
                    .WriteInt() // NOTE: Points
                    .WriteInt(); // NOTE: Alliance ID.

                return oPacket.ToArray();
            }
        }

        protected override int GetKeyForItem(GuildMember item)
        {
            return item.ID;
        }

        protected override void InsertItem(int index, GuildMember item)
        {
            item.Guild = this;

            base.InsertItem(index, item);

            if (item.Character != null)
            {
                item.Character.Guild = this;

                this.Show(item.Character);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                {
                    oPacket
                        .WriteByte((byte)GuildResult.AddMember)
                        .WriteInt(this.ID)
                        .WriteInt(item.ID)
                        .WriteBytes(item.ToByteArray());

                    this.Broadcast(oPacket, item);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            GuildMember item = base.Items[index];

            item.Guild = null;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
            {
                oPacket
                    .WriteByte((byte)(item.Expelled ? GuildResult.MemberExpel : GuildResult.LeaveMember))
                    .WriteInt(this.ID)
                    .WriteInt(item.ID)
                    .WriteMapleString(item.Name);

                this.Broadcast(oPacket);
            }

            if (item.Character != null)
            {
                item.Character.Guild = null;
            }
            else if (item.Expelled)
            {
                Datum datum = new Datum("memos");

                datum["CharacterID"] = item.ID;
                datum["Sender"] = "Guild Master"; // TODO: Get the kicker name.
                datum["Message"] = "You have been expelled from the guild.";
                datum["Received"] = DateTime.UtcNow;

                datum.Insert();
            }

            base.RemoveItem(index);
        }
    }
}
