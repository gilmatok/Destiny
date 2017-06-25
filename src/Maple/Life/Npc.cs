using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Network;
using Destiny.Data;

namespace Destiny.Maple.Life
{
    public sealed class Npc : LifeObject, ISpawnable, IControllable
    {
        public Npc(Datum datum) : base(datum) { }

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
            OutPacket oPacket = new OutPacket(requestControl ? ServerOperationCode.NpcChangeController : ServerOperationCode.NpcEnterField);

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
            OutPacket oPacket = new OutPacket(ServerOperationCode.NpcChangeController);

            oPacket
                .WriteBool()
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.NpcLeaveField);

            oPacket.WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}
