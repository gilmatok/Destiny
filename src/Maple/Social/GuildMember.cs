using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Social
{
    public sealed class GuildMember
    {
        public Guild Guild { get; private set; }

        public int ID { get; private set; }
        public string Name { get; private set; }
        public byte Level { get; private set; }
        public Job Job { get; private set; }
        public int Rank { get; private set; }
        public bool IsOnline { get; private set; }

        public GuildMember(Guild guild, Datum datum)
        {
            this.Guild = guild;

            this.ID = (int)datum["ID"];
            this.Name = (string)datum["Name"];
            this.Level = (byte)datum["Level"];
            this.Job = (Job)datum["Job"];
            this.Rank = (int)datum["GuildRank"];
            this.IsOnline = false;
        }

        public GuildMember(Guild guild, Character character)
        {
            this.Guild = guild;

            this.ID = character.ID;
            this.Name = character.Name;
            this.Level = character.Level;
            this.Job = character.Job;
            this.Rank = character.GuildRank;
            this.IsOnline = true;
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
