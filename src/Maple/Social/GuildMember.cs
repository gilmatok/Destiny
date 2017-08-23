using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class GuildMember
    {
        public Guild Guild { get; set; }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public byte Level { get; private set; }
        public Job Job { get; private set; }
        public int Rank { get; set; }
        public bool Expelled { get; set; }
        public string Expeller { get; set; }

        private Character character;

        public Character Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                {
                    oPacket
                        .WriteByte((byte)GuildResult.MemberOnline)
                        .WriteInt(this.Guild.ID)
                        .WriteInt(this.ID)
                        .WriteBool(this.IsOnline);

                    this.Guild.Broadcast(oPacket, this);
                }
            }
        }

        public bool IsOnline
        {
            get
            {
                return this.Character != null;
            }
        }

        public GuildMember(Datum datum)
        {
            this.ID = (int)datum["ID"];
            this.Name = (string)datum["Name"];
            this.Level = (byte)datum["Level"];
            this.Job = (Job)datum["Job"];
            this.Rank = (int)datum["GuildRank"];
        }

        public GuildMember(Character character)
        {
            this.ID = character.ID;
            this.Name = character.Name;
            this.Level = character.Level;
            this.Job = character.Job;
            this.Rank = character.GuildRank;
            this.character = character;
        }

        public byte[] ToByteArray()
        {
            using (OutPacket oPacket = new OutPacket())
            {
                oPacket
                    .WritePaddedString(this.Name, 13)
                    .WriteInt((int)this.Job)
                    .WriteInt(this.Level)
                    .WriteInt(this.Rank)
                    .WriteInt(this.IsOnline ? 1 : 0)
                    .WriteInt() // NOTE: Signature.
                    .WriteInt(); // NOTE: Alliance rank.

                return oPacket.ToArray();
            }
        }
    }
}
