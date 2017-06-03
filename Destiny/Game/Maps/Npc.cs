using Destiny.Core.IO;
using Destiny.Game.Characters;
using Destiny.Game.Data;
using Destiny.Server;
using Destiny.Network;

namespace Destiny.Game.Maps
{
    public sealed class Npc : MapObject, ISpawnable, IControllable
    {
        public int MapleID { get; private set; }
        public NpcData Data { get; private set; }
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
            this.Data = MasterServer.Instance.Data.Npcs[this.MapleID];
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
                .WriteBool(!this.Spawn.Flip)
                .WriteShort(this.Spawn.Foothold)
                .WriteShort(this.Spawn.MinimumClickX)
                .WriteShort(this.Spawn.MaximumClickX)
                .WriteBool(!this.Spawn.Hide);

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
