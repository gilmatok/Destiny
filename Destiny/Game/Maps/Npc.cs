using Destiny.Game.Characters;
using Destiny.Game.Data;

namespace Destiny.Game.Maps
{
    public sealed class Npc : MapObject
    {
        public int MapleID { get; private set; }
        public MapNpcSpawnData Spawn { get; private set; }
        public Character Controller { get; set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Npc;
            }
        }

        public Npc(int mapleID)
        {
            this.MapleID = mapleID;
        }

        public Npc(MapNpcSpawnData spawn)
            : this(spawn.MapleID)
        {
            this.Spawn = spawn;
            this.Position = spawn.Positon;
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character character in this.Map.Characters)
                    {
                        if (character.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledNpcs.Count;
                            newController = character;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledNpcs.Add(this);
                }
            }
        }
    }
}
