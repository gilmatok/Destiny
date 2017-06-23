using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Utility;

namespace Destiny.Maple.Life
{
    public sealed class Mob : MapObject, IMoveable, ISpawnable, IControllable
    {
        public int MapleID { get; private set; }
        public Character Controller { get; set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }

        public bool FacesLeft
        {
            get
            {
                return this.Stance % 2 == 0;
            }
        }

        public Mob(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("mobid");
        }

        public Mob(int mapleID)
        {
            this.MapleID = mapleID;
        }

        public Mob(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;
            this.Foothold = this.SpawnPoint.Foothold;
            this.Position = this.SpawnPoint.Position;
        }

        public void Move(InPacket iPacket)
        {
            short moveAction = iPacket.ReadShort();
            bool cheatResult = (iPacket.ReadByte() & 0xF) != 0;
            byte centerSplit = iPacket.ReadByte();
            int illegalVelocity = iPacket.ReadInt();
            byte unknown = iPacket.ReadByte();
            iPacket.ReadInt();

            Movements movements = Movements.Decode(iPacket);

            Movement lastMovement = movements[movements.Count - 1];

            this.Position = lastMovement.Position;
            this.Foothold = lastMovement.Foothold;
            this.Stance = lastMovement.Stance;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MobCtrlAck))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteShort(moveAction)
                    .WriteBool(cheatResult)
                    .WriteShort() // NOTE: Mob mana.
                    .WriteByte() // NOTE: Ability ID.
                    .WriteByte(); // NOTE: Ability level.

                this.Controller.Client.Send(oPacket);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MobMove))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteBool(cheatResult)
                    .WriteByte(centerSplit)
                    .WriteInt(illegalVelocity);

                movements.Encode(oPacket);

                this.Map.Broadcast(oPacket, this.Controller);
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
            OutPacket oPacket = new OutPacket(requestControl ? ServerOperationCode.MobChangeController : ServerOperationCode.MobEnterField);

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
                .WriteShort(this.Foothold)
                .WriteShort(this.Foothold)
                .WriteByte((byte)(newSpawn ? -2 : -1))
                .WriteByte()
                .WriteByte(byte.MaxValue) // NOTE: Carnival team.
                .WriteInt(); // NOTE: Unknown.

            return oPacket;
        }

        public OutPacket GetControlCancelPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.MobChangeController);

            oPacket
                .WriteBool()
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.MobLeaveField);

            oPacket
                .WriteByte() // TODO: Death effect.
                .WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}
