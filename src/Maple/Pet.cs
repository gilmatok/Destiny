using Destiny.Core.IO;
using Destiny.Maple.Maps;
using Destiny.Core.Network;
using Destiny.Maple.Characters;
using Destiny.Data;

namespace Destiny.Maple
{
    public sealed class Pet : MapObject, ISpawnable, IMoveable
    {
        public CharacterPets Parent { get; set; }

        public int ID { get; private set; }
        public Item Item { get; private set; }
        public string Name { get; private set; }
        public byte Level { get; private set; }
        public short Closeness { get; private set; }
        public byte Fullness { get; private set; }
        public short Foothold { get; set; }
        public byte Stance { get; set; }
        public bool Summoned { get; set; }

        public Character Character
        {
            get
            {
                return this.Parent.Parent;
            }
        }

        public Pet(Item item)
            : base()
        {
            Datum datum = new Datum("pets");

            try
            {
                datum.Populate("ID = {0}", item.PetID);
            }
            catch
            {
                // NOTE: Item associated with a pet that does not exist.
                // TODO: Attempt to recreate the pet by assigning defaults.

                return;
            }

            this.ID = item.PetID;
            this.Item = item;
            this.Name = (string)datum["Name"];
            this.Level = (byte)datum["Level"];
            this.Closeness = (short)datum["Closeness"];
            this.Fullness = (byte)datum["Fullness"];
        }

        public void Move(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Unknown.
            Movements movements = Movements.Decode(iPacket);

            this.Position = movements.Position;
            this.Foothold = movements.Foothold;
            this.Stance = movements.Stance;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.PetMove))
            {
                oPacket
                    .WriteInt(this.Character.ID)
                    .WriteByte() // NOTE: Index.
                    .WriteBytes(movements.ToByteArray());

                this.Character.Map.Broadcast(oPacket, this.Character);
            }
        }

        public OutPacket GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.PetEnterField);

            oPacket
                .WriteInt(this.Character.ID)
                .WriteByte() // NOTE: Index.
                .WriteBool(true)
                .WriteBool(false) // NOTE: Kick existing pet (only when player doesn't have Follow the Lead skill).
                .WriteInt(this.Item.MapleID)
                .WriteMapleString(this.Name)
                .WriteInt() // NOTE: Unique ID.
                .WriteInt()
                .WritePoint(this.Position)
                .WriteByte(this.Stance)
                .WriteShort(this.Foothold)
                .WriteBool() // NOTE: Item tag.
                .WriteBool(); // NOTE: Quote ring.

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.PetEnterField);

            oPacket
             .WriteInt(this.Character.ID)
             .WriteByte() // NOTE: Index.
             .WriteBool(false)
             .WriteBool(false); // NOTE: Kick existing pet (only when player doesn't have Follow the Lead skill).

            return oPacket;
        }
    }
}
