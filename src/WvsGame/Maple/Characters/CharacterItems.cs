using Destiny.Maple.Maps;
using System.Collections.Generic;
using System;
using System.Collections;
using Destiny.Data;
using Destiny.Network;
using System.Linq;
using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.IO;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterItems : IEnumerable<Item>
    {
        public Character Parent { get; private set; }
        public Dictionary<ItemType, byte> MaxSlots { get; private set; }
        private List<Item> Items { get; set; }

        public CharacterItems(Character parent, byte equipmentSlots, byte usableSlots, byte setupSlots, byte etceteraSlots, byte cashSlots)
            : base()
        {
            this.Parent = parent;

            this.MaxSlots = new Dictionary<ItemType, byte>(Enum.GetValues(typeof(ItemType)).Length);

            this.MaxSlots.Add(ItemType.Equipment, equipmentSlots);
            this.MaxSlots.Add(ItemType.Usable, usableSlots);
            this.MaxSlots.Add(ItemType.Setup, setupSlots);
            this.MaxSlots.Add(ItemType.Etcetera, etceteraSlots);
            this.MaxSlots.Add(ItemType.Cash, cashSlots);

            this.Items = new List<Item>();
        }

        public void Load()
        {
            // TODO: Use JOIN with the pets table.
            foreach (Datum datum in new Datums("items").Populate("CharacterID = {0} AND IsStored = 0", this.Parent.ID))
            {
                Item item = new Item(datum);

                this.Add(item);

                if (item.PetID != null)
                {
                    //this.Parent.Pets.Add(new Pet(item));
                }
            }
        }

        public void Save()
        {
            foreach (Item item in this)
            {
                item.Save();
            }
        }

        public void Add(Item item, bool fromDrop = false, bool autoMerge = true, bool forceGetSlot = false)
        {
            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this.Where(x => x.MapleID == item.MapleID && x.Quantity < x.MaxPerStack))
                {
                    if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                    {
                        loopItem.Quantity += item.Quantity;
                        loopItem.Update();

                        item.Quantity = 0;

                        break;
                    }
                    else
                    {
                        item.Quantity -= (short)(loopItem.MaxPerStack - loopItem.Quantity);
                        item.Slot = this.GetNextFreeSlot(item.Type);

                        loopItem.Quantity = loopItem.MaxPerStack;

                        if (this.Parent.IsInitialized)
                        {
                            loopItem.Update();
                        }

                        break;
                    }
                }
            }

            if (item.Quantity > 0)
            {
                item.Parent = this;

                if ((this.Parent.IsInitialized && item.Slot == 0) || forceGetSlot)
                {
                    item.Slot = this.GetNextFreeSlot(item.Type);
                }

                this.Items.Add(item);

                if (this.Parent.IsInitialized)
                {
                    using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                    {
                        oPacket
                            .WriteBool(fromDrop)
                            .WriteByte(1)
                            .WriteByte((byte)InventoryOperationType.AddItem)
                            .WriteByte((byte)item.Type)
                            .WriteShort(item.Slot)
                            .WriteBytes(item.ToByteArray(true));

                        this.Parent.Client.Send(oPacket);
                    }
                }
            }
        }

        public void AddRange(IEnumerable<Item> items, bool fromDrop = false, bool autoMerge = true)
        {
            foreach (Item loopItem in items)
            {
                this.Add(loopItem, fromDrop, autoMerge);
            }
        }

        public void Remove(int mapleId, short quantity)
        {
            short leftToRemove = quantity;

            List<Item> toRemove = new List<Item>();

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    if (loopItem.Quantity > leftToRemove)
                    {
                        loopItem.Quantity -= leftToRemove;
                        loopItem.Update();

                        break;
                    }
                    else
                    {
                        leftToRemove -= loopItem.Quantity;
                        toRemove.Add(loopItem);
                    }
                }
            }

            foreach (Item loopItem in toRemove)
            {
                this.Remove(loopItem, true);
            }
        }

        public void Remove(Item item, bool removeFromSlot, bool fromDrop = false)
        {
            if (removeFromSlot && item.IsEquipped)
            {
                throw new InvalidOperationException("Cannot remove equipped items from slot.");
            }

            if (removeFromSlot)
            {
                using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                {
                    oPacket
                        .WriteBool(fromDrop)
                        .WriteByte(1)
                        .WriteByte((byte)InventoryOperationType.RemoveItem)
                        .WriteByte((byte)item.Type)
                        .WriteShort(item.Slot);

                    this.Parent.Client.Send(oPacket);
                }
            }

            if (item.Assigned)
            {
                item.Delete();
            }

            item.Parent = null;

            bool wasEquipped = item.IsEquipped;

            this.Items.Remove(item);

            if (wasEquipped)
            {
                this.Parent.UpdateApperance();
            }
        }

        public void Clear(bool removeFromSlot)
        {
            List<Item> toRemove = new List<Item>();

            foreach (Item loopItem in this)
            {
                toRemove.Add(loopItem);
            }

            foreach (Item loopItem in toRemove)
            {
                if (!(loopItem.IsEquipped && removeFromSlot))
                {
                    this.Remove(loopItem, removeFromSlot);
                }
            }
        }

        public bool Contains(int mapleId)
        {
            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Contains(int mapleId, short quantity)
        {
            int count = 0;

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    count += loopItem.Quantity;
                }
            }

            return count >= quantity;
        }

        public int Available(int mapleId)
        {
            int count = 0;

            foreach (Item loopItem in this)
            {
                if (loopItem.MapleID == mapleId)
                {
                    count += loopItem.Quantity;
                }
            }

            return count;
        }

        public sbyte GetNextFreeSlot(ItemType type)
        {
            for (sbyte i = 1; i <= this.MaxSlots[type]; i++)
            {
                if (this[type, i] == null)
                {
                    return i;
                }
            }

            throw new InventoryFullException();
        }

        public void NotifyFull()
        {

        }

        public bool IsFull(ItemType type)
        {
            short count = 0;

            foreach (Item item in this)
            {
                if (item.Type == type)
                {
                    count++;
                }
            }

            return (count == this.MaxSlots[type]);
        }

        public int RemainingSlots(ItemType type)
        {
            short remaining = this.MaxSlots[type];

            foreach (Item item in this)
            {
                if (item.Type == type)
                {
                    remaining--;
                }
            }

            return remaining;
        }

        public void Sort(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            ItemType type = (ItemType)iPacket.ReadByte();
        }

        public void Gather(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            ItemType type = (ItemType)iPacket.ReadByte();
        }

        public void Handle(Packet iPacket)
        {
            iPacket.ReadInt();

            ItemType type = (ItemType)iPacket.ReadByte();
            short source = iPacket.ReadShort();
            short destination = iPacket.ReadShort();
            short quantity = iPacket.ReadShort();

            try
            {
                Item item = this[type, source];

                if (destination < 0)
                {
                    item.Equip();
                }
                else if (source < 0 && destination > 0)
                {
                    item.Unequip(destination);
                }
                else if (destination == 0)
                {
                    item.Drop(quantity);
                }
                else
                {
                    item.Move(destination);
                }
            }
            catch (InventoryFullException)
            {
                this.NotifyFull();
            }
        }

        public void UseItem(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            short slot = iPacket.ReadShort();
            int itemID = iPacket.ReadInt();

            Item item = this[ItemType.Usable, slot];

            if (item == null || itemID != item.MapleID)
            {
                return;
            }

            this.Remove(itemID, 1);

            if (item.CHealth > 0)
            {
                this.Parent.Health += item.CHealth;
            }

            if (item.CMana > 0)
            {
                this.Parent.Mana += item.CMana;
            }

            if (item.CHealthPercentage != 0)
            {
                this.Parent.Health += (short)((item.CHealthPercentage * this.Parent.MaxHealth) / 100);
            }

            if (item.CManaPercentage != 0)
            {
                this.Parent.Mana += (short)((item.CManaPercentage * this.Parent.MaxMana) / 100);
            }

            if (item.CBuffTime > 0 && item.CProb == 0)
            {
                // TODO: Add buff.
            }

            if (false)
            {
                // TODO: Add Monster Book card.
            }
        }

        public void UseSummonBag(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            short slot = iPacket.ReadShort();
            int itemID = iPacket.ReadInt();

            Item item = this[ItemType.Usable, slot];

            if (item == null || itemID != item.MapleID)
            {
                return;
            }

            this.Remove(itemID, 1);

            foreach (Tuple<int, short> summon in item.Summons)
            {
                if (Application.Random.Next(0, 100) < summon.Item2)
                {
                    if (DataProvider.Mobs.Contains(summon.Item1))
                    {
                        this.Parent.Map.Mobs.Add(new Mob(summon.Item1, this.Parent.Position));
                    }
                }
            }
        }

        public void UseCashItem(Packet iPacket)
        {
            short slot = iPacket.ReadShort();
            int itemID = iPacket.ReadInt();

            Item item = this[ItemType.Cash, slot];

            if (item == null || itemID != item.MapleID)
            {
                return;
            }

            bool used = false;

            switch (item.MapleID) // TODO: Enum for these.
            {
                case 5040000: // NOTE: Teleport Rock.
                case 5040001: // NOTE: Coke Teleport Rock.
                case 5041000: // NOTE: VIP Teleport Rock.
                    {
                        used = this.Parent.Trocks.Use(itemID, iPacket);
                    }
                    break;

                case 5050001: // NOTE: 1st Job SP Reset.
                case 5050002: // NOTE: 2nd Job SP Reset.
                case 5050003: // NOTE: 3rd Job SP Reset.
                case 5050004: // NOTE: 4th Job SP Reset.
                    {

                    }
                    break;

                case 5050000: // NOTE: AP Reset.
                    {
                        StatisticType statDestination = (StatisticType)iPacket.ReadInt();
                        StatisticType statSource = (StatisticType)iPacket.ReadInt();

                        this.Parent.AddAbility(statDestination, 1, true);
                        this.Parent.AddAbility(statSource, -1, true);

                        used = true;
                    }
                    break;

                case 5071000: // NOTE: Megaphone.
                    {
                        if (this.Parent.Level <= 10)
                        {
                            // NOTE: You can't use a megaphone unless you're over level 10.

                            return;
                        }

                        string text = iPacket.ReadString();

                        string message = string.Format($"{this.Parent.Name} : {text}"); // TODO: Include medal name.

                        // NOTE: In GMS, this sends to everyone on the current channel, not the map (despite the item's description).
                        using (Packet oPacket = new Packet(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.Megaphone)
                                .WriteString(message);

                            //this.Parent.Client.Channel.Broadcast(oPacket);
                        }

                        used = true;
                    }
                    break;

                case 5072000: // NOTE: Super Megaphone.
                    {
                        if (this.Parent.Level <= 10)
                        {
                            // NOTE: You can't use a megaphone unless you're over level 10.

                            return;
                        }

                        string text = iPacket.ReadString();
                        bool whisper = iPacket.ReadBool();

                        string message = string.Format($"{this.Parent.Name} : {text}"); // TODO: Include medal name.

                        using (Packet oPacket = new Packet(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.SuperMegaphone)
                                .WriteString(message)
                                .WriteByte(WvsGame.ChannelID)
                                .WriteBool(whisper);

                            //this.Parent.Client.World.Broadcast(oPacket);
                        }

                        used = true;
                    }
                    break;

                case 5390000: // NOTE: Diablo Messenger.
                case 5390001: // NOTE: Cloud 9 Messenger.
                case 5390002: // NOTE: Loveholic Messenger.
                    {
                        if (this.Parent.Level <= 10)
                        {
                            // NOTE: You can't use a megaphone unless you're over level 10.

                            return;
                        }

                        string text1 = iPacket.ReadString();
                        string text2 = iPacket.ReadString();
                        string text3 = iPacket.ReadString();
                        string text4 = iPacket.ReadString();
                        bool whisper = iPacket.ReadBool();

                        using (Packet oPacket = new Packet(ServerOperationCode.SetAvatarMegaphone))
                        {
                            oPacket
                                .WriteInt(itemID)
                                .WriteString(this.Parent.Name)
                                .WriteString(text1)
                                .WriteString(text2)
                                .WriteString(text3)
                                .WriteString(text4)
                                .WriteInt(WvsGame.ChannelID)
                                .WriteBool(whisper)
                                .WriteBytes(this.Parent.AppearanceToByteArray());

                            //this.Parent.Client.World.Broadcast(oPacket);
                        }

                        used = true;
                    }
                    break;

                case 5076000: // NOTE: Item Megaphone.
                    {
                        string text = iPacket.ReadString();
                        bool whisper = iPacket.ReadBool();
                        bool includeItem = iPacket.ReadBool();

                        Item targetItem = null;

                        if (includeItem)
                        {
                            ItemType type = (ItemType)iPacket.ReadInt();
                            short targetSlot = iPacket.ReadShort();

                            targetItem = this[type, targetSlot];

                            if (targetItem == null)
                            {
                                return;
                            }
                        }

                        string message = string.Format($"{this.Parent.Name} : {text}"); // TODO: Include medal name.

                        using (Packet oPacket = new Packet(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.ItemMegaphone)
                                .WriteString(message)
                                .WriteByte(WvsGame.ChannelID)
                                .WriteBool(whisper)
                                .WriteByte((byte)(targetItem != null ? targetItem.Slot : 0));

                            if (targetItem != null)
                            {
                                oPacket.WriteBytes(targetItem.ToByteArray(true));
                            }

                            //this.Parent.Client.World.Broadcast(oPacket);
                        }

                        used = true;
                    }
                    break;

                case 5077000: // NOTE: Art Megaphone.
                    {

                    }
                    break;

                case 5170000: // NOTE: Pet Name Tag.
                    {
                        //// TODO: Get the summoned pet.

                        //string name = iPacket.ReadString();

                        //using (Packet oPacket = new Packet(ServerOperationCode.PetNameChanged))
                        //{
                        //    oPacket
                        //        .WriteInt(this.Parent.ID)
                        //        .WriteByte() // NOTE: Index.
                        //        .WriteString(name)
                        //        .WriteByte();

                        //    this.Parent.Map.Broadcast(oPacket);
                        //}
                    }
                    break;

                case 5060000: // NOTE: Item Name Tag.
                    {
                        short targetSlot = iPacket.ReadShort();

                        if (targetSlot == 0)
                        {
                            return;
                        }

                        Item targetItem = this[ItemType.Equipment, targetSlot];

                        if (targetItem == null)
                        {
                            return;
                        }

                        targetItem.Creator = this.Parent.Name;
                        targetItem.Update(); // TODO: This does not seem to update the item's creator.

                        used = true;
                    }
                    break;

                case 5520000: // NOTE: Scissors of Karma.
                case 5060001: // NOTE: Item Lock. 
                    {

                    }
                    break;

                case 5075000: // NOTE: Maple TV Messenger.
                case 5075003: // NOTE: Megassenger.
                    {

                    }
                    break;

                case 5075001: // NOTE: Maple TV Star Messenger.
                case 5075004: // NOTE: Star Megassenger.
                    {

                    }
                    break;

                case 5075002: // NOTE: Maple TV Heart Messenger.
                case 5075005: // NOTE: Heart Megassenger.
                    {

                    }
                    break;

                case 5200000: // NOTE: Bronze Sack of Meso.
                case 5200001: // NOTE: Silver Sack of Meso.
                case 5200002: // NOTE: Gold Sack of Meso.
                    {
                        this.Parent.Meso += item.Meso;

                        // TODO: We definitely need a GainMeso method with inChat parameter.
                        using (Packet oPacket = new Packet(ServerOperationCode.Message))
                        {
                            oPacket
                                .WriteByte((byte)MessageType.IncreaseMeso)
                                .WriteInt(item.Meso)
                                .WriteShort();

                            this.Parent.Client.Send(oPacket);
                        }

                        used = true;
                    }
                    break;

                case 5370000: // NOTE: Chalkboard.
                case 5370001: // NOTE: Chalkboard 2.
                    {
                        string text = iPacket.ReadString();

                        this.Parent.Chalkboard = text;
                    }
                    break;

                case 5300000: // NOTE: Fungus Scrol.
                case 5300001: // NOTE: Oinker Delight.
                case 5300002: // NOTE: Zeta Nightmare.
                    {

                    }
                    break;

                case 5570000: // NOTE: Vicious Hammer.
                    {

                    }
                    break;

                case 5090000: // NOTE: Note (Memo).
                    {
                        //string targetName = iPacket.ReadString();
                        //string message = iPacket.ReadString();

                        //if (this.Parent.Client.World.IsCharacterOnline(targetName))
                        //{
                        //    using (Packet oPacket = new Packet(ServerOperationCode.MemoResult))
                        //    {
                        //        oPacket
                        //            .WriteByte((byte)MemoResult.Error)
                        //            .WriteByte((byte)MemoError.ReceiverOnline);

                        //        this.Parent.Client.Send(oPacket);
                        //    }
                        //}
                        //else if (!Database.Exists("characters", "Name = {0}", targetName))
                        //{
                        //    using (Packet oPacket = new Packet(ServerOperationCode.MemoResult))
                        //    {
                        //        oPacket
                        //            .WriteByte((byte)MemoResult.Error)
                        //            .WriteByte((byte)MemoError.ReceiverInvalidName);

                        //        this.Parent.Client.Send(oPacket);
                        //    }
                        //}
                        //else if (false) // TODO: Receiver's inbox is full. I believe the maximum amount is 5, but need to verify.
                        //{
                        //    using (Packet oPacket = new Packet(ServerOperationCode.MemoResult))
                        //    {
                        //        oPacket
                        //            .WriteByte((byte)MemoResult.Error)
                        //            .WriteByte((byte)MemoError.ReceiverInboxFull);

                        //        this.Parent.Client.Send(oPacket);
                        //    }
                        //}
                        //else
                        //{
                        //    Datum datum = new Datum("memos");

                        //    datum["CharacterID"] = Database.Fetch("characters", "ID", "Name = {0}", targetName);
                        //    datum["Sender"] = this.Parent.Name;
                        //    datum["Message"] = message;
                        //    datum["Received"] = DateTime.Now;

                        //    datum.Insert();

                        //    using (Packet oPacket = new Packet(ServerOperationCode.MemoResult))
                        //    {
                        //        oPacket.WriteByte((byte)MemoResult.Sent);

                        //        this.Parent.Client.Send(oPacket);
                        //    }

                        //    used = true;
                        //}
                    }
                    break;

                case 5100000: // NOTE: Congratulatory Song.
                    {

                    }
                    break;
            }

            if (used)
            {
                this.Remove(itemID, 1);
            }
            else
            {
                this.Parent.Release(); // TODO: Blank inventory update.
            }
        }

        public void UseReturnScroll(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            short slot = iPacket.ReadShort();
            int itemID = iPacket.ReadInt();

            Item item = this[itemID, slot];

            if (item == null)
            {
                return;
            }

            this.Remove(itemID, 1);

            this.Parent.ChangeMap(item.CMoveTo);
        }

        public void Pickup(Drop drop)
        {
            if (drop.Picker == null)
            {
                try
                {
                    drop.Picker = this.Parent;

                    if (drop is Meso)
                    {
                        this.Parent.Meso += ((Meso)drop).Amount; // TODO: Check for max meso.
                    }
                    else if (drop is Item)
                    {
                        if (((Item)drop).OnlyOne)
                        {
                            // TODO: Appropriate message.

                            return;
                        }

                        ((Item)drop).Slot = this.GetNextFreeSlot(((Item)drop).Type); // TODO: Check for inv. full. 
                        this.Add((Item)drop, true);
                    }

                    this.Parent.Map.Drops.Remove(drop);

                    using (Packet oPacket = drop.GetShowGainPacket())
                    {
                        drop.Picker.Client.Send(oPacket);
                    }
                }
                catch (InventoryFullException)
                {
                    this.NotifyFull();
                }
            }
        }

        public void Pickup(Packet iPacket)
        {
            iPacket.Skip(1);
            iPacket.Skip(4);
            Point position = new Point(iPacket.ReadShort(), iPacket.ReadShort());

            // TODO: Validate position relative to the picker.

            int objectId = iPacket.ReadInt();

            lock (this.Parent.Map.Drops)
            {
                if (this.Parent.Map.Drops.Contains(objectId))
                {
                    this.Pickup(this.Parent.Map.Drops[objectId]);
                }
            }
        }

        public Item this[ItemType type, short slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Type == type && item.Slot == slot)
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public Item this[EquipmentSlot slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Slot == (sbyte)slot)
                    {
                        return item;
                    }
                }

                return null; // TODO: Should be keynotfoundexception, but I'm lazy.
            }
        }

        public Item this[int mapleId, short slot]
        {
            get
            {
                foreach (Item item in this)
                {
                    if (item.Slot == slot && item.Type == Item.GetType(mapleId))
                    {
                        return item;
                    }
                }

                return null;
            }
        }

        public IEnumerable<Item> this[ItemType type]
        {
            get
            {
                foreach (Item loopItem in this.Items)
                {
                    if (loopItem.Type == type && !loopItem.IsEquipped)
                    {
                        yield return loopItem;
                    }
                }
            }
        }

        public IEnumerable<Item> GetStored()
        {
            foreach (Item loopItem in this.Items)
            {
                if (loopItem.IsStored)
                {
                    yield return loopItem;
                }
            }
        }

        public IEnumerable<Item> GetEquipped(EquippedQueryMode mode = EquippedQueryMode.Any)
        {
            foreach (Item loopItem in this.Items)
            {
                if (loopItem.IsEquipped)
                {
                    switch (mode)
                    {
                        case EquippedQueryMode.Any:
                            yield return loopItem;
                            break;

                        case EquippedQueryMode.Normal:
                            if (loopItem.Slot > -100)
                            {
                                yield return loopItem;
                            }
                            break;

                        case EquippedQueryMode.Cash:
                            if (loopItem.Slot < -100)
                            {
                                yield return loopItem;
                            }
                            break;
                    }
                }
            }
        }

        public int SpaceTakenBy(Item item, bool autoMerge = true)
        {
            if (item.Quantity < 0)
                return 0;

            if (this.Available(item.MapleID) % item.MaxPerStack != 0 && autoMerge)
            {
                foreach (Item loopItem in this.Where(x => x.MapleID == item.MapleID && x.Quantity < x.MaxPerStack))
                {
                    if (loopItem.Quantity + item.Quantity <= loopItem.MaxPerStack)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public bool CouldReceive(IEnumerable<Item> items, bool autoMerge = true)
        {
            Dictionary<ItemType, int> spaceCount = new Dictionary<ItemType, int>(5);
            {
                spaceCount.Add(ItemType.Equipment, 0);
                spaceCount.Add(ItemType.Usable, 0);
                spaceCount.Add(ItemType.Setup, 0);
                spaceCount.Add(ItemType.Etcetera, 0);
                spaceCount.Add(ItemType.Cash, 0);
            }

            foreach (Item loopItem in items)
            {
                spaceCount[loopItem.Type] += this.SpaceTakenBy(loopItem, autoMerge);
            }

            foreach (KeyValuePair<ItemType, int> loopSpaceCount in spaceCount)
            {
                if (this.RemainingSlots(loopSpaceCount.Key) < loopSpaceCount.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public byte[] ToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteByte(this.MaxSlots[ItemType.Equipment])
                    .WriteByte(this.MaxSlots[ItemType.Usable])
                    .WriteByte(this.MaxSlots[ItemType.Setup])
                    .WriteByte(this.MaxSlots[ItemType.Etcetera])
                    .WriteByte(this.MaxSlots[ItemType.Cash])
                    .WriteLong(); // NOTE: Unknown.

                foreach (Item item in this.GetEquipped(EquippedQueryMode.Normal))
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteShort();

                foreach (Item item in this.GetEquipped(EquippedQueryMode.Cash))
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteShort();

                foreach (Item item in this[ItemType.Equipment])
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteShort();
                oPacket.WriteShort(); // TODO: Evan inventory.

                foreach (Item item in this[ItemType.Usable])
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteByte();

                foreach (Item item in this[ItemType.Setup])
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteByte();

                foreach (Item item in this[ItemType.Etcetera])
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteByte();

                foreach (Item item in this[ItemType.Cash])
                {
                    oPacket.WriteBytes(item.ToByteArray());
                }

                oPacket.WriteByte();

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Items).GetEnumerator();
        }
    }
}

