using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Core.Security;
using Destiny.Data;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Destiny
{
    public sealed class MapleClient : Session
    {
        public Account Account { get; set; }
        public Character Character { get; set; }

        public string LastUsername { get; private set; }
        public string LastPassword { get; private set; }

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

            if (this.IsInCashShop)
            {
                MasterServer.CashShop.Clients.Remove(this);
            }
            else if (this.Channel >= 0)
            {
                MasterServer.Channels[this.Channel].Clients.Remove(this);
            }

            MasterServer.Login.Clients.Remove(this);
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

                case ClientOperationCode.CharacterSelectRegisterPic:
                    this.SelectCharacter(iPacket, registerPic: true);
                    break;

                case ClientOperationCode.CharacterSelectRequestPic:
                    this.SelectCharacter(iPacket, requestPic: true);
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

                //case ClientOperationCode.DueyAction:
                //    break;

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
                    this.Character.DistributeAP(iPacket);
                    break;

                case ClientOperationCode.AutoDistributeAP:
                    this.Character.AutoDistributeAP(iPacket);
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
                    this.Character.Quests.Handle(iPacket);
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
                        this.Account.Salt = HashGenerator.GenerateMD5();
                        this.Account.Password = SHACryptograph.Encrypt(SHAMode.SHA512, password + this.Account.Salt);
                        this.Account.Pin = string.Empty;
                        this.Account.Pic = string.Empty;
                        this.Account.IsBanned = false;
                        this.Account.IsMaster = false;
                        this.Account.Creation = DateTime.UtcNow;
                        this.Account.Birthday = DateTime.UtcNow;

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
                        .WriteBool()
                        .WriteByte()
                        .WriteByte()
                        .WriteMapleString(this.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteShort((short)(MasterServer.Login.RequestPin ? 0 : 2)); // NOTE: 1 seems to not do anything.
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
                this.Account.Save();

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
                        .WriteShort((short)(MasterServer.Login.RequestPin ? 0 : 2)); // NOTE: 1 seems to not do anything.

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
                this.Account.Save();

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
                        .WriteShort((short)(MasterServer.Login.RequestPin ? 0 : 2)); // NOTE: 1 seems to not do anything.

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
                this.Account.Save();

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.UpdatePinCodeResult))
                {
                    oPacket.WriteByte(); // NOTE: All the other result types end up in a "trouble logging into the game" message.

                    this.Send(oPacket);
                }
            }
        }

        private void ListWorlds()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
            {
                oPacket
                    .WriteByte()
                    .WriteMapleString(MasterServer.World.Name)
                    .WriteByte((byte)MasterServer.World.Flag)
                    .WriteMapleString(MasterServer.World.EventMessage)
                    .WriteShort(100) //Event EXP rate
                    .WriteShort(100) //Event Drop rate
                    .WriteByte()
                    .WriteByte((byte)MasterServer.Channels.Length);

                foreach (ChannelServer channel in MasterServer.Channels)
                {
                    oPacket
                        .WriteMapleString(channel.Label)
                        .WriteInt(channel.Load)
                        .WriteByte(1)
                        .WriteByte(channel.ID)
                        .WriteByte(); //Adult-only channel
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
            this.Channel = iPacket.ReadByte();

            List<Character> characters = new List<Character>();

            foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = '{0}'", this.Account.ID))
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
                    character.Encode(oPacket);
                }

                oPacket
                    .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2))
                    .WriteInt(MasterServer.Login.MaxCharacters); // TODO: Account specific character creation slots. For now, use server-configured value.

                this.Send(oPacket);
            }
        }

        private void CheckCharacterName(InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = Database.Exists("characters", "Name = '{0}'", name);

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

            //Name constraints
            if (name.Length < 4 || name.Length > 12
                || Database.Exists("characters", "Name = '{0}'", name)
                || DataProvider.CharacterCreationData.ForbiddenNames.Any(forbiddenWord => name.ToLowerInvariant().Contains(forbiddenWord)))
            {
                error = true;
            }

            //Gender-specific cosmetic/item checks
            if (gender == Gender.Male)
            {
                if (!DataProvider.CharacterCreationData.MaleSkins.Any(x => x.Item1 == jobType && x.Item2 == skin)
                    || !DataProvider.CharacterCreationData.MaleFaces.Any(x => x.Item1 == jobType && x.Item2 == face)
                    || !DataProvider.CharacterCreationData.MaleHairs.Any(x => x.Item1 == jobType && x.Item2 == hair)
                    || !DataProvider.CharacterCreationData.MaleHairColors.Any(x => x.Item1 == jobType && x.Item2 == hairColor)
                    || !DataProvider.CharacterCreationData.MaleTops.Any(x => x.Item1 == jobType && x.Item2 == topID)
                    || !DataProvider.CharacterCreationData.MaleBottoms.Any(x => x.Item1 == jobType && x.Item2 == bottomID)
                    || !DataProvider.CharacterCreationData.MaleShoes.Any(x => x.Item1 == jobType && x.Item2 == shoesID)
                    || !DataProvider.CharacterCreationData.MaleWeapons.Any(x => x.Item1 == jobType && x.Item2 == weaponID))
                {
                    error = true;
                }
            }
            else if (gender == Gender.Female)
            {
                if (!DataProvider.CharacterCreationData.FemaleSkins.Any(x => x.Item1 == jobType && x.Item2 == skin)
                    || !DataProvider.CharacterCreationData.FemaleFaces.Any(x => x.Item1 == jobType && x.Item2 == face)
                    || !DataProvider.CharacterCreationData.FemaleHairs.Any(x => x.Item1 == jobType && x.Item2 == hair)
                    || !DataProvider.CharacterCreationData.FemaleHairColors.Any(x => x.Item1 == jobType && x.Item2 == hairColor)
                    || !DataProvider.CharacterCreationData.FemaleTops.Any(x => x.Item1 == jobType && x.Item2 == topID)
                    || !DataProvider.CharacterCreationData.FemaleBottoms.Any(x => x.Item1 == jobType && x.Item2 == bottomID)
                    || !DataProvider.CharacterCreationData.FemaleShoes.Any(x => x.Item1 == jobType && x.Item2 == shoesID)
                    || !DataProvider.CharacterCreationData.FemaleWeapons.Any(x => x.Item1 == jobType && x.Item2 == weaponID))
                {
                    error = true;
                }
            }
            else
            {
                //Not allowed to choose "both" genders at character creation
                error = true;
            }

            Character character = new Character();

            character.AccountID = this.Account.ID;
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
            character.Health = 50;
            character.MaxHealth = 50;
            character.Mana = 5;
            character.MaxMana = 5;
            character.AbilityPoints = 0;
            character.SkillPoints = 0;
            character.Experience = 0;
            character.Fame = 0;
            character.Map = MasterServer.Channels[this.Channel].Maps[jobType == JobType.Cygnus ? 130030000 : jobType == JobType.Explorer ? 10000 : 914000000];
            character.SpawnPoint = 0;
            character.Meso = 0;

            character.Items.Add(new Item(topID, equipped: true));
            character.Items.Add(new Item(bottomID, equipped: true));
            character.Items.Add(new Item(shoesID, equipped: true));
            character.Items.Add(new Item(weaponID, equipped: true));
            character.Items.Add(new Item(jobType == JobType.Cygnus ? 4161047 : jobType == JobType.Explorer ? 4161001 : 4161048), forceGetSlot: true);

            if (!error)
            {
                character.Save();
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CreateNewCharacterResult))
            {
                oPacket.WriteBool(error);

                if (!error)
                {
                    character.Encode(oPacket);
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

            if (SHACryptograph.Encrypt(SHAMode.SHA256, pic) == this.Account.Pic)
            {
                Database.Delete("characters", "ID = '{0}'", characterID);
                Database.Delete("buffs", "CharacterID = '{0}'", characterID);
                Database.Delete("items", "CharacterID = '{0}'", characterID);
                Database.Delete("keymaps", "CharacterID = '{0}'", characterID);
                Database.Delete("quests_completed", "CharacterID = '{0}'", characterID);
                Database.Delete("quests_started", "CharacterID = '{0}'", characterID);
                Database.Delete("skills", "CharacterID = '{0}'", characterID);

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

            if (fromViewAll)
            {
                iPacket.ReadInt(); // NOTE: World ID.
                this.Channel = 0; // TODO: Least loaded channel.
            }

            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            if (registerPic)
            {
                iPacket.ReadMapleString();
                pic = iPacket.ReadMapleString();

                if (string.IsNullOrEmpty(this.Account.Pic))
                {
                    this.Account.Pic = SHACryptograph.Encrypt(SHAMode.SHA256, pic);
                    this.Account.Save();
                }
            }

            if (!requestPic || SHACryptograph.Encrypt(SHAMode.SHA256, pic) == this.Account.Pic)
            {
                MasterServer.Channels[this.Channel].Migrations.Add(this.Host, this.Account.ID, characterID);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectCharacterResult))
                {
                    oPacket
                        .WriteByte()
                        .WriteByte()
                        .WriteBytes(MasterServer.World.HostIP.GetAddressBytes())
                        .WriteShort(MasterServer.Channels[this.Channel].Port)
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

            if ((accountID = MasterServer.Channels[this.Channel].Migrations.Validate(this.Host, characterID)) == -1)
            {
                this.Close();

                return;
            }

            this.Account = new Account(this);
            this.Account.Load(accountID);

            this.Character = new Character(characterID, this);
            this.Character.Load();
            this.Character.Initialize();
        }
    }
}
