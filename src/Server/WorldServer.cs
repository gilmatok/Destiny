using Destiny.Core.IO;
using Destiny.Data;
using Destiny.IO;
using Destiny.Maple.Characters;
using Destiny.Maple.Social;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Destiny.Server
{
    public sealed class WorldServer : KeyedCollection<byte, ChannelServer>
    {
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public IPAddress HostIP { get; private set; }
        public WorldFlag Flag { get; private set; }
        public string EventMessage { get; private set; }
        public string TickerMessage { get; set; }
        public int ExperienceRate { get; private set; }
        public int QuestExperienceRate { get; private set; }
        public int PartyQuestExperienceRate { get; private set; }
        public int MesoRate { get; private set; }
        public int DropRate { get; private set; }

        private int mMessengerIDs = 0;
        private Dictionary<int, Messenger> Messengers { get; set; }
        private int mPartyIDs = 0;
        private Dictionary<int, Party> Parties { get; set; }
        private int mGuildIDs = 0;
        private Dictionary<int, Guild> Guilds { get; set; }
        
        public WorldStatus Status
        {
            get
            {
                return WorldStatus.Normal; //TODO: Other statuses based on population
            }
        }

        public WorldServer(byte id)
            : base()
        {
            this.ID = id;
            string configSection = "World" + this.ID.ToString();

            this.Name = Settings.GetString(configSection + "/Name");
            this.HostIP = Settings.GetIPAddress(configSection + "/HostIP");
            this.Flag = Settings.GetEnum<WorldFlag>(configSection + "/Flag");
            this.EventMessage = Settings.GetString(configSection + "/EventMessage");
            this.TickerMessage = Settings.GetString(configSection + "/TickerMessage");
            this.ExperienceRate = Settings.GetInt(configSection + "/ExperienceRate");
            this.QuestExperienceRate = Settings.GetInt(configSection + "/QuestExperienceRate");
            this.PartyQuestExperienceRate = Settings.GetInt(configSection + "/PartyQuestExperienceRate");
            this.MesoRate = Settings.GetInt(configSection + "/MesoDropRate");
            this.DropRate = Settings.GetInt(configSection + "/ItemDropRate");

            this.Messengers = new Dictionary<int, Messenger>();
            this.Parties = new Dictionary<int, Party>();
            this.Guilds = new Dictionary<int, Guild>();

            byte channels = Settings.GetByte(configSection + "/Channels");

            for (byte i = 0; i < channels; i++)
            {
                this.Add(new ChannelServer(i, this, (short)(8585 + 100 * id + i)));
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

        public void Notify(string text, NoticeType type = NoticeType.Notice)
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

        public Messenger GetMessenger(int id)
        {
            Messenger ret = null;

            this.Messengers.TryGetValue(id, out ret);

            return ret;
        }

        public void CreateMessenger(Character host)
        {
            int id = ++mMessengerIDs;

            this.Messengers.Add(id, new Messenger(id, host));
        }

        public void RemoveMessenger(int id)
        {
            this.Messengers.Remove(id);
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

        public void CreateGuild(Character master, string name)
        {
            int id = ++mGuildIDs;

            this.Guilds.Add(id, new Guild(id, name, master));
        }

        protected override byte GetKeyForItem(ChannelServer item)
        {
            return item.ID;
        }
    }
}
