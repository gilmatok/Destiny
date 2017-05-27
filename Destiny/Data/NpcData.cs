using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class NpcData
    {
        [Flags]
        public enum ENpcFlags : byte
        {
            None = 0 << 0,
            Maple_TV = 1 << 0,
            MapleTV = 1 << 0,
            Is_Guild_Rank = 1 << 1,
            IsGuildRank = 1 << 1
        }

        public sealed class NpcShopData
        {
            public sealed class NpcShopItemData
            {
                public int ItemIdentifier { get; set; }
                public ushort Quantity { get; set; }
                public int Price { get; set; }
                public float RechargePrice { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(ItemIdentifier);
                    pWriter.Write(Quantity);
                    pWriter.Write(Price);
                    pWriter.Write(RechargePrice);
                }

                public void Load(BinaryReader pReader)
                {
                    ItemIdentifier = pReader.ReadInt32();
                    Quantity = pReader.ReadUInt16();
                    Price = pReader.ReadInt32();
                    RechargePrice = pReader.ReadSingle();
                }
            }


            public int ShopIdentifier { get; set; }
            public byte RechargeTier { get; set; }
            public List<NpcShopItemData> Items { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(ShopIdentifier);
                pWriter.Write(RechargeTier);

                pWriter.Write(Items.Count);
                Items.ForEach(i => i.Save(pWriter));
            }

            public void Load(BinaryReader pReader)
            {
                ShopIdentifier = pReader.ReadInt32();
                RechargeTier = pReader.ReadByte();

                int itemsCount = pReader.ReadInt32();
                Items = new List<NpcShopItemData>(itemsCount);
                while (itemsCount-- > 0)
                {
                    NpcShopItemData item = new NpcShopItemData();
                    item.Load(pReader);
                    Items.Add(item);
                }
            }
        }


        public int Identifier { get; set; }
        public ENpcFlags Flags { get; set; }
        public ushort StorageCost { get; set; }
        public List<NpcShopData> Shops { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((byte)Flags);
            pWriter.Write(StorageCost);

            pWriter.Write(Shops.Count);
            Shops.ForEach(s => s.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (ENpcFlags)pReader.ReadByte();
            StorageCost = pReader.ReadUInt16();

            int shopsCount = pReader.ReadInt32();
            Shops = new List<NpcShopData>(shopsCount);
            while (shopsCount-- > 0)
            {
                NpcShopData shop = new NpcShopData();
                shop.Load(pReader);
                Shops.Add(shop);
            }
        }
    }
}
