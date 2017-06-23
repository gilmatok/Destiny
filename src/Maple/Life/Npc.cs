using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Network;
using Destiny.Utility;

namespace Destiny.Maple.Life
{
    public sealed class Npc : LifeObject, ISpawnable, IControllable
    {
        public Npc(DatabaseQuery query) : base(query) { }

        public Character Controller { get; set; }

        public string ScriptPath
        {
            get
            {
                return string.Format(@"..\..\scripts\npcs\{0}.js", this.MapleID);
            }
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

        public OutPacket GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            return this.GetInternalPacket(false);
        }

        public OutPacket GetControlRequestPacket()
        {
            return this.GetInternalPacket(true);
        }

        private OutPacket GetInternalPacket(bool requestControl)
        {
            OutPacket oPacket = new OutPacket(requestControl ? SendOps.NpcChangeController : SendOps.NpcEnterField);

            if (requestControl)
            {
                oPacket.WriteBool(true);
            }

            oPacket
                .WriteInt(this.ObjectID)
                .WriteInt(this.MapleID)
                .WritePoint(this.Position)
                .WriteBool(!this.FacesLeft)
                .WriteShort(this.Foothold)
                .WriteShort(this.MinimumClickX)
                .WriteShort(this.MaximumClickX)
                .WriteBool(true); // NOTE: Hide.

            return oPacket;
        }

        public OutPacket GetControlCancelPacket()
        {
            OutPacket oPacket = new OutPacket(SendOps.NpcChangeController);

            oPacket
                .WriteBool()
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(SendOps.NpcLeaveField);

            oPacket.WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}
