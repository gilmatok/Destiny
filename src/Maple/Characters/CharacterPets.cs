using Destiny.Core.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterPets : KeyedCollection<int, Pet>
    {
        public Character Parent { get; private set; }

        public CharacterPets(Character parent)
        {
            this.Parent = parent;
        }

        // NOTE: Pets are loaded using existing items associated with them.
        // Therefore, there is no need for a load method.

        public void Save()
        {

        }

        public void Delete()
        {

        }

        public void Spawn(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            short slot = iPacket.ReadShort();
            bool lead = iPacket.ReadBool();

            Item item = this.Parent.Items[ItemType.Cash, slot];

            if (item == null)
            {
                return;
            }

            Pet pet = this[item.PetID];

            if (pet.Summoned)
            {
                using (OutPacket oPacket = pet.GetDestroyPacket())
                {
                    this.Parent.Map.Broadcast(oPacket);
                }

                pet.Summoned = false;
            }
            else
            {
                pet.Position = this.Parent.Position;
                pet.Stance = 0;
                pet.Foothold = 0; // TODO: Get from parent's position.

                using (OutPacket oPacket = pet.GetCreatePacket())
                {
                    this.Parent.Map.Broadcast(oPacket);
                }

                pet.Summoned = true;
            }
        }

        public void Move(InPacket iPacket)
        {
            int petID = iPacket.ReadInt();

            Pet pet;

            try
            {
                pet = this[1];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            pet.Move(iPacket);
        }

        protected override void InsertItem(int index, Pet item)
        {
            item.Parent = this;

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Pet item = base.Items[index];

            item.Parent = null;

            base.RemoveItem(index);
        }

        protected override int GetKeyForItem(Pet item)
        {
            return item.ID;
        }
    }
}
