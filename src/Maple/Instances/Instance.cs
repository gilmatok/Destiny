using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using Destiny.Maple.Social;
using Destiny.Server;
using Destiny.Threading;
using System.Collections.Generic;
using System.Threading;

namespace Destiny.Maple.Instances
{
    public abstract class Instance
    {
        public ChannelServer Channel { get; private set; }

        public abstract string Name { get; }
        public abstract bool ShowTimer { get; }
        public List<Map> Maps { get; private set; }
        public List<Party> Parties { get; private set; }
        public Dictionary<int, Character> Characters { get; private set; }
        public Dictionary<string, Delay> Timers { get; private set; }

        public int RemainingSeconds
        {
            get
            {
                return this.Timers.ContainsKey("Main") ? (int)this.Timers["Main"].DueTime.TotalSeconds : 0;
            }
        }

        public Instance(ChannelServer channel, int time)
        {
            this.Channel = channel;

            this.Maps = new List<Map>();
            this.Parties = new List<Party>();
            this.Characters = new Dictionary<int, Character>();
            this.Timers = new Dictionary<string, Delay>();

            this.AddTimer("Main", time);
        }

        public void AddTimer(string label, int timeout, int repeat = Timeout.Infinite)
        {
            this.Timers.Add(label, new Delay(() =>
            {
                this.TimerEnd(label);
            }, timeout, repeat));
        }

        public void AddMap(int mapID)
        {
            Map map = this.Channel.Maps[mapID];

            map.Instance = this;

            this.Maps.Add(map);
        }

        public bool ContainsMap(Map map)
        {
            return this.Maps.Contains(map);
        }

        public void AddParty(Party party)
        {
            party.Instance = this;

            this.Parties.Add(party);
        }

        public void AddCharacter(Character character)
        {
            character.Instance = this;

            this.Characters.Add(character.ID, character);
        }

        public void RemoveCharacter(Character character)
        {
            character.Instance = null;

            this.Characters.Remove(character.ID);
        }

        public void RemoveCharacters()
        {
            List<Character> toRemove = new List<Character>();

            lock (this.Characters)
            {
                foreach (Character character in this.Characters.Values)
                {
                    toRemove.Add(character);
                }
            }

            foreach (Character character in toRemove)
            {
                this.RemoveCharacter(character);
            }
        }

        public void WarpCharacters(int mapID)
        {
            lock (this.Characters)
            {
                foreach (Character character in this.Characters.Values)
                {
                    character.ChangeMap(mapID);
                }
            }
        }

        public virtual void TimerEnd(string label) { }
        public virtual void CharacterDeath(Character character) { }
        public virtual void CharacterDisconnect(Character character, bool isPartyLeader) { }
        public virtual void CharacterMapChange(Character character, Map oldMap, Map newMap, bool isPartyLeader) { }
        public virtual void MobSpawn(Mob mob) { }
        public virtual void MobDeath(Mob mob) { }
        public virtual void PartyDisband(Party party) { }
        public virtual void PartyRemoveMember(Party party, PartyMember member) { }
    }
}
