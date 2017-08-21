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
        public int Rank { get; private set; }

        private bool isOnline;

        public bool IsOnline
        {
            get
            {
                return isOnline;
            }
            set
            {
                isOnline = value;

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                {
                    oPacket
                        .WriteByte(61)
                        .WriteInt(this.Guild.ID)
                        .WriteInt(this.ID)
                        .WriteBool(this.IsOnline);

                    this.Guild.Broadcast(oPacket, this);
                }
            }
        }

        // NOTE: Constructed from database, upon server load.
        public GuildMember(Datum datum)
        {
            this.ID = (int)datum["ID"];
            this.Name = (string)datum["Name"];
            this.Level = (byte)datum["Level"];
            this.Job = (Job)datum["Job"];
            this.Rank = (int)datum["GuildRank"];
        }

        // NOTE: Constructed from already connected character, upon joining a guild.
        public GuildMember(Character character)
        {
            this.ID = character.ID;
            this.Name = character.Name;
            this.Level = character.Level;
            this.Job = character.Job;
            this.Rank = 5; // NOTE: Default rank.
        }
    }
}
