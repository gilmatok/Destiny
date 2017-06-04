using Destiny.Core.IO;
using Destiny.Game.Characters;
using Destiny.Server.Data;
using Destiny.Network;

namespace Destiny.Game.Maps
{
    public sealed class Mob : MapObject, IMoveable, ISpawnable, IControllable
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

        public OutPacket GetCreatePacket()
        {
            return this.GetInternalPacket(false, true);
        }

        public OutPacket GetSpawnPacket()
        {
            return this.GetInternalPacket(false, false);
        }

        public OutPacket GetControlRequestPacket()
        {
            return this.GetInternalPacket(true, false);
        }

        private OutPacket GetInternalPacket(bool requestControl, bool newSpawn)
        {
            OutPacket oPacket = new OutPacket(requestControl ? SendOps.MobChangeController : SendOps.MobEnterField);

            if (requestControl)
            {
                oPacket.WriteByte(1); // TODO: 2 if mob is provoked (aggro).
            }

            oPacket
                .WriteInt(this.ObjectID)
                .WriteByte((byte)(this.Controller == null ? 5 : 1))
                .WriteInt(this.MapleID)
                .WriteZero(15) // NOTE: Unknown.
                .WriteByte(0x88) // NOTE: Unknown.
                .WriteZero(6) // NOTE: Unknown.
                .WritePoint(this.Position)
                .WriteByte((byte)(0x02 | (this.FacesLeft ? 0x01 : 0x00)))
                .WriteShort(this.Spawn.Foothold)
                .WriteShort(this.Foothold)
                .WriteByte((byte)(newSpawn ? -2 : -1))
                .WriteByte()
                .WriteByte(byte.MaxValue) // NOTE: Carnival team.
                .WriteInt(); // NOTE: Unknown.

            return oPacket;
        }

        public OutPacket GetControlCancelPacket()
        {
            OutPacket oPacket = new OutPacket(SendOps.MobChangeController);

            oPacket
                .WriteBool()
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(SendOps.MobLeaveField);

            oPacket
                .WriteByte() // TODO: Death effect.
                .WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}
