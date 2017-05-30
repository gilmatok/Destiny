using Destiny.Game.Characters;
using System.IO;

namespace Destiny.Game.Maps
{
    public sealed class Npc : MapObject
    {
        public int MapleID { get; private set; }
        public bool Flip { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public bool Hide { get; private set; }

        public Character Controller { get; set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Npc;
            }
        }

        public Npc(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.Foothold = reader.ReadInt16();
            this.Flip = reader.ReadBoolean();
            this.MinimumClickX = reader.ReadInt16();
            this.MaximumClickX = reader.ReadInt16();
            this.Hide = reader.ReadBoolean();
            reader.ReadInt32();
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
