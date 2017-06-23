using Destiny.Core.Network;
using System.Net.Sockets;
using Destiny.Maple;
using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Security;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using Destiny.Server;
using System.Collections.Generic;

namespace Destiny
{
    public sealed class MapleClient : Session
    {
        public Account Account { get; set; }
        public Character Character { get; set; }

        public byte Channel { get; set; }
        public bool IsInCashShop { get; set; }

        public MapleClient(Socket socket) : base(socket) { }

        protected override void Terminate()
        {
            if (this.Character != null)
            {
                this.Character.Save();

                this.Character.Map.Characters.Remove(this.Character);
            }

            // TODO: Remove client from server.
        }

        protected override void Dispatch(InPacket iPacket)
        {
            switch (iPacket.OperationCode)
            {
                case ClientOperationCode.AccountLogin:
                    this.Login(iPacket);
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
                    this.SelectCharacter(iPacket);
                    break;

                case ClientOperationCode.CharacterLoad:
                    this.LoadCharacter(iPacket);
                    break;

                case ClientOperationCode.MapChange:
                    this.Character.ChangeMap(iPacket);
                    break;

                case ClientOperationCode.ChannelChange:
                    break;

                case ClientOperationCode.CashShopMigration:
                    break;

                case ClientOperationCode.PlayerMovement:
                    this.Character.Move(iPacket);
                    break;

                case ClientOperationCode.CancelChair:
                    break;

                case ClientOperationCode.UseChair:
                    break;

                case ClientOperationCode.CloseRangeAttack:
                    break;

                case ClientOperationCode.RangedAttack:
                    break;

                case ClientOperationCode.MagicAttack:
                    break;

                case ClientOperationCode.EnergyOrbAttack:
                    break;

                case ClientOperationCode.TakeDamage:
                    break;

                case ClientOperationCode.PlayerChat:
                    this.Character.Talk(iPacket);
                    break;

                case ClientOperationCode.CloseChalkboard:
                    break;

                case ClientOperationCode.FaceExpression:
                    this.Character.Express(iPacket);
                    break;

                case ClientOperationCode.UseItemEffect:
                    break;

                case ClientOperationCode.UseDeathItem:
                    break;

                case ClientOperationCode.NpcConverse:
                    this.Character.Converse(iPacket);
                    break;

                case ClientOperationCode.NpcResult:
                    this.Character.NpcScript.Handle(iPacket);
                    break;

                case ClientOperationCode.NpcShop:
                    break;

                case ClientOperationCode.Storage:
                    break;

                case ClientOperationCode.HiredMerchant:
                    break;

                case ClientOperationCode.DueyAction:
                    break;

                case ClientOperationCode.ItemSort:
                    break;

                case ClientOperationCode.ItemSort2:
                    break;

                case ClientOperationCode.ItemMovement:
                    this.Character.Items.Handle(iPacket);
                    break;

                case ClientOperationCode.UseItem:
                    break;

                case ClientOperationCode.CancelItemEffect:
                    break;

                case ClientOperationCode.UseSummonBag:
                    break;

                case ClientOperationCode.UsePetFood:
                    break;

                case ClientOperationCode.UseMountFood:
                    break;

                case ClientOperationCode.UseScriptedItem:
                    break;

                case ClientOperationCode.UseCashItem:
                    break;

                case ClientOperationCode.UseCatchItem:
                    break;

                case ClientOperationCode.UseSkillBook:
                    break;

                case ClientOperationCode.UseTeleportRock:
                    break;

                case ClientOperationCode.UseReturnScroll:
                    break;

                case ClientOperationCode.UseUpgradeScroll:
                    break;

                case ClientOperationCode.DistributeAP:
                    break;

                case ClientOperationCode.AutoDistributeAP:
                    break;

                case ClientOperationCode.HealOverTime:
                    break;

                case ClientOperationCode.DistributeSP:
                    break;

                case ClientOperationCode.SpecialMove:
                    break;

                case ClientOperationCode.CancelBuff:
                    break;

                case ClientOperationCode.SkillEffect:
                    break;

                case ClientOperationCode.MesoDrop:
                    this.Character.DropMeso(iPacket);
                    break;

                case ClientOperationCode.GiveFame:
                    break;

                case ClientOperationCode.PlayerInformation:
                    this.Character.InformOnCharacter(iPacket);
                    break;

                case ClientOperationCode.SpawnPet:
                    break;

                case ClientOperationCode.CancelDebuff:
                    break;

                case ClientOperationCode.ChangeMapSpecial:
                    this.Character.ChangeMapSpecial(iPacket);
                    break;

                case ClientOperationCode.UseInnerPortal:
                    break;

                case ClientOperationCode.TrockAddMap:
                    break;

                case ClientOperationCode.Report:
                    break;

                case ClientOperationCode.QuestAction:
                    break;

                case ClientOperationCode.SkillMacro:
                    break;

                case ClientOperationCode.SpouseChat:
                    break;

                case ClientOperationCode.UseFishingItem:
                    break;

                case ClientOperationCode.MakerSkill:
                    break;

                case ClientOperationCode.UseRemote:
                    break;

                case ClientOperationCode.PartyChat:
                    break;

                case ClientOperationCode.Whisper:
                    break;

                case ClientOperationCode.Messenger:
                    break;

                case ClientOperationCode.PlayerInteraction:
                    break;

                case ClientOperationCode.PartyOperation:
                    break;

                case ClientOperationCode.DenyPartyRequest:
                    break;

                case ClientOperationCode.GuildOperation:
                    break;

                case ClientOperationCode.DenyGuildRequest:
                    break;

                case ClientOperationCode.AdminCommand:
                    break;

                case ClientOperationCode.AdminLog:
                    break;

                case ClientOperationCode.BuddyListModify:
                    break;

                case ClientOperationCode.NoteAction:
                    break;

                case ClientOperationCode.UseDoor:
                    break;

                case ClientOperationCode.ChangeKeymap:
                    break;

                case ClientOperationCode.RingAction:
                    break;

                case ClientOperationCode.OpenFamily:
                    break;

                case ClientOperationCode.AddFamily:
                    break;

                case ClientOperationCode.AcceptFamily:
                    break;

                case ClientOperationCode.AllianceOperation:
                    break;

                case ClientOperationCode.BbsOperation:
                    break;

                case ClientOperationCode.MtsMigration:
                    break;

                case ClientOperationCode.PetTalk:
                    break;

                case ClientOperationCode.UseSolomonItem:
                    break;

                case ClientOperationCode.MovePet:
                    break;

                case ClientOperationCode.PetChat:
                    break;

                case ClientOperationCode.PetCommand:
                    break;

                case ClientOperationCode.PetLoot:
                    break;

                case ClientOperationCode.PetAutoPot:
                    break;

                case ClientOperationCode.PetExcludeItems:
                    break;

                case ClientOperationCode.MoveSummon:
                    break;

                case ClientOperationCode.SummonAttack:
                    break;

                case ClientOperationCode.DamageSummon:
                    break;

                case ClientOperationCode.Beholder:
                    break;

                case ClientOperationCode.MobMovement:
                    break;

                case ClientOperationCode.AutoAggro:
                    break;

                case ClientOperationCode.MobDamageModFriendly:
                    break;

                case ClientOperationCode.MonsterBomb:
                    break;

                case ClientOperationCode.MobDamageMob:
                    break;

                case ClientOperationCode.NpcAction:
                    break;

                case ClientOperationCode.DropPickup:
                    this.Character.Items.Pickup(iPacket);
                    break;

                case ClientOperationCode.DamageReactor:
                    break;

                case ClientOperationCode.ChangedMap:
                    break;

                case ClientOperationCode.NpcMovement:
                    break;

                case ClientOperationCode.TouchingReactor:
                    break;

                case ClientOperationCode.MonsterCarnival:
                    break;

                case ClientOperationCode.PartySearchRegister:
                    break;

                case ClientOperationCode.PartySearchStart:
                    break;

                case ClientOperationCode.PlayerUpdate:
                    break;

                case ClientOperationCode.CashShopOperation:
                    break;

                case ClientOperationCode.BuyCashItem:
                    break;

                case ClientOperationCode.CouponCode:
                    break;

                case ClientOperationCode.OpenItemInterface:
                    break;

                case ClientOperationCode.CloseItemInterface:
                    break;

                case ClientOperationCode.UseItemInterface:
                    break;

                case ClientOperationCode.MtsOperation:
                    break;

                case ClientOperationCode.UseMapleLife:
                    break;

                case ClientOperationCode.UseHammer:
                    break;

                case ClientOperationCode.MapleTV:
                    break;
            }
        }

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
                Account account = null;

