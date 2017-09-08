using Destiny.Network;
using Destiny.Maple.Characters;
using System.Collections.Generic;

namespace Destiny.Maple.Interaction
{
    public sealed class Trade
    {
        public Character Owner { get; private set; }
        public Character Visitor { get; private set; }
        public int OwnerMeso { get; private set; }
        public int VisitorMeso { get; private set; }
        public List<Item> OwnerItems { get; private set; }
        public List<Item> VisitorItems { get; private set; }
        public bool Started { get; private set; }
        public bool OwnerLocked { get; private set; }
        public bool VisitorLocked { get; private set; }

        public Trade(Character owner)
        {
            this.Owner = owner;
            this.Visitor = null;
            this.OwnerMeso = 0;
            this.VisitorMeso = 0;
            this.OwnerItems = new List<Item>();
            this.VisitorItems = new List<Item>();
            this.Started = false;
            this.OwnerLocked = false;
            this.VisitorLocked = false;

            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
            {
                oPacket
                    .WriteByte((byte)InteractionCode.Room)
                    .WriteByte(3)
                    .WriteByte(2)
                    .WriteByte(0) // NOTE: Player index.
                    .WriteByte(0)
                    .WriteBytes(this.Owner.AppearanceToByteArray())
                    .WriteString(this.Owner.Name)
                    .WriteByte(byte.MaxValue);

                this.Owner.Client.Send(oPacket);
            }
        }

        public void Handle(Character character, InteractionCode code, Packet iPacket)
        {
            switch (code)
            {
                case InteractionCode.Invite:
                    {
                        int characterID = iPacket.ReadInt();

                        if (!this.Owner.Map.Characters.Contains(characterID))
                        {
                            // TODO: What does happen in case the invitee left?

                            return;
                        }

                        Character invitee = this.Owner.Map.Characters[characterID];

                        if (invitee.Trade != null)
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                    .WriteByte((byte)InteractionCode.Decline)
                                    .WriteByte(2)
                                    .WriteString(invitee.Name);

                                this.Owner.Client.Send(oPacket);
                            }
                        }
                        else
                        {
                            invitee.Trade = this;
                            this.Visitor = invitee;

                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                    .WriteByte((byte)InteractionCode.Invite)
                                    .WriteByte(3)
                                    .WriteString(this.Owner.Name)
                                    .WriteBytes(new byte[4] { 0xB7, 0x50, 0x00, 0x00 });

                                this.Visitor.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case InteractionCode.Decline:
                    {
                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.Decline)
                                .WriteByte(3)
                                .WriteString(character.Name);

                            this.Owner.Client.Send(oPacket);
                        }

                        this.Owner.Trade = null;
                        this.Visitor.Trade = null;
                        this.Owner = null;
                        this.Visitor = null;
                    }
                    break;

                case InteractionCode.Visit:
                    {
                        if (this.Owner == null)
                        {
                            this.Visitor = null;
                            character.Trade = null;

                            character.Notify("Trade has been closed.", NoticeType.Popup);
                        }
                        else
                        {
                            this.Started = true;

                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                    .WriteByte((byte)InteractionCode.Visit)
                                    .WriteByte(1)
                                    .WriteBytes(this.Visitor.AppearanceToByteArray())
                                    .WriteString(this.Visitor.Name);

                                this.Owner.Client.Send(oPacket);
                            }

                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                    .WriteByte((byte)InteractionCode.Room)
                                    .WriteByte(3)
                                    .WriteByte(2)
                                    .WriteByte(1)
                                    .WriteByte(0)
                                    .WriteBytes(this.Owner.AppearanceToByteArray())
                                    .WriteString(this.Owner.Name)
                                    .WriteByte(1)
                                    .WriteBytes(this.Visitor.AppearanceToByteArray())
                                    .WriteString(this.Visitor.Name)
                                    .WriteByte(byte.MaxValue);

                                this.Visitor.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case InteractionCode.SetItems:
                    {
                        ItemType type = (ItemType)iPacket.ReadByte();
                        short slot = iPacket.ReadShort();
                        short quantity = iPacket.ReadShort();
                        byte targetSlot = iPacket.ReadByte();

                        Item item = character.Items[type, slot];

                        if (item.IsBlocked)
                        {
                            return;
                        }

                        if (quantity > item.Quantity)
                        {
                            return;
                        }

                        if (quantity < item.Quantity)
                        {
                            item.Quantity -= quantity;
                            item.Update();

                            item = new Item(item.MapleID, quantity);
                        }
                        else
                        {
                            character.Items.Remove(item, true);
                        }

                        item.Slot = 0;

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.SetItems)
                                .WriteByte(0)
                                .WriteByte(targetSlot)
                                .WriteBytes(item.ToByteArray(true));

                            if (character == this.Owner)
                            {
                                this.OwnerItems.Add(item);

                                this.Owner.Client.Send(oPacket);
                            }
                            else
                            {
                                this.VisitorItems.Add(item);

                                this.Visitor.Client.Send(oPacket);
                            }
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.SetItems)
                                .WriteByte(1)
                                .WriteByte(targetSlot)
                                .WriteBytes(item.ToByteArray(true));

