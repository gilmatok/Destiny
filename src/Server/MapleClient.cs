using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Core.Security;
using Destiny.Core.Data;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Commands.Implementation;
using Destiny.Maple.Data;
using Destiny.Maple.Social;
using Destiny.Server;
using Destiny.Server.Migration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Destiny.Server
{
    public sealed class MapleClient : Session
    {
        private ServerBase mParentServer;

        public byte WorldID { get; set; }
        public byte ChannelID { get; set; }

        public Account Account { get; set; }
        public Character Character { get; set; }

        public string LastUsername { get; private set; }
        public string LastPassword { get; private set; }

        public bool IsInViewAllChar { get; set; }

        public WorldServer World
        {
            get
            {
                return MasterServer.Worlds[this.WorldID];
            }
        }

        public ChannelServer Channel
        {
            get
            {
                return MasterServer.Worlds[this.WorldID][this.ChannelID];
            }
        }

        public MapleClient(Socket socket, ServerBase parentServer)
            : base(socket)
        {
            mParentServer = parentServer;

            mParentServer.AddClient(this);
        }

        protected override void Terminate()
        {
            if (this.Character != null)
            {
                this.Character.Save();
                this.Character.LastNpc = null;
                this.Character.Map.Characters.Remove(this.Character);

                if (this.Character.Messenger != null)
                {
                    this.Character.Messenger.RemoveParticipant(this.Character);
                }

                if (this.Character.Party != null)
                {
                    this.Character.Party[this.Character.ID].Character = null;
                }

                if (this.Character.Guild != null)
                {
                    this.Character.Guild[this.Character.ID].Character = null;
                }

                if (this.Character.Trade != null)
                {
                    this.Character.Trade.Cancel();
                }

                if (this.Character.PlayerShop != null)
                {
                    this.Character.PlayerShop.Close();
                }

                this.Channel.Characters.Unregister(this.Character);
            }

            mParentServer.RemoveClient(this);
        }

        protected override void Dispatch(InPacket iPacket)
        {
            switch (iPacket.OperationCode)
            {
                case ClientOperationCode.AccountLogin:
                    this.Login(iPacket);
                    break;

                case ClientOperationCode.EULA:
                    this.EULA(iPacket);
                    break;

                case ClientOperationCode.AccountGender:
                    this.SetGender(iPacket);
                    break;

                case ClientOperationCode.PinCheck:
                    this.CheckPin(iPacket);
                    break;

                case ClientOperationCode.PinUpdate:
                    this.UpdatePin(iPacket);
                    break;

                case ClientOperationCode.WorldList:
                case ClientOperationCode.WorldRelist:
                    this.ListWorlds();
                    break;

                case ClientOperationCode.WorldStatus:
                    this.InformWorldStatus(iPacket);
                    break;

                case ClientOperationCode.WorldSelect:
                    this.SelectWorld(iPacket);
                    break;

                case ClientOperationCode.ViewAllChar:
                    this.ViewAllChar(iPacket);
                    break;

                case ClientOperationCode.VACFlagSet:
                    this.SetViewAllChar(iPacket);
                    break;

                case ClientOperationCode.CharacterNameCheck:
                    this.CheckCharacterName(iPacket);
                    break;

                case ClientOperationCode.CharacterCreate:
                    this.CreateCharacter(iPacket);
                    break;

                case ClientOperationCode.CharacterDelete:
                    this.DeleteCharacter(iPacket);
                    break;

                case ClientOperationCode.CharacterSelect:
                case ClientOperationCode.SelectCharacterByVAC:
                    this.SelectCharacter(iPacket);
                    break;

                case ClientOperationCode.CharacterSelectRegisterPic:
                case ClientOperationCode.RegisterPicFromVAC:
                    this.SelectCharacter(iPacket, registerPic: true);
                    break;

                case ClientOperationCode.CharacterSelectRequestPic:
                case ClientOperationCode.RequestPicFromVAC:
                    this.SelectCharacter(iPacket, requestPic: true);
                    break;

                case ClientOperationCode.CharacterLoad:
                    this.LoadCharacter(iPacket);
                    break;

                case ClientOperationCode.MapChange:
                    this.Character.ChangeMap(iPacket);
                    break;

                case ClientOperationCode.ChannelChange:
                    this.ChangeChannel(iPacket);
                    break;

                case ClientOperationCode.PlayerMovement:
                    this.Character.Move(iPacket);
                    break;

                case ClientOperationCode.Sit:
                    this.Character.Sit(iPacket);
                    break;

                case ClientOperationCode.UseChair:
                    this.Character.SitChair(iPacket);
                    break;

                case ClientOperationCode.CloseRangeAttack:
                    this.Character.Attack(iPacket, AttackType.Melee);
                    break;

                case ClientOperationCode.RangedAttack:
                    this.Character.Attack(iPacket, AttackType.Range);
                    break;

                case ClientOperationCode.MagicAttack:
                    this.Character.Attack(iPacket, AttackType.Magic);
                    break;

                case ClientOperationCode.TakeDamage:
                    this.Character.Damage(iPacket);
                    break;

                case ClientOperationCode.PlayerChat:
                    this.Character.Talk(iPacket);
                    break;

                case ClientOperationCode.CloseChalkboard:
                    this.Character.Chalkboard = string.Empty;
                    break;

                case ClientOperationCode.FaceExpression:
                    this.Character.Express(iPacket);
                    break;

                case ClientOperationCode.NpcConverse:
                    this.Character.Converse(iPacket);
                    break;

                case ClientOperationCode.NpcResult:
                    this.Character.LastNpc.Handle(this.Character, iPacket);
                    break;

                case ClientOperationCode.NpcShop:
                    this.Character.LastNpc.Shop.Handle(this.Character, iPacket);
                    break;

                case ClientOperationCode.Storage:
                    this.Character.Storage.Handle(iPacket);
                    break;

                case ClientOperationCode.AdminShopAction:
                    {
                        AdminShopAction action = (AdminShopAction)iPacket.ReadByte();

                        switch (action)
                        {
                            case AdminShopAction.Buy:
                                {
                                    int commodityID = iPacket.ReadInt();
                                    int quantity = iPacket.ReadInt();

                                    var adminShopItem = AdminShopCommand.Items[commodityID];

                                    this.Character.Items.Add(new Item(adminShopItem.Item2, (short)quantity));

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.AdminShopMessage))
                                    {
                                        oPacket.WriteByte();

                                        this.Send(oPacket);
                                    }
                                }
                                break;

                            case AdminShopAction.Exit:
                                break;

                            case AdminShopAction.Register:
                                {
                                    int itemID = iPacket.ReadInt();
                                }
                                break;
                        }
                    }
                    break;

                case ClientOperationCode.InventorySort:
                    this.Character.Items.Sort(iPacket);
                    break;

                case ClientOperationCode.InventoryGather:
                    this.Character.Items.Gather(iPacket);
                    break;

                case ClientOperationCode.InventoryAction:
                    this.Character.Items.Handle(iPacket);
                    break;

                case ClientOperationCode.UseItem:
                    this.Character.Items.UseItem(iPacket);
                    break;

                case ClientOperationCode.UseSummonBag:
                    this.Character.Items.UseSummonBag(iPacket);
                    break;

                case ClientOperationCode.UseCashItem:
                    this.Character.Items.UseCashItem(iPacket);
                    break;

                case ClientOperationCode.UseTeleportRock: // NOTE: Only occurs with the special Teleport Rock in the usable inventory.
                    this.Character.Trocks.Use(2320000, iPacket);
                    break;

                case ClientOperationCode.UseReturnScroll:
                    this.Character.Items.UseReturnScroll(iPacket);
                    break;

                case ClientOperationCode.DistributeAP:
                    this.Character.DistributeAP(iPacket);
                    break;

                case ClientOperationCode.AutoDistributeAP:
                    this.Character.AutoDistributeAP(iPacket);
                    break;

                case ClientOperationCode.HealOverTime:
                    this.Character.HealOverTime(iPacket);
                    break;

                case ClientOperationCode.DistributeSP:
                    this.Character.DistributeSP(iPacket);
                    break;

                case ClientOperationCode.UseSkill:
                    this.Character.Skills.Cast(iPacket);
                    break;

                case ClientOperationCode.CancelBuff:
                    this.Character.Buffs.Cancel(iPacket);
                    break;

                case ClientOperationCode.MesoDrop:
                    this.Character.DropMeso(iPacket);
                    break;

                case ClientOperationCode.PlayerInformation:
                    this.Character.InformOnCharacter(iPacket);
                    break;

                case ClientOperationCode.SpawnPet:
                    //this.Character.Pets.Spawn(iPacket);
                    break;

                case ClientOperationCode.ChangeMapSpecial:
                    this.Character.EnterPortal(iPacket);
                    break;

                case ClientOperationCode.TrockAction:
                    this.Character.Trocks.Update(iPacket);
                    break;

                case ClientOperationCode.Report:
                    this.Character.Report(iPacket);
                    break;

                case ClientOperationCode.QuestAction:
                    this.Character.Quests.Handle(iPacket);
                    break;

                case ClientOperationCode.MultiChat:
                    this.Character.MultiTalk(iPacket);
                    break;

                case ClientOperationCode.Command:
                    this.Character.UseCommand(iPacket);
                    break;

                // TODO: Move else-where.
                case ClientOperationCode.Messenger:
                    {
                        MessengerAction action = (MessengerAction)iPacket.ReadByte();

                        switch (action)
                        {
                            case MessengerAction.Open:
                                {
                                    int messengerID = iPacket.ReadInt();

                                    if (messengerID == 0)
                                    {
                                        this.World.CreateMessenger(this.Character);
                                    }
                                    else
                                    {
                                        Messenger messenger = this.World.GetMessenger(messengerID);

                                        if (messenger == null || messenger.IsFull)
                                        {
                                            // NOTE: I'm pretty sure Nexon doesn't send a message in this case.

                                            return;
                                        }

                                        messenger.AddParticipant(this.Character);
                                    }
                                }
                                break;

                            case MessengerAction.Leave:
                                {
                                    if (this.Character.Messenger == null)
                                    {
                                        return;
                                    }

                                    this.Character.Messenger.RemoveParticipant(this.Character);
                                }
                                break;

                            case MessengerAction.Invite:
                                {
                                    if (this.Character.Messenger == null)
                                    {
                                        return;
                                    }

                                    string targetName = iPacket.ReadMapleString();

                                    Character target = this.World.GetCharacter(targetName);

                                    if (target == null)
                                    {
                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                                        {
                                            oPacket
                                                .WriteByte((byte)MessengerResult.Note)
                                                .WriteMapleString(targetName)
                                                .WriteBool(false);

                                            this.Send(oPacket);
                                        }
                                    }
                                    else if (target.Messenger != null)
                                    {
                                        // TODO: What message do we send? 
                                    }
                                    else
                                    {
                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                                        {
                                            oPacket
                                                .WriteByte((byte)MessengerResult.Invite)
                                                .WriteMapleString(this.Character.Name)
                                                .WriteByte()
                                                .WriteInt(this.Character.Messenger.ID)
                                                .WriteByte();

                                            target.Client.Send(oPacket);
                                        }

                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                                        {
                                            oPacket
                                                .WriteByte((byte)MessengerResult.Note)
                                                .WriteMapleString(targetName)
                                                .WriteBool(true);

                                            this.Send(oPacket);
                                        }
                                    }
                                }
                                break;

                            case MessengerAction.Decline:
                                {
                                    // TODO: Validate inivitation.

                                    string inviterName = iPacket.ReadMapleString();

                                    Character target = this.World.GetCharacter(inviterName);

                                    if (target == null)
                                    {
                                        return;
                                    }

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                                    {
                                        oPacket
                                            .WriteByte((byte)MessengerResult.Decline)
                                            .WriteMapleString(this.Character.Name)
                                            .WriteByte();

                                        target.Client.Send(oPacket);
                                    }
                                }
                                break;

                            case MessengerAction.Chat:
                                {
                                    if (this.Character.Messenger == null)
                                    {
                                        return;
                                    }

                                    string text = iPacket.ReadMapleString();

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.Messenger))
                                    {
                                        oPacket
                                            .WriteByte((byte)MessengerResult.Chat)
                                            .WriteMapleString(text);

                                        this.Character.Messenger.Broadcast(oPacket, this.Character);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case ClientOperationCode.PlayerInteraction:
                    this.Character.Interact(iPacket);
                    break;

                // TODO: Move else-where.
                case ClientOperationCode.PartyOperation:
                    {
                        PartyAction action = (PartyAction)iPacket.ReadByte();

                        switch (action)
                        {
                            case PartyAction.Create:
                                {
                                    if (this.Character.Party != null)
                                    {
                                        return;
                                    }

                                    this.World.CreateParty(this.Character);
                                }
                                break;

                            case PartyAction.Leave:
                                {
                                    if (this.Character.Party == null)
                                    {
                                        return;
                                    }

                                    if (this.Character.Party.LeaderID == this.Character.ID)
                                    {
                                        this.Character.Party.Disband();
                                    }
                                    else
                                    {
                                        this.Character.Party.Remove(this.Character.ID);
                                    }
                                }
                                break;

                            case PartyAction.Join:
                                {
                                    if (this.Character.Party != null)
                                    {
                                        return;
                                    }

                                    int partyID = iPacket.ReadInt();

                                    // TODO: Validate invitation.

                                    Party party = this.World.GetParty(partyID);

                                    if (party == null)
                                    {
                                        return;
                                    }

                                    if (party.IsFull)
                                    {
                                        return;
                                    }

                                    party.Add(new PartyMember(this.Character));
                                }
                                break;

                            case PartyAction.Invite:
                                {
                                    if (this.Character.Party == null)
                                    {
                                        return;
                                    }

                                    if (this.Character.Party.LeaderID != this.Character.ID)
                                    {
                                        return;
                                    }

                                    string targetName = iPacket.ReadMapleString();

                                    Character target = this.World.GetCharacter(targetName);

                                    // TODO: Check if target is taking care of another inivitation.

                                    if (target.Party != null)
                                    {
                                        return;
                                    }

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.PartyResult))
                                    {
                                        oPacket
                                            .WriteByte((byte)PartyResult.Invite)
                                            .WriteInt(this.Character.Party.ID)
                                            .WriteMapleString(this.Character.Name)
                                            .WriteByte();

                                        target.Client.Send(oPacket);
                                    }
                                }
                                break;

                            case PartyAction.Expel:
                                {
                                    if (this.Character.Party == null)
                                    {
                                        return;
                                    }

                                    if (this.Character.Party.LeaderID != this.Character.ID)
                                    {
                                        return;
                                    }

                                    int memberID = iPacket.ReadInt();

                                    // TODO: Validate member ID.

                                    PartyMember member = this.Character.Party[memberID];

                                    member.Expelled = true;

                                    this.Character.Party.Remove(memberID);
                                }
                                break;

                            case PartyAction.ChangeLeader:
                                {

                                }
                                break;
                        }
                    }
                    break;

                case ClientOperationCode.DenyPartyRequest:
                    {

                    }
                    break;

                // TODO: Move else-where.
                case ClientOperationCode.GuildOperation:
                    {
                        GuildAction action = (GuildAction)iPacket.ReadByte();

                        switch (action)
                        {
                            case GuildAction.Create:
                                {
                                    if (this.Character.Guild != null || this.Character.Map.MapleID != 200000301)
                                    {
                                        return;
                                    }

                                    if (this.Character.Meso < 1500000) // TODO: Make this a constant.
                                    {
                                        this.Character.Notify("You do not have enough mesos to create a Guild.");

                                        return;
                                    }

                                    string guildName = iPacket.ReadMapleString();

                                    if (!guildName.IsAlphaNumeric() || guildName.Length < 3 || guildName.Length > 12) // TODO: Better name checks.
                                    {
                                        this.Character.Notify("The Guild name you have chosen is not accepted.");

                                        return;
                                    }

                                    this.World.CreateGuild(this.Character, guildName);

                                    this.Character.Meso -= 1500000;

                                    this.Character.Notify("You have successfully created a Guild");
                                }
                                break;

                            case GuildAction.Invite:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank > 2)
                                    {
                                        return;
                                    }

                                    string targetName = iPacket.ReadMapleString();

                                    Character target = this.Channel.Characters.GetCharacter(targetName);

                                    if (target == null || target.Guild != null)
                                    {
                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                        {
                                            oPacket.WriteByte((byte)GuildResult.InviteeNotInChannel);

                                            this.Send(oPacket);
                                        }
                                    }
                                    else if (target.Guild != null)
                                    {
                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                        {
                                            oPacket.WriteByte((byte)GuildResult.InviteeAlreadyInGuild);

                                            this.Send(oPacket);
                                        }
                                    }
                                    else
                                    {
                                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                        {
                                            oPacket
                                                .WriteByte((byte)GuildResult.Invite)
                                                .WriteInt(this.Character.Guild.ID)
                                                .WriteMapleString(this.Character.Name);

                                            target.Client.Send(oPacket);
                                        }
                                    }
                                }
                                break;

                            case GuildAction.Join:
                                {
                                    if (this.Character.Guild != null)
                                    {
                                        return;
                                    }

                                    int guildID = iPacket.ReadInt();

                                    // TODO: Validate initivation.

                                    Guild guild = this.World.GetGuild(guildID);

                                    if (guild == null)
                                    {
                                        return;
                                    }

                                    if (guild.IsFull)
                                    {
                                        return;
                                    }

                                    guild.Add(new GuildMember(this.Character));
                                }
                                break;

                            case GuildAction.Leave:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank == 1)
                                    {
                                        return;
                                    }

                                    this.Character.Guild.Remove(this.Character.ID);
                                }
                                break;

                            case GuildAction.Expel:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank > 2)
                                    {
                                        return;
                                    }

                                    int targetID = iPacket.ReadInt();
                                    string targetName = iPacket.ReadMapleString();

                                    if (targetID == this.Character.ID)
                                    {
                                        return;
                                    }

                                    GuildMember member = this.Character.Guild[targetID];

                                    member.Expelled = true;
                                    member.Expeller = this.Character.Name;

                                    this.Character.Guild.Remove(member);
                                }
                                break;

                            case GuildAction.ModifyTitles:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank != 1)
                                    {
                                        return;
                                    }

                                    this.Character.Guild.Rank1 = iPacket.ReadMapleString();
                                    this.Character.Guild.Rank2 = iPacket.ReadMapleString();
                                    this.Character.Guild.Rank3 = iPacket.ReadMapleString();
                                    this.Character.Guild.Rank4 = iPacket.ReadMapleString();
                                    this.Character.Guild.Rank5 = iPacket.ReadMapleString();

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                    {
                                        oPacket
                                            .WriteByte((byte)GuildResult.UpdateRanks)
                                            .WriteInt(this.Character.Guild.ID)
                                            .WriteMapleString(this.Character.Guild.Rank1)
                                            .WriteMapleString(this.Character.Guild.Rank2)
                                            .WriteMapleString(this.Character.Guild.Rank3)
                                            .WriteMapleString(this.Character.Guild.Rank4)
                                            .WriteMapleString(this.Character.Guild.Rank5);

                                        this.Character.Guild.Broadcast(oPacket);
                                    }
                                }
                                break;

                            case GuildAction.ModifyRank:
                                {
                                    int targetID = iPacket.ReadInt();
                                    byte newRank = iPacket.ReadByte();

                                    if (this.Character.Guild == null || this.Character.GuildRank > 2 || (newRank == 2 && this.Character.GuildRank != 1) || newRank < 2 || newRank > 5)
                                    {
                                        return;
                                    }

                                    GuildMember member = this.Character.Guild[targetID];

                                    member.Rank = newRank;

                                    if (member.Character != null)
                                    {
                                        member.Character.GuildRank = newRank;
                                    }

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                    {
                                        oPacket
                                            .WriteByte((byte)GuildResult.ChangeRank)
                                            .WriteInt(this.Character.Guild.ID)
                                            .WriteInt(targetID)
                                            .WriteByte(newRank);

                                        this.Character.Guild.Broadcast(oPacket);
                                    }
                                }
                                break;

                            case GuildAction.ModifyEmblem:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank != 1)
                                    {
                                        return;
                                    }

                                    short background = iPacket.ReadShort();
                                    byte backgroundColor = iPacket.ReadByte();
                                    short logo = iPacket.ReadShort();
                                    byte logoColor = iPacket.ReadByte();

                                    // TODO: Validate values.

                                    this.Character.Guild.Background = background;
                                    this.Character.Guild.BackgroundColor = backgroundColor;
                                    this.Character.Guild.Logo = logo;
                                    this.Character.Guild.LogoColor = logoColor;

                                    // TODO: Do we save or let the world take care of saving?

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                    {
                                        oPacket
                                            .WriteByte((byte)GuildResult.ShowEmblem)
                                            .WriteInt(this.Character.Guild.ID)
                                            .WriteShort(this.Character.Guild.Background)
                                            .WriteByte(this.Character.Guild.BackgroundColor)
                                            .WriteShort(this.Character.Guild.Logo)
                                            .WriteByte(this.Character.Guild.LogoColor);

                                        this.Character.Guild.Broadcast(oPacket);
                                    }

                                    foreach (GuildMember member in this.Character.Guild)
                                    {
                                        if (member.Character != null)
                                        {
                                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildMarkChanged))
                                            {
                                                oPacket
                                                    .WriteInt(member.Character.ID)
                                                    .WriteShort(this.Character.Guild.Background)
                                                    .WriteByte(this.Character.Guild.BackgroundColor)
                                                    .WriteShort(this.Character.Guild.Logo)
                                                    .WriteByte(this.Character.Guild.LogoColor);

                                                member.Character.Map.Broadcast(oPacket, member.Character);
                                            }
                                        }
                                    }
                                }
                                break;

                            case GuildAction.ModifyNotice:
                                {
                                    if (this.Character.Guild == null || this.Character.GuildRank > 2)
                                    {
                                        return;
                                    }

                                    this.Character.Guild.Notice = iPacket.ReadMapleString();

                                    // TODO: Do we save or let the world take care of saving?

                                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                    {
                                        oPacket
                                            .WriteByte((byte)GuildResult.UpdateNotice)
                                            .WriteInt(this.Character.Guild.ID)
                                            .WriteMapleString(this.Character.Guild.Notice);

                                        this.Character.Guild.Broadcast(oPacket);
                                    }
                                }
                                break;
                        }
                    }
                    break;

                case ClientOperationCode.DenyGuildRequest:
                    {

                    }
                    break;

                case ClientOperationCode.AdminCommand:
                    this.Character.UseAdminCommand(iPacket);
                    break;

                case ClientOperationCode.NoteAction:
                    this.Character.Memos.Handle(iPacket);
                    break;

                case ClientOperationCode.ChangeKeymap:
                    this.Character.Keymap.Change(iPacket);
                    break;

                // TODO: Move else-where.
                case ClientOperationCode.BbsOperation:
                    {
                        BbsAction action = (BbsAction)iPacket.ReadByte();

                        switch (action)
                        {
                            case BbsAction.List:
                                {
                                    int page = iPacket.ReadInt();
                                }
                                break;
                        }
                    }
                    break;

                case ClientOperationCode.MovePet:
                    //this.Character.Pets.Move(iPacket);
                    break;

                case ClientOperationCode.MobMovement:
                    this.Character.ControlledMobs.Move(iPacket);
                    break;

                case ClientOperationCode.DropPickup:
                    this.Character.Items.Pickup(iPacket);
                    break;

                case ClientOperationCode.NpcMovement:
                    this.Character.ControlledNpcs.Move(iPacket);
                    break;

                case ClientOperationCode.HitReactor:
                    this.Character.Map.Reactors.Hit(iPacket, this.Character);
                    break;

                case ClientOperationCode.TouchReactor:
                    this.Character.Map.Reactors.Touch(iPacket, this.Character);
                    break;
            }
        }

        // TODO: Merge all the login packets into one.
        private void Login(InPacket iPacket)
        {
            string username = iPacket.ReadMapleString();
            string password = iPacket.ReadMapleString();

            LoginResult result = LoginResult.Valid;

            if (!username.IsAlphaNumeric())
            {
                result = LoginResult.InvalidUsername;
            }
            else
            {
                this.IsInViewAllChar = false;
                this.Account = new Account(this);

                try
                {
                    this.Account.Load(username);

                    if (SHACryptograph.Encrypt(SHAMode.SHA512, password + this.Account.Salt) != this.Account.Password)
                    {
                        result = LoginResult.InvalidPassword;
                    }
                    else if (this.Account.IsBanned)
                    {
                        result = LoginResult.Banned;
                    }
                    else if (!this.Account.EULA)
                    {
                        result = LoginResult.EULA;
                    }

                    // TODO: Add more scenarios (require master IP, check banned IP, check logged in).
                }
                catch (NoAccountException)
                {
                    if (MasterServer.Login.AutoRegister && username == this.LastUsername && password == this.LastPassword)
                    {
                        this.Account.Username = username;
                        this.Account.Password = SHACryptograph.Encrypt(SHAMode.SHA512, password + this.Account.Salt);
                        this.Account.Salt = HashGenerator.GenerateMD5();
                        this.Account.EULA = false;
                        this.Account.Gender = Gender.Unset;
                        this.Account.Pin = string.Empty;
                        this.Account.Pic = string.Empty;
                        this.Account.IsBanned = false;
                        this.Account.IsMaster = false;
                        this.Account.Birthday = DateTime.UtcNow;
                        this.Account.Creation = DateTime.UtcNow;
                        this.Account.MaxCharacters = MasterServer.Login.MaxCharacters;

                        this.Account.Save();
                    }
                    else
                    {
                        result = LoginResult.InvalidUsername;

                        this.LastUsername = username;
                        this.LastPassword = password;
                    }
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckPasswordResult))
            {
                oPacket
                    .WriteInt((int)result)
                    .WriteByte()
                    .WriteByte();

                if (result == LoginResult.Valid)
                {
                    oPacket
                        .WriteInt(this.Account.ID)
                        .WriteByte((byte)this.Account.Gender)
                        .WriteByte() // NOTE: Grade code.
                        .WriteByte() // NOTE: Subgrade code.
                        .WriteByte() // NOTE: Country code.
                        .WriteMapleString(this.Account.Username)
                        .WriteByte() // NOTE: Unknown.
                        .WriteByte() // NOTE: Quiet ban reason. 
                        .WriteLong() // NOTE: Quiet ban lift date.
                        .WriteDateTime(this.Account.Creation) 
                        .WriteInt() // NOTE: Unknown.
                        .WriteByte((byte)(MasterServer.Login.RequestPin ? 0 : 2)) // NOTE: 1 seems to not do anything.
                        .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2));
                }

                this.Send(oPacket);
            }
        }

        private void EULA(InPacket iPacket)
        {
            bool accepted = iPacket.ReadBool();

            if (accepted)
            {
                this.Account.EULA = true;

                Datum datum = new Datum("accounts");

                datum["EULA"] = true;

                datum.Update("ID = {0}", this.Account.ID);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckPasswordResult))
                {
                    oPacket
                        .WriteInt()
                        .WriteByte()
                        .WriteByte()
                        .WriteInt(this.Account.ID)
                        .WriteByte((byte)this.Account.Gender)
                        .WriteBool()
                        .WriteByte()
                        .WriteByte()
                        .WriteMapleString(this.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteByte((byte)(MasterServer.Login.RequestPin ? 0 : 2)) // NOTE: 1 seems to not do anything.
                        .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2));

                    this.Send(oPacket);
                }
            }
            else
            {
                this.Close(); // NOTE: I'm pretty sure in the real client it disconnects you if you refuse to accept the EULA.
            }
        }

        private void SetGender(InPacket iPacket)
        {
            if (this.Account.Gender != Gender.Unset)
            {
                return;
            }

            bool valid = iPacket.ReadBool();

            if (valid)
            {
                Gender gender = (Gender)iPacket.ReadByte();

                this.Account.Gender = gender;

                Datum datum = new Datum("accounts");

                datum["Gender"] = (byte)this.Account.Gender;

                datum.Update("ID = {0}", this.Account.ID);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckPasswordResult))
                {
                    oPacket
                        .WriteInt()
                        .WriteByte()
                        .WriteByte()
                        .WriteInt(this.Account.ID)
                        .WriteByte((byte)this.Account.Gender)
                        .WriteBool()
                        .WriteByte()
                        .WriteByte()
                        .WriteMapleString(this.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteByte((byte)(MasterServer.Login.RequestPin ? 0 : 2)) // NOTE: 1 seems to not do anything.
                        .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2));

                    this.Send(oPacket);
                }
            }
        }

        private void CheckPin(InPacket iPacket)
        {
            byte a = iPacket.ReadByte();
            byte b = iPacket.ReadByte();

            PinResult result;

            if (b == 0)
            {
                string pin = iPacket.ReadMapleString();

                if (SHACryptograph.Encrypt(SHAMode.SHA256, pin) != this.Account.Pin)
                {
                    result = PinResult.Invalid;
                }
                else
                {
                    if (a == 1)
                    {
                        result = PinResult.Valid;
                    }
                    else if (a == 2)
                    {
                        result = PinResult.Register;
                    }
                    else
                    {
                        result = PinResult.Error;
                    }
                }
            }
            else if (b == 1)
            {
                if (string.IsNullOrEmpty(this.Account.Pin))
                {
                    result = PinResult.Register;
                }
                else
                {
                    result = PinResult.Request;
                }
            }
            else
            {
                result = PinResult.Error;
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckPinCodeResult))
            {
                oPacket.WriteByte((byte)result);

                this.Send(oPacket);
            }
        }

        private void UpdatePin(InPacket iPacket)
        {
            bool procceed = iPacket.ReadBool();
            string pin = iPacket.ReadMapleString();

            if (procceed)
            {
                this.Account.Pin = SHACryptograph.Encrypt(SHAMode.SHA256, pin);

                Datum datum = new Datum("accounts");

                datum["Pin"] = this.Account.Pin;

                datum.Update("ID = {0}", this.Account.ID);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePinCodeResult))
                {
                    oPacket.WriteByte(); // NOTE: All the other result types end up in a "trouble logging into the game" message.

                    this.Send(oPacket);
                }
            }
        }

        private void ListWorlds()
        {
            foreach (WorldServer world in MasterServer.Worlds)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
                {
                    oPacket
                        .WriteByte(world.ID)
                        .WriteMapleString(world.Name)
                        .WriteByte((byte)world.Flag)
                        .WriteMapleString(world.EventMessage)
                        .WriteShort(100) // NOTE: Event EXP rate
                        .WriteShort(100) // NOTE: Event Drop rate
                        .WriteBool(false) // NOTE: Character creation disable.
                        .WriteByte((byte)world.Count);

                    foreach (ChannelServer channel in world)
                    {
                        oPacket
                            .WriteMapleString(channel.Label)
                            .WriteInt(channel.Load)
                            .WriteByte(1)
                            .WriteByte(channel.ID)
                            .WriteBool(false); // NOTE: Adult channel.
                    }

                    //TODO: Add login balloons. These are chat bubbles shown on the world select screen
                    oPacket.WriteShort(); //balloon count
                                          //foreach (var balloon in balloons)
                                          //{
                                          //    oPacket
                                          //        .WriteShort(balloon.X)
                                          //        .WriteShort(balloon.Y)
                                          //        .WriteString(balloon.Text);
                                          //}

                    this.Send(oPacket);
                }

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
                {
                    oPacket.WriteByte(byte.MaxValue);

                    this.Send(oPacket);
                }

                // TODO: Last connected world. Get this from the database. Set the last connected world once you succesfully load a character.
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.LastConnectedWorld))
                {
                    oPacket.WriteInt(); // NOTE: World ID.

                    this.Send(oPacket);
                }

                // TODO: Recommended worlds. Get this from configuration.
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.RecommendedWorldMessage))
                {
                    oPacket
                        .WriteByte(1) // NOTE: Count.
                        .WriteInt() // NOTE: World ID.
                        .WriteMapleString("Check out Scania! The best world to play - and not because it's the only one available... hehe."); // NOTE: Message.

                    this.Send(oPacket);
                }
            }
        }

        private void InformWorldStatus(InPacket iPacket)
        {
            byte worldID = iPacket.ReadByte();

            // TODO: Validate world ID.

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)MasterServer.Worlds[worldID].Status);

                this.Send(oPacket);
            }
        }

        private void SelectWorld(InPacket iPacket)
        {
            iPacket.ReadByte(); // NOTE: Connection kind (GameLaunching, WebStart, etc.).
            byte worldID = iPacket.ReadByte();
            byte channelID = iPacket.ReadByte();
            iPacket.ReadBytes(4); // NOTE: IPv4 Address.

            // TODO: Validate world/channel ID.

            this.WorldID = worldID;
            this.ChannelID = channelID;

            List<Character> characters = new List<Character>();

            foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = {0} AND WorldID = {1}", this.Account.ID, this.WorldID))
            {
                Character character = new Character((int)datum["ID"], this);

                character.Load();

                characters.Add(character);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte((byte)characters.Count);

                foreach (Character character in characters)
                {
                    oPacket.WriteBytes(character.ToByteArray());
                }

                oPacket
                    .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2))
                    .WriteInt(this.Account.MaxCharacters);

                this.Send(oPacket);
            }
        }

        private void ViewAllChar(InPacket iPacket)
        {
            if (this.IsInViewAllChar)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.ViewAllCharResult))
                {
                    oPacket
                        .WriteByte((byte)VACResult.UnknownError)
                        .WriteByte();

                    this.Send(oPacket);
                }

                return;
            }

            this.IsInViewAllChar = true;

            List<Character> characters = new List<Character>();

            foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = {0}", this.Account.ID))
            {
                Character character = new Character((int)datum["ID"], this);

                character.Load();

                characters.Add(character);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ViewAllCharResult))
            {
                if (characters.Count == 0)
                {
                    oPacket
                        .WriteByte((byte)VACResult.NoCharacters);
                }
                else
                {
                    oPacket
                        .WriteByte((byte)VACResult.SendCount)
                        .WriteInt(MasterServer.Worlds.Length)
                        .WriteInt(characters.Count);
                }

                this.Send(oPacket);
            }

            foreach (WorldServer world in MasterServer.Worlds)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.ViewAllCharResult))
                {
                    IEnumerable<Character> worldChars = characters.Where(x => x.WorldID == world.ID);

                    oPacket
                        .WriteByte((byte)VACResult.CharInfo)
                        .WriteByte(world.ID)
                        .WriteByte((byte)worldChars.Count());

                    foreach (Character character in worldChars)
                    {
                        oPacket.WriteBytes(character.ToByteArray());
                    }

                    this.Send(oPacket);
                }
            }
        }

        private void SetViewAllChar(InPacket iPacket)
        {
            this.IsInViewAllChar = iPacket.ReadBool();
        }

        private void CheckCharacterName(InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = Database.Exists("characters", "Name = {0}", name);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteMapleString(name)
                    .WriteBool(unusable);

                this.Send(oPacket);
            }
        }

        private void CreateCharacter(InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            JobType jobType = (JobType)iPacket.ReadInt();
            int face = iPacket.ReadInt();
            int hair = iPacket.ReadInt();
            int hairColor = iPacket.ReadInt();
            byte skin = (byte)iPacket.ReadInt();
            int topID = iPacket.ReadInt();
            int bottomID = iPacket.ReadInt();
            int shoesID = iPacket.ReadInt();
            int weaponID = iPacket.ReadInt();
            Gender gender = (Gender)iPacket.ReadByte();

            bool error = false;

            if (name.Length < 4 || name.Length > 12
                || Database.Exists("characters", "Name = {0}", name)
                || DataProvider.CreationData.ForbiddenNames.Any(forbiddenWord => name.ToLowerInvariant().Contains(forbiddenWord)))
            {
                error = true;
            }

            if (gender == Gender.Male)
            {
                if (!DataProvider.CreationData.MaleSkins.Any(x => x.Item1 == jobType && x.Item2 == skin)
                    || !DataProvider.CreationData.MaleFaces.Any(x => x.Item1 == jobType && x.Item2 == face)
                    || !DataProvider.CreationData.MaleHairs.Any(x => x.Item1 == jobType && x.Item2 == hair)
                    || !DataProvider.CreationData.MaleHairColors.Any(x => x.Item1 == jobType && x.Item2 == hairColor)
                    || !DataProvider.CreationData.MaleTops.Any(x => x.Item1 == jobType && x.Item2 == topID)
                    || !DataProvider.CreationData.MaleBottoms.Any(x => x.Item1 == jobType && x.Item2 == bottomID)
                    || !DataProvider.CreationData.MaleShoes.Any(x => x.Item1 == jobType && x.Item2 == shoesID)
                    || !DataProvider.CreationData.MaleWeapons.Any(x => x.Item1 == jobType && x.Item2 == weaponID))
                {
                    error = true;
                }
            }
            else if (gender == Gender.Female)
            {
                if (!DataProvider.CreationData.FemaleSkins.Any(x => x.Item1 == jobType && x.Item2 == skin)
                    || !DataProvider.CreationData.FemaleFaces.Any(x => x.Item1 == jobType && x.Item2 == face)
                    || !DataProvider.CreationData.FemaleHairs.Any(x => x.Item1 == jobType && x.Item2 == hair)
                    || !DataProvider.CreationData.FemaleHairColors.Any(x => x.Item1 == jobType && x.Item2 == hairColor)
                    || !DataProvider.CreationData.FemaleTops.Any(x => x.Item1 == jobType && x.Item2 == topID)
                    || !DataProvider.CreationData.FemaleBottoms.Any(x => x.Item1 == jobType && x.Item2 == bottomID)
                    || !DataProvider.CreationData.FemaleShoes.Any(x => x.Item1 == jobType && x.Item2 == shoesID)
                    || !DataProvider.CreationData.FemaleWeapons.Any(x => x.Item1 == jobType && x.Item2 == weaponID))
                {
                    error = true;
                }
            }
            else // NOTE: Not allowed to choose "both" genders at character creation.
            {
                error = true;
            }

            Character character = new Character(client: this); // NOTE: Client is passed because it is needed for ViewAllChar.

            character.AccountID = this.Account.ID;
            character.WorldID = this.WorldID;
            character.Name = name;
            character.Gender = gender;
            character.Skin = skin;
            character.Face = face;
            character.Hair = hair + hairColor;
            character.Level = 1;
            character.Job = jobType == JobType.Cygnus ? Job.Noblesse : jobType == JobType.Explorer ? Job.Beginner : Job.Legend;
            character.Strength = 12;
            character.Dexterity = 5;
            character.Intelligence = 4;
            character.Luck = 4;
            character.MaxHealth = 50;
            character.MaxMana = 5;
            character.Health = 50;
            character.Mana = 5;
            character.AbilityPoints = 0;
            character.SkillPoints = 0;
            character.Experience = 0;
            character.Fame = 0;
            character.Map = this.Channel.Maps[jobType == JobType.Cygnus ? 130030000 : jobType == JobType.Explorer ? 10000 : 914000000];
            character.SpawnPoint = 0;
            character.Meso = 0;

            character.Items.Add(new Item(topID, equipped: true));
            character.Items.Add(new Item(bottomID, equipped: true));
            character.Items.Add(new Item(shoesID, equipped: true));
            character.Items.Add(new Item(weaponID, equipped: true));
            character.Items.Add(new Item(jobType == JobType.Cygnus ? 4161047 : jobType == JobType.Explorer ? 4161001 : 4161048), forceGetSlot: true);

            character.Keymap.Add(new Shortcut(KeymapKey.One, KeymapAction.AllChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Two, KeymapAction.PartyChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Three, KeymapAction.BuddyChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Four, KeymapAction.GuildChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Five, KeymapAction.AllianceChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Six, KeymapAction.SpouseChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Q, KeymapAction.QuestMenu));
            character.Keymap.Add(new Shortcut(KeymapKey.W, KeymapAction.WorldMap));
            character.Keymap.Add(new Shortcut(KeymapKey.E, KeymapAction.EquipmentMenu));
            character.Keymap.Add(new Shortcut(KeymapKey.R, KeymapAction.BuddyList));
            character.Keymap.Add(new Shortcut(KeymapKey.I, KeymapAction.ItemMenu));
            character.Keymap.Add(new Shortcut(KeymapKey.O, KeymapAction.PartySearch));
            character.Keymap.Add(new Shortcut(KeymapKey.P, KeymapAction.PartyList));
            character.Keymap.Add(new Shortcut(KeymapKey.BracketLeft, KeymapAction.Shortcut));
            character.Keymap.Add(new Shortcut(KeymapKey.BracketRight, KeymapAction.QuickSlot));
            character.Keymap.Add(new Shortcut(KeymapKey.LeftCtrl, KeymapAction.Attack));
            character.Keymap.Add(new Shortcut(KeymapKey.S, KeymapAction.AbilityMenu));
            character.Keymap.Add(new Shortcut(KeymapKey.F, KeymapAction.FamilyList));
            character.Keymap.Add(new Shortcut(KeymapKey.G, KeymapAction.GuildList));
            character.Keymap.Add(new Shortcut(KeymapKey.H, KeymapAction.WhisperChat));
            character.Keymap.Add(new Shortcut(KeymapKey.K, KeymapAction.SkillMenu));
            character.Keymap.Add(new Shortcut(KeymapKey.L, KeymapAction.QuestHelper));
            character.Keymap.Add(new Shortcut(KeymapKey.Semicolon, KeymapAction.Medal));
            character.Keymap.Add(new Shortcut(KeymapKey.Quote, KeymapAction.ExpandChat));
            character.Keymap.Add(new Shortcut(KeymapKey.Backtick, KeymapAction.CashShop));
            character.Keymap.Add(new Shortcut(KeymapKey.Backslash, KeymapAction.SetKey));
            character.Keymap.Add(new Shortcut(KeymapKey.Z, KeymapAction.PickUp));
            character.Keymap.Add(new Shortcut(KeymapKey.X, KeymapAction.Sit));
            character.Keymap.Add(new Shortcut(KeymapKey.C, KeymapAction.Messenger));
            character.Keymap.Add(new Shortcut(KeymapKey.B, KeymapAction.MonsterBook));
            character.Keymap.Add(new Shortcut(KeymapKey.M, KeymapAction.MiniMap));
            character.Keymap.Add(new Shortcut(KeymapKey.LeftAlt, KeymapAction.Jump));
            character.Keymap.Add(new Shortcut(KeymapKey.Space, KeymapAction.NpcChat));
            character.Keymap.Add(new Shortcut(KeymapKey.F1, KeymapAction.Cockeyed));
            character.Keymap.Add(new Shortcut(KeymapKey.F2, KeymapAction.Happy));
            character.Keymap.Add(new Shortcut(KeymapKey.F3, KeymapAction.Sarcastic));
            character.Keymap.Add(new Shortcut(KeymapKey.F4, KeymapAction.Crying));
            character.Keymap.Add(new Shortcut(KeymapKey.F5, KeymapAction.Outraged));
            character.Keymap.Add(new Shortcut(KeymapKey.F6, KeymapAction.Shocked));
            character.Keymap.Add(new Shortcut(KeymapKey.F7, KeymapAction.Annoyed));

            if (!error)
            {
                character.Save();
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CreateNewCharacterResult))
            {
                oPacket.WriteBool(error);

                if (!error)
                {
                    oPacket.WriteBytes(character.ToByteArray());
                }

                this.Send(oPacket);
            }
        }

        // TODO: Proper character deletion with all the necessary checks (cash items, guilds, etcetera). 
        private void DeleteCharacter(InPacket iPacket)
        {
            string pic = iPacket.ReadMapleString();
            int characterID = iPacket.ReadInt();

            if (!Database.Exists("characters", "ID = {0} AND AccountID = {1}", characterID, this.Account.ID))
            {
                this.Terminate();

                return;
            }

            CharacterDeletionResult result;

            if (SHACryptograph.Encrypt(SHAMode.SHA256, pic) == this.Account.Pic || !MasterServer.Login.RequestPic)
            {
                //NOTE: As long as foreign keys are set to cascade, all child entries related to this CharacterID will also be deleted.
                Database.Delete("characters", "ID = {0}", characterID);

                result = CharacterDeletionResult.Valid;
            }
            else
            {
                result = CharacterDeletionResult.InvalidPic;
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.DeleteCharacterResult))
            {
                oPacket
                    .WriteInt(characterID)
                    .WriteByte((byte)result);

                this.Send(oPacket);
            }
        }

        private void SelectCharacter(InPacket iPacket, bool fromViewAll = false, bool requestPic = false, bool registerPic = false)
        {
            string pic = string.Empty;

            if (requestPic)
            {
                pic = iPacket.ReadMapleString();
            }
            else if (registerPic)
            {
                iPacket.ReadByte();
            }

            int characterID = iPacket.ReadInt();

            if (!Database.Exists("characters", "ID = {0} AND AccountID = {1}", characterID, this.Account.ID))
            {
                this.Terminate();

                return;
            }

            if (this.IsInViewAllChar)
            {
                this.WorldID = (byte)iPacket.ReadInt();
                this.ChannelID = 0; // TODO: Least loaded channel.
            }

            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            if (registerPic)
            {
                iPacket.ReadMapleString();
                pic = iPacket.ReadMapleString();

                if (string.IsNullOrEmpty(this.Account.Pic))
                {
                    this.Account.Pic = SHACryptograph.Encrypt(SHAMode.SHA256, pic);

                    Datum datum = new Datum("accounts");

                    datum["Pic"] = this.Account.Pic;

                    datum.Update("ID = {0}", this.Account.ID);
                }
            }

            if (!requestPic || SHACryptograph.Encrypt(SHAMode.SHA256, pic) == this.Account.Pic)
            {
                this.Channel.Migrations.Add(new MigrationData(this.Host, this.Account.ID, characterID));

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectCharacterResult))
                {
                    oPacket
                        .WriteByte()
                        .WriteByte()
                        .WriteBytes(this.World.HostIP.GetAddressBytes())
                        .WriteShort(this.Channel.Port)
                        .WriteInt(characterID)
                        .WriteInt()
                        .WriteByte();

                    this.Send(oPacket);
                }
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckSPWResult))
                {
                    oPacket.WriteByte();

                    this.Send(oPacket);
                }
            }
        }

        private void LoadCharacter(InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();
            iPacket.Skip(2); //NOTE: Unknown

            if ((accountID = this.Channel.Migrations.Validate(this.Host, characterID)) == -1)
            {
                this.Close();

                return;
            }

            this.Account = new Account(this);
            this.Account.Load(accountID);

            this.Character = new Character(characterID, this);
            this.Character.Load(true);

            this.Channel.Characters.Register(this.Character);

            this.Character.Initialize();
        }

        private void ChangeChannel(InPacket iPacket)
        {
            byte id = iPacket.ReadByte();

            // TODO: Validate channel ID.

            ChannelServer destination = this.World[id];

            destination.Migrations.Add(new MigrationData(this.Host, this.Account.ID, this.Character.ID));

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MigrateCommand))
            {
                oPacket
                    .WriteBool(true)
                    .WriteBytes(this.World.HostIP.GetAddressBytes())
                    .WriteShort(destination.Port);

                this.Send(oPacket);
            }
        }
    }
}