                using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `username` = @username", new MySqlParameter("username", username)))
                {
                    if (query.NextRow())
                    {
                        account = new Account(query);
                    }
                }

                if (account == null)
                {
                    result = LoginResult.InvalidUsername;
                }
                else if (SHACryptograph.Encrypt(SHAMode.SHA512, password + account.Salt) != account.Password)
                {
                    result = LoginResult.InvalidPassword;
                }
                else if (false) // TODO: Handle ban scenario.
                {
                    result = LoginResult.Banned;
                }
                else if (false) // TODO: Handle logged-in scenario.
                {
                    result = LoginResult.LoggedIn;
                }
                else
                {
                    this.Account = account;
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
                        .WriteByte()
                        .WriteBool()
                        .WriteByte()
                        .WriteByte()
                        .WriteMapleString(this.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteShort(2);
                }

                this.Send(oPacket);
            }
        }

        private void ListWorlds()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
            {
                oPacket
                    .WriteByte()
                    .WriteMapleString("Destiny")
                    .WriteByte((byte)WorldFlag.New)
                    .WriteMapleString("Welcome to #rDestiny#k!")
                    .WriteShort(100)
                    .WriteShort(100)
                    .WriteByte()
                    .WriteByte((byte)MasterServer.Channels.Length);

                foreach (ChannelServer channel in MasterServer.Channels)
                {
                    oPacket
                        .WriteMapleString(channel.Label)
                        .WriteInt(channel.Load)
                        .WriteByte(1)
                        .WriteShort(channel.ID);
                }

                oPacket.WriteShort();

                this.Send(oPacket);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
            {
                oPacket.WriteByte(byte.MaxValue);

                this.Send(oPacket);
            }
        }