                            if (character == this.Owner)
                            {
                                this.Visitor.Client.Send(oPacket);
                            }
                            else
                            {
                                this.Owner.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case InteractionCode.SetMeso:
                    {
                        int meso = iPacket.ReadInt();

                        if (meso < 0 || meso > character.Meso)
                        {
                            return;
                        }

                        // TODO: The meso written in this packet is the added meso amount.
                        // The amount that should be written is the total amount.

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.SetMeso)
                                .WriteByte(0)
                                .WriteInt(meso);

                            if (character == this.Owner)
                            {
                                if (this.OwnerLocked)
                                {
                                    return;
                                }

                                this.OwnerMeso += meso;
                                this.Owner.Meso -= meso;

                                this.Owner.Client.Send(oPacket);
                            }
                            else
                            {
                                if (this.VisitorLocked)
                                {
                                    return;
                                }

                                this.VisitorMeso += meso;
                                this.Visitor.Meso -= meso;

                                this.Visitor.Client.Send(oPacket);
                            }
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.SetMeso)
                                .WriteByte(1)
                                .WriteInt(meso);

                            if (this.Owner == character)
                            {
                                this.Visitor.Client.Send(oPacket);
                            }
                            else
                            {
                                oPacket.WriteInt(this.OwnerMeso);

                                this.Owner.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case InteractionCode.Exit:
                    {
                        if (this.Started)
                        {
                            this.Cancel();

                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                   .WriteByte((byte)InteractionCode.Exit)
                                   .WriteByte(0)
                                   .WriteByte(2);

                                this.Owner.Client.Send(oPacket);
                                this.Visitor.Client.Send(oPacket);
                            }

                            this.Owner.Trade = null;
                            this.Visitor.Trade = null;
                            this.Owner = null;
                            this.Visitor = null;
                        }
                        else
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                   .WriteByte((byte)InteractionCode.Exit)
                                    .WriteByte(0)
                                    .WriteByte(2);

                                this.Owner.Client.Send(oPacket);
                            }

                            this.Owner.Trade = null;
                            this.Owner = null;
                        }
                    }
                    break;

                case InteractionCode.Confirm:
                    {
                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket.WriteByte((byte)InteractionCode.Confirm);

                            if (character == this.Owner)
                            {
                                this.OwnerLocked = true;

                                this.Visitor.Client.Send(oPacket);
                            }
                            else
                            {
                                this.VisitorLocked = true;

                                this.Owner.Client.Send(oPacket);
                            }
                        }

                        if (this.OwnerLocked && this.VisitorLocked)
                        {
                            this.Complete();

                            using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                            {
                                oPacket
                                    .WriteByte((byte)InteractionCode.Exit)
                                    .WriteByte(0)
                                    .WriteByte(6);

                                this.Owner.Client.Send(oPacket);
                                this.Visitor.Client.Send(oPacket);
                            }

                            this.Owner.Trade = null;
                            this.Visitor.Trade = null;
                            this.Owner = null;
                            this.Visitor = null;
                        }
                    }
                    break;

                case InteractionCode.Chat:
                    {
                        string text = iPacket.ReadString();

                        using (Packet oPacket = new Packet(ServerOperationCode.PlayerInteraction))
                        {
                            oPacket
                                .WriteByte((byte)InteractionCode.Chat)
                                .WriteByte(8)
                                .WriteBool(this.Owner != character)
                                .WriteString("{0} : {1}", character.Name, text);

                            this.Owner.Client.Send(oPacket);
                            this.Visitor.Client.Send(oPacket);
                        }
                    }
                    break;
            }
        }

        public void Complete()
        {
            if (this.Owner.Items.CouldReceive(this.VisitorItems) && this.Visitor.Items.CouldReceive(this.OwnerItems))
            {
                this.Owner.Meso += this.VisitorMeso;
                this.Visitor.Meso += this.OwnerMeso;

                this.Owner.Items.AddRange(this.VisitorItems);
                this.Visitor.Items.AddRange(this.OwnerItems);
            }
            else
            {
                // TODO: Cancel trade.
            }
        }

        public void Cancel()
        {
            this.Owner.Meso += this.OwnerMeso;
            this.Visitor.Meso += this.VisitorMeso;

            this.Owner.Items.AddRange(this.OwnerItems);
            this.Visitor.Items.AddRange(this.VisitorItems);
        }
    }
}
