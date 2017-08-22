using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Social;
using System.Collections.Generic;
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

        private int mPartyIDs = 0;
        private Dictionary<int, Party> Parties { get; set; }
        private int mGuildIDs = 0;
        private Dictionary<int, Guild> Guilds { get; set; }

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

            this.Parties = new Dictionary<int, Party>();
            this.Guilds = new Dictionary<int, Guild>();

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

            foreach (Datum datum in new Datums("guilds").Populate("WorldID = {0}", this.ID))
            {
                this.Guilds.Add((int)datum["ID"], new Guild(datum));
            }

            foreach (Guild guild in this.Guilds.Values)
            {
                foreach (Datum datum in new Datums("characters").PopulateWith("ID, Name, Level, Job, GuildRank", "GuildID = {0}", guild.ID))
                {
                    guild.Add(new GuildMember(datum));
                }

                mGuildIDs = guild.ID; // NOTE: Setting the last used guild ID.
            }

            // TODO: Load Guild BBS.
        }

        public void Stop()
        {
            foreach (ChannelServer channel in this)
            {
                channel.Stop();
            }
        }

        public void Broadcast(OutPacket oPacket)
        {
            foreach (ChannelServer channel in this)
            {
                channel.Broadcast(oPacket);
            }
        }

        public void Notify(string text, NoticeType type)
        {
            foreach (ChannelServer channel in this)
            {
                channel.Notify(text, type);
            }
        }

        public bool IsCharacterOnline(int id)
        {
            foreach (ChannelServer channel in this)
            {
                if (channel.Characters.Contains(id))
                {
                    return true;
                }
            }

            return false;
        }

        public Character GetCharacter(int id)
        {
            Character character = null;

            foreach (ChannelServer channel in this)
            {
                character = channel.Characters.GetCharacter(id);

                if (character != null)
                {
                    break;
                }
            }

            return character;
        }

        public bool IsCharacterOnline(string name)
        {
            foreach (ChannelServer channel in this)
            {
                if (channel.Characters.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        public Character GetCharacter(string name)
        {
            Character character = null;

            foreach (ChannelServer channel in this)
            {
                character = channel.Characters.GetCharacter(name);

                if (character != null)
                {
                    break;
                }
            }

            return character;
        }

        public Party GetParty(int id)
        {
            Party ret = null;

            this.Parties.TryGetValue(id, out ret);

            return ret;
        }

        public void CreateParty(Character leader)
        {
            int id = ++mPartyIDs;

            this.Parties.Add(id, new Party(this, id, leader));
        }

        public void RemoveParty(Party party)
        {
            this.Parties.Remove(party.ID);
        }

        public Guild GetGuild(int id)
        {
            Guild ret = null;

            this.Guilds.TryGetValue(id, out ret);

            return ret;
        }

        public void CreateGuild(Character master)
        {

        }

        protected override byte GetKeyForItem(ChannelServer item)
        {
            return item.ID;
        }
    }
}