        private void InformWorldStatus(InPacket iPacket)
        {
            iPacket.Skip(1); // NOTE: World ID, but we're not using it.

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)WorldStatus.Normal);

                this.Send(oPacket);
            }
        }

        private void SelectWorld(InPacket iPacket)
        {
            iPacket.Skip(1);
            iPacket.Skip(1); // NOTE: World ID, but we're not using it.
            byte channelID = iPacket.ReadByte();

            // TODO: Validate channel ID.

            this.Channel = channelID;

            byte characterCount = (byte)(long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `account_id` = @account_id", new MySqlParameter("@account_id", this.Account.ID));

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte(characterCount);

                if (characterCount > 0)
                {
                    using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `account_id` = @account_id", new MySqlParameter("@account_id", this.Account.ID)))
                    {
                        while (query.NextRow())
                        {
                            this.AddCharacterEntry(oPacket, query);
                        }
                    }
                }

                oPacket
                    .WriteByte(2)
                    .WriteInt(3); // TODO: Account specific character creation slots. For now, use default 3.

                this.Send(oPacket);
            }
        }

        private void CheckCharacterName(InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = (long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `name` = @name", new MySqlParameter("name", name)) != 0;

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
            int jobType = iPacket.ReadInt();
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

            // TODO: Validate name, beauty and equipment before creating the character.

            int id = Database.InsertAndReturnIdentifier("INSERT INTO `characters` (account_id, name, gender, skin, face, hair) " +
                                                        "VALUES (@account_id, @name, @gender, @skin, @face, @hair)",
                                                        new MySqlParameter("account_id", this.Account.ID),
                                                        new MySqlParameter("name", name),
                                                        new MySqlParameter("gender", gender),
                                                        new MySqlParameter("skin", skin),
                                                        new MySqlParameter("face", face),
                                                        new MySqlParameter("hair", hair));

            // TODO: Validate the default equipment statistics. I'm pretty sure some of them are untradable.

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, weapon_defense) " +
                             "VALUES (@character_id, 1, -5, @maple_id, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", topID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, weapon_defense) " +
                             "VALUES (@character_id, 1, -6, @maple_id, 2)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", bottomID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, slots, weapon_defense) " +
                             "VALUES (@character_id, 1, -7, @maple_id, 5, 3)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", shoesID));

            Database.Execute("INSERT INTO `items` (character_id, inventory, slot, maple_id, slots, weapon_attack) " +
                             "VALUES (@character_id, 1, -11, @maple_id, 7, 17)",
                             new MySqlParameter("character_id", id),
                             new MySqlParameter("maple_id", weaponID));

            // TODO: Add beginner's guide (based on job).

            using (DatabaseQuery query = Database.Query("SELECT * FROM characters WHERE `character_id` = @character_id", new MySqlParameter("character_id", id)))
            {
                query.NextRow();

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.CreateNewCharacterResult))
                {
                    oPacket.WriteBool(error);

                    this.AddCharacterEntry(oPacket, query);

                    this.Send(oPacket);
                }
            }
        }

        private void DeleteCharacter(InPacket iPacket)
        {

        }

        private void SelectCharacter(InPacket iPacket)
        {
            int characterID = iPacket.ReadInt();
            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            MasterServer.Channels[this.Channel].Migrations.Add(this.Host, this.Account.ID, characterID);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectCharacterResult))
            {
                oPacket
                    .WriteByte()
                    .WriteByte()
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(MasterServer.Channels[this.Channel].Port)
                    .WriteInt(characterID)
                    .WriteInt()
                    .WriteByte();

                this.Send(oPacket);
            }
        }

        private void LoadCharacter(InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();

            if ((accountID = MasterServer.Channels[this.Channel].Migrations.Validate(this.Host, characterID)) == -1)
            {
                this.Close();

                return;
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `accounts` WHERE `account_id` = @account_id", new MySqlParameter("account_id", accountID)))
            {
                query.NextRow();

                this.Account = new Account(query);
            }

            using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `character_id` = @character_id", new MySqlParameter("character_id", characterID)))
            {
                query.NextRow();

                this.Character = new Character(this, query);
            }

            this.Character.Initialize();
        }

        private void AddCharacterEntry(OutPacket oPacket, DatabaseQuery query)
        {
            oPacket
                .WriteInt(query.GetInt("character_id"))
                .WritePaddedString(query.GetString("name"), 13)
                .WriteByte(query.GetByte("gender"))
                .WriteByte(query.GetByte("skin"))
                .WriteInt(query.GetInt("face"))
                .WriteInt(query.GetInt("hair"))
                .WriteLong()
                .WriteLong()
                .WriteLong()
                .WriteByte(query.GetByte("level"))
                .WriteShort(query.GetShort("job"))
                .WriteShort(query.GetShort("strength"))
                .WriteShort(query.GetShort("dexterity"))
                .WriteShort(query.GetShort("intelligence"))
                .WriteShort(query.GetShort("luck"))
                .WriteShort(query.GetShort("health"))
                .WriteShort(query.GetShort("max_health"))
                .WriteShort(query.GetShort("mana"))
                .WriteShort(query.GetShort("max_mana"))
                .WriteShort(query.GetShort("ability_points"))
                .WriteShort(query.GetShort("skill_points"))
                .WriteInt(query.GetInt("experience"))
                .WriteShort(query.GetShort("fame"))
                .WriteInt()
                .WriteInt(query.GetInt("map"))
                .WriteByte(query.GetByte("spawn_point"))
                .WriteInt();

            oPacket
                .WriteByte(query.GetByte("gender"))
                .WriteByte(query.GetByte("skin"))
                .WriteInt(query.GetInt("face"))
                .WriteBool(true)
                .WriteInt(query.GetInt("hair"));

            Dictionary<byte, int> visibleLayer = new Dictionary<byte, int>();
            Dictionary<byte, int> hiddenLayer = new Dictionary<byte, int>();

            using (DatabaseQuery equipmentQuery = Database.Query("SELECT `slot`, `maple_id` FROM `items` WHERE `character_id` = @character_id AND `inventory` = 1 AND `slot` < 0", new MySqlParameter("@character_id", query.GetInt("character_id"))))
            {
                while (equipmentQuery.NextRow())
                {
                    byte slot = (byte)(short)(-(equipmentQuery.GetShort("slot")));
                    int mapleID = equipmentQuery.GetInt("maple_id");

                    if (slot < 100 && !visibleLayer.ContainsKey(slot))
                    {
                        visibleLayer[slot] = mapleID;
                    }
                    else if (slot > 100 && slot != 111)
                    {
                        slot -= 100;

                        if (visibleLayer.ContainsKey(slot))
                        {
                            hiddenLayer[slot] = visibleLayer[slot];
                        }

                        visibleLayer[slot] = mapleID;
                    }
                    else if (visibleLayer.ContainsKey(slot))
                    {
                        hiddenLayer[slot] = mapleID;
                    }
                }
            }

            foreach (KeyValuePair<byte, int> entry in visibleLayer)
            {
                oPacket
                    .WriteByte(entry.Key)
                    .WriteInt(entry.Value);
            }

            oPacket.WriteByte(byte.MaxValue);

            foreach (KeyValuePair<byte, int> entry in hiddenLayer)
            {
                oPacket
                    .WriteByte(entry.Key)
                    .WriteInt(entry.Value);
            }

            oPacket.WriteByte(byte.MaxValue);

            oPacket
                .WriteInt() // TODO: Cash weapon.
                .WriteZero(12)
                .WriteByte()
                .WriteBool();
        }
    }
}
