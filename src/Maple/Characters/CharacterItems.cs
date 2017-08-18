using Destiny.Maple.Maps;
using System.Collections.Generic;
using System;
using System.Collections;
using Destiny.Core.IO;
using Destiny.Data;
using Destiny.Core.Network;
using System.Linq;
using Destiny.Network;

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
            foreach (Datum datum in new Datums("items").Populate("CharacterID = {0} AND IsStored = False", this.Parent.ID))
            {
                Item item = new Item(datum);

                this.Add(item);

                if (item.PetID != 0)
                {
                    this.Parent.Pets.Add(new Pet(item));
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

        public void Delete()
        {
            foreach (Item item in this)
            {
                item.Delete();
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
                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.InventoryOperation))
                    {
                        oPacket
                            .WriteBool(fromDrop)
                            .WriteByte(1)
                            .WriteByte((byte)InventoryOperationType.AddItem)
                            .WriteByte((byte)item.Type)
                            .WriteShort(item.Slot);

                        item.Encode(oPacket, true);

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
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.InventoryOperation))
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

        public void Sort(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            ItemType type = (ItemType)iPacket.ReadByte();
        }

        public void Gather(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            ItemType type = (ItemType)iPacket.ReadByte();
        }

        public void Handle(InPacket iPacket)
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

        public void UseCashItem(InPacket iPacket)
        {
            short slot = iPacket.ReadShort();
            int itemID = iPacket.ReadInt();

            Item item = this[itemID, slot];

            if (item == null)
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

                        string text = iPacket.ReadMapleString();

                        string message = string.Format($"{this.Parent.Name} : {text}"); // TODO: Include medal name.

                        // NOTE: In GMS, this sends to everyone on the current channel, not the map (despite the item's description).
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.Megaphone)
                                .WriteMapleString(message);

                            foreach (var character in MasterServer.OnlineCharacters.Where(c => c.Value.Client.Character == this.Parent.Client.Character).Select((c) => c.Value))
                            {
                                character.Client.Send(oPacket);
                            }
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

                        string text = iPacket.ReadMapleString();
                        bool whisper = iPacket.ReadBool();

                        string message = string.Format($"{this.Parent.Name} : {text}"); // TODO: Include medal name.

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.SuperMegaphone)
                                .WriteMapleString(message)
                                .WriteByte(this.Parent.Client.Channel)
                                .WriteBool(whisper);

                            foreach (Character character in MasterServer.OnlineCharacters.Select((c) => c.Value))
                            {
                                character.Client.Send(oPacket);
                            }
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

                        string text1 = iPacket.ReadMapleString();
                        string text2 = iPacket.ReadMapleString();
                        string text3 = iPacket.ReadMapleString();
                        string text4 = iPacket.ReadMapleString();
                        bool whisper = iPacket.ReadBool();

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.SetAvatarMegaphone))
                        {
                            oPacket
                                .WriteInt(itemID)
                                .WriteMapleString(this.Parent.Name)
                                .WriteMapleString(text1)
                                .WriteMapleString(text2)
                                .WriteMapleString(text3)
                                .WriteMapleString(text4)
                                .WriteInt(this.Parent.Client.Channel)
                                .WriteBool(whisper);

                            this.Parent.EncodeApperance(oPacket, true);

                            foreach (Character character in MasterServer.OnlineCharacters.Select((c) => c.Value))
                            {
                                character.Client.Send(oPacket);
                            }
                        }

                        used = true;
                    }
                    break;

                case 5076000: // NOTE: Item Megaphone.
                    {
                        string text = iPacket.ReadMapleString();
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

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.BroadcastMsg))
                        {
                            oPacket
                                .WriteByte((byte)NoticeType.ItemMegaphone)
                                .WriteMapleString(message)
                                .WriteByte(this.Parent.Client.Channel)
                                .WriteBool(whisper)
                                .WriteByte((byte)(targetItem != null ? targetItem.Slot : 0));

                            if (targetItem != null)
                            {
                                targetItem.Encode(oPacket, true, true);
                            }

                            foreach (Character character in MasterServer.OnlineCharacters.Select((c) => c.Value))
                            {
                                character.Client.Send(oPacket);
                            }
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
                        // TODO: Get the summoned pet.

                        string name = iPacket.ReadMapleString();

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.PetNameChanged))
                        {
                            oPacket
                                .WriteInt(this.Parent.ID)
                                .WriteByte() // NOTE: Index.
                                .WriteMapleString(name)
                                .WriteByte();

                            this.Parent.Map.Broadcast(oPacket);
                        }
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
                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Message))
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
                        string text = iPacket.ReadMapleString();

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
                        string targetName = iPacket.ReadMapleString();
                        string message = iPacket.ReadMapleString();

                        if (MasterServer.OnlineCharacters.ContainsKey(targetName))
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MemoResult))
                            {
                                oPacket
                                    .WriteByte((byte)MemoResult.Error)
                                    .WriteByte((byte)MemoError.ReceiverOnline);

                                this.Parent.Client.Send(oPacket);
                            }
                        }
                        else if (!Database.Exists("characters", "Name = {0}", targetName))
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MemoResult))
                            {
                                oPacket
                                    .WriteByte((byte)MemoResult.Error)
                                    .WriteByte((byte)MemoError.ReceiverInvalidName);

                                this.Parent.Client.Send(oPacket);
                            }
                        }
                        else if (false) // TODO: Receiver's inbox is full. I believe the maximum amount is 5, but need to verify.
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MemoResult))
                            {
                                oPacket
                                    .WriteByte((byte)MemoResult.Error)
                                    .WriteByte((byte)MemoError.ReceiverInboxFull);

                                this.Parent.Client.Send(oPacket);
                            }
                        }
                        else
                        {
                            Datum datum = new Datum("memos");

                            datum["CharacterID"] = Database.Fetch("characters", "ID", "Name = {0}", targetName);
                            datum["Sender"] = this.Parent.Name;
                            datum["Message"] = message;
                            datum["Received"] = DateTime.Now;

                            datum.Insert();

                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MemoResult))
                            {
                                oPacket.WriteByte((byte)MemoResult.Sent);

                                this.Parent.Client.Send(oPacket);
                            }

                            used = true;
                        }
                    }
                    break;

                case 5100000: // NOTE: Congratulatory Song.
                    {

                    }
                    break;
            }

            if (used)
            {
                this.Remove(item, true);
            }
            else
            {
                // TODO: Blank inventory update.
            }
        }

        public void UseReturnScroll(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            short slot = iPacket.ReadShort();
            int mapleID = iPacket.ReadInt();

            Item item = this[mapleID, slot];

            if (item == null)
            {
                return;
            }

            this.Remove(item, true);

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
                        ((Item)drop).Slot = this.GetNextFreeSlot(((Item)drop).Type); // TODO: Check for inv. full. 
                        this.Add((Item)drop, true);
                    }

                    this.Parent.Map.Drops.Remove(drop);

                    using (OutPacket oPacket = drop.GetShowGainPacket())
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

        public void Pickup(InPacket iPacket)
        {
            iPacket.Skip(1);
            iPacket.Skip(4);
            Point position = iPacket.ReadPoint();

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

        public void Encode(OutPacket oPacket)
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
                item.Encode(oPacket);
            }

            oPacket.WriteShort();

            foreach (Item item in this.GetEquipped(EquippedQueryMode.Cash))
            {
                item.Encode(oPacket);
            }

            oPacket.WriteShort();

            foreach (Item item in this[ItemType.Equipment])
            {
                item.Encode(oPacket);
            }

            oPacket.WriteShort();
            oPacket.WriteShort(); // TODO: Evan inventory.

            foreach (Item item in this[ItemType.Usable])
            {
                item.Encode(oPacket);
            }

            oPacket.WriteByte();

            foreach (Item item in this[ItemType.Setup])
            {
                item.Encode(oPacket);
            }

            oPacket.WriteByte();

            foreach (Item item in this[ItemType.Etcetera])
            {
                item.Encode(oPacket);
            }

            oPacket.WriteByte();

            foreach (Item item in this[ItemType.Cash])
            {
                item.Encode(oPacket);
            }

            oPacket.WriteByte();
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
