using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Shops;

namespace Destiny.Maple.Life
{
    public sealed class Npc : LifeObject, ISpawnable, IControllable
    {
        public Npc(Datum datum) : base(datum) { }

        public Character Controller { get; set; }

        public Shop Shop { get; set; }

        public void Move(InPacket iPacket)
        {
            byte action1 = iPacket.ReadByte();
            byte action2 = iPacket.ReadByte();

            Movements movements = null;

            if (iPacket.Remaining > 0)
            {
                movements = Movements.Decode(iPacket);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.NpcMove))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteByte(action1)
                    .WriteByte(action2);

                if (movements != null)
                {
                    oPacket.WriteBytes(movements.ToByteArray());
                }

                this.Map.Broadcast(oPacket);
            }
        }

        public void Converse(Character talker)
        {
            if (this.Shop != null)
            {
                this.Shop.Show(talker);
            }
            else if (false) // TODO: Check if this Npc is a storage (usually by checking if it's storage deposit cost variable is not 0).
            {
                talker.Storage.Show(this);
            }
            else // NOTE: If this Npc is not a shop or a storage, it's a script.
            {
                // TODO: Start script.
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
