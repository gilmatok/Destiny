using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using System.Collections.Generic;

namespace Destiny.Maple.Social
{
    public sealed class Guild
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Notice { get; private set; }
        public string Rank1 { get; private set; }
        public string Rank2 { get; private set; }
        public string Rank3 { get; private set; }
        public string Rank4 { get; private set; }
        public string Rank5 { get; private set; }
        public short Logo { get; private set; }
        public byte LogoColor { get; private set; }
        public short Background { get; private set; }
        public byte BackgroundColor { get; private set; }

        private Dictionary<int, GuildMember> Members { get; set; }
        private Dictionary<int, Character> Characters { get; set; }

        public Guild(Datum datum)
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

            this.Members = new Dictionary<int, GuildMember>();
            this.Characters = new Dictionary<int, Character>();
        }

        public void AddMember(Character character)
        {
            character.Guild = this;

            this.Members[character.ID] = new GuildMember(this, character);
            this.Characters[character.ID] = character;
        }

        public void AddMember(GuildMember member)
        {
            this.Members[member.ID] = member;
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

        public void UpdateNotice(string text)
        {
            this.Notice = text;

            Datum datum = new Datum("guilds");

            datum["Notice"] = text;

            datum.Update("ID = {0}", this.ID);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
            {
                oPacket
                    .WriteByte((byte)GuildResult.UpdateNotice)
                    .WriteInt(this.ID)
                    .WriteMapleString(this.Notice);

                this.Broadcast(oPacket);
            }
        }

        public void Broadcast(OutPacket oPacket, Character ignored = null)
        {
            foreach (Character character in this.Characters.Values)
            {
                if (character != ignored)
                {
                    character.Client.Send(oPacket);
                }
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
                    .WriteByte((byte)this.Members.Count);

                foreach (int memberID in this.Members.Keys)
                {
                    oPacket.WriteInt(memberID);
                }

                foreach (GuildMember member in this.Members.Values)
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
    }
}
