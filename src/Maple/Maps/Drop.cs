using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Threading;
using Destiny.Packets;

namespace Destiny.Maple.Maps
{
    public abstract class Drop : MapObject, ISpawnable
    {
        public const int ExpiryTime = 60 * 1000;

        public Character Owner { get; set; }
        public Character Picker { get; set; }
        public Point Origin { get; set; }
        public Delay Expiry { get; set; }

        private MapObject mDropper;

        public MapObject Dropper
        {
            get
            {
                return mDropper;
            }
            set
            {
                mDropper = value;

                this.Origin = mDropper.Position;
                this.Position = mDropper.Map.Footholds.FindFloor(mDropper.Position);
            }
        }

        public abstract OutPacket GetShowGainPacket();

        public OutPacket GetCreatePacket()
        {
            return this.GetInternalPacket(true, null);
        }

        public OutPacket GetCreatePacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(true, temporaryOwner);
        }

        public OutPacket GetSpawnPacket()
        {
            return this.GetInternalPacket(false, null);
        }

        public OutPacket GetSpawnPacket(Character temporaryOwner)
        {
            return this.GetInternalPacket(false, temporaryOwner);
        }

        private OutPacket GetInternalPacket(bool dropped, Character temporaryOwner)
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.DropEnterField);

            oPacket
                .WriteByte((byte)(dropped ? 1 : 2)) // TODO: Other types; 3 = disappearing, and 0 probably is something as well.
                .WriteInt(this.ObjectID)
                .WriteBool(this is Meso);

            if (this is Meso)
            {
                oPacket.WriteInt(((Meso)this).Amount);
            }
            else if (this is Item)
            {
                oPacket.WriteInt(((Item)this).MapleID);
            }

            oPacket
                .WriteInt(this.Owner != null ? this.Owner.ID : temporaryOwner.ID)
                .WriteByte() // TODO: Type implementation (0 - normal, 1 - party, 2 - FFA, 3 - explosive)
                .WritePoint(this.Position)
                .WriteInt(this.Dropper.ObjectID);

            if (dropped)
            {
                oPacket.WritePoint(this.Origin);
                oPacket.WriteShort(); // NOTE: Foothold, probably.
            }

            if (this is Item)
            {
                oPacket.WriteLong(); // NOTE: Item expiration.
            }

            oPacket.WriteByte(); // NOTE: Pet equip pick-up.

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.DropLeaveField);

            oPacket
                .WriteByte((byte)(this.Picker == null ? 0 : 2))
                .WriteInt(this.ObjectID)
                .WriteInt(this.Picker != null ? this.Picker.ID : 0);

            return oPacket;
        }
    }
}
