using Destiny.Game.Characters;
using Destiny.Game.Data;

namespace Destiny.Game.Maps
{
    public sealed class Mob : MapObject, IMoveable
    {
        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Mob;
            }
        }

        public int MapleID { get; private set; }
        public MapMobSpawnData Spawn { get; private set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public Character Controller { get; set; }

        public bool FacesLeft
        {
            get
            {
                return this.Stance % 2 == 0;
            }
        }

        public Mob(int mapleID)
        {
            this.MapleID = mapleID;
        }

        public Mob(MapMobSpawnData spawn)
            : this(spawn.MapleID)
        {
            this.Spawn = spawn;
            this.Stance = (byte)(spawn.Flip ? 0 : 1);
            this.Foothold = spawn.Foothold;
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
                        if (character.ControlledMobs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledMobs.Count;
                            newController = character;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledMobs.Add(this);
                }
            }
        }
    }
}
