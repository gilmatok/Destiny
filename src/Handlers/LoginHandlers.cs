using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Core.Security;
using Destiny.Data;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Destiny.Handlers
{
    public static class LoginHandlers
    {
        public static void HandleAccountLogin(MapleClient client, InPacket iPacket)
        {
            string username = iPacket.ReadMapleString();
            string password = iPacket.ReadMapleString();

            LoginResult result = LoginResult.Success;

            if (!username.IsAlphaNumeric())
            {
                result = LoginResult.InvalidUsername;
            }
            else
            {
                client.Account = new Account(client);

                try
                {
                    client.Account.Load(username);

                    if (SHACryptograph.Encrypt(SHAMode.SHA512, password + client.Account.Salt) != client.Account.Password)
                    {
                        result = LoginResult.InvalidPassword;
                    }
                    else if (client.Account.IsBanned)
                    {
                        result = LoginResult.Banned;
                    }
                    else if (!client.Account.EULA)
                    {
                        result = LoginResult.EULA;
                    }

                    // TODO: Add more scenarios (require master IP, check banned IP, check logged in).
                }
                catch (NoAccountException)
                {
                    if (MasterServer.Login.AutoRegister && username == client.LastUsername && password == client.LastUsername)
                    {
                        client.Account.Username = username;
                        client.Account.Salt = HashGenerator.GenerateMD5();
                        client.Account.Password = SHACryptograph.Encrypt(SHAMode.SHA512, password + client.Account.Salt);
                        client.Account.Pin = string.Empty;
                        client.Account.Pic = string.Empty;
                        client.Account.IsBanned = false;
                        client.Account.IsMaster = false;
                        client.Account.Creation = DateTime.UtcNow;
                        client.Account.Birthday = DateTime.UtcNow;

                        client.Account.Save();
                    }
                    else
                    {
                        result = LoginResult.InvalidUsername;

                        client.LastUsername = username;
                        client.LastPassword = password;
                    }
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckPasswordResult))
            {
                oPacket
                    .WriteInt((int)result)
                    .WriteByte()
                    .WriteByte();

                if (result == LoginResult.Success)
                {
                    oPacket
                        .WriteInt(client.Account.ID)
                        .WriteByte((byte)client.Account.Gender)
                        .WriteBool() // NOTE: Is Admin
                        .WriteByte() // NOTE: Admin byte
                        .WriteBool()
                        .WriteMapleString(client.Account.Username)
                        .WriteByte()
                        .WriteBool()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt()
                        .WriteByte((byte)(MasterServer.Login.RequestPin ? 0 : 2)) // NOTE: 1 seems to not do anything.
                        .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(client.Account.Pic) ? 0 : 1) : 2));
                }

                client.Send(oPacket);
            }
        }

        public static void HandleWorldList(MapleClient client, InPacket iPacket)
        {
            foreach (WorldServer world in MasterServer.Worlds)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
                {
                    oPacket
                        .WriteByte()
                        .WriteMapleString(world.Name)
                        .WriteByte((byte)world.Flag)
                        .WriteMapleString(world.EventMessage)
                        .WriteShort(100) // NOTE: Event EXP rate.
                        .WriteShort(100) // NOTE: Event drop rate.
                        .WriteBool(false) // NOTE: Disables character creation.
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

                    oPacket.WriteShort(); // TODO: Implement world balloons. These are chat bubbles shown on the world select screen.

                    client.Send(oPacket);
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.WorldInformation))
            {
                oPacket.WriteByte(byte.MaxValue);

                client.Send(oPacket);
            }
        }

        public static void HandleWorldStatus(MapleClient client, InPacket iPacket)
        {
            byte worldID = iPacket.ReadByte();

            // NOTE: Unless we want to impose a maximum registered users, this is useless.

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)WorldStatus.Normal);

                client.Send(oPacket);
            }
        }

        public static void HandleWorldSelect(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1); // NOTE: Connection type (GameLaunching, WebStart, etc.).
            byte worldID = iPacket.ReadByte();
            byte channelID = iPacket.ReadByte();
            iPacket.ReadInt(); // NOTE: IPv4 address.

            // TODO: Validate world/channel IDs.

            client.World = worldID;
            client.Channel = channelID;

            List<Character> characters = new List<Character>();

            foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = {0} && WorldID = {1}", client.Account.ID, client.World))
            {
                Character character = new Character((int)datum["ID"], client);

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
                    .WriteByte((byte)(MasterServer.Login.RequestPic ? (string.IsNullOrEmpty(client.Account.Pic) ? 0 : 1) : 2))
                    .WriteInt(client.Account.MaxCharacters);

                client.Send(oPacket);
            }
        }

        public static void HandleCharacterNameCheck(MapleClient client, InPacket iPacket)
        {
            string name = iPacket.ReadMapleString();
            bool unusable = Database.Exists("characters", "Name = {0}", name);

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteMapleString(name)
                    .WriteBool(unusable);

                client.Send(oPacket);
            }
        }

        public static void HandleCharacterCreation(MapleClient client, InPacket iPacket)
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

            Character character = new Character(client: client);

            character.AccountID = client.Account.ID;
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
            character.Map = MasterServer.Worlds[client.World][client.Channel].Maps[jobType == JobType.Cygnus ? 130030000 : jobType == JobType.Explorer ? 10000 : 914000000];
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
                    character.Encode(oPacket);
                }

                client.Send(oPacket);
            }
        }

        // TODO: Proper character deletion with all the necessary checks (cash items, guilds, etcetera). 
        public static void HandleCharacterDeletion(MapleClient client, InPacket iPacket)
        {
            string pic = iPacket.ReadMapleString();
            int characterID = iPacket.ReadInt();

            if (!Database.Exists("characters", "ID = {0} AND AccountID = {1}", characterID, client.Account.ID))
            {
                client.Close();

                return;
            }

            CharacterDeletionResult result;

            if (SHACryptograph.Encrypt(SHAMode.SHA256, pic) == client.Account.Pic || !MasterServer.Login.RequestPic)
            {
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

                client.Send(oPacket);
            }
        }

        public static void HandleCharacterSelection(MapleClient client, InPacket iPacket)
        {
            bool requestPic = false;
            bool registerPic = false;

            if (iPacket.OperationCode == ClientOperationCode.CharacterSelectRequestPic)
            {
                requestPic = true;
            }
            else if (iPacket.OperationCode == ClientOperationCode.CharacterSelectRegisterPic || iPacket.OperationCode == ClientOperationCode.RegisterPicFromVAC)
            {
                registerPic = true;
            }

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

            if (!Database.Exists("characters", "ID = {0} AND AccountID = {1}", characterID, client.Account.ID))
            {
                client.Close();

                return;
            }

            if (client.VAC)
            {
                iPacket.ReadInt(); // NOTE: World ID.
                client.Channel = 0; // TODO: Least loaded channel.
            }

            string macAddresses = iPacket.ReadMapleString(); // TODO: Do something with these.

            if (registerPic)
            {
                iPacket.ReadMapleString();
                pic = iPacket.ReadMapleString();

                if (string.IsNullOrEmpty(client.Account.Pic))
                {
                    client.Account.Pic = SHACryptograph.Encrypt(SHAMode.SHA256, pic);
                    client.Account.Save();
                }
            }

            if (!requestPic || SHACryptograph.Encrypt(SHAMode.SHA256, pic) == client.Account.Pic)
            {
                ChannelServer destinationChannel = MasterServer.Worlds[client.World][client.Channel];

                destinationChannel.Migrations.Add(client.Host, client.Account.ID, characterID);

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.SelectCharacterResult))
                {
                    oPacket
                        .WriteByte()
                        .WriteByte()
                        .WriteBytes(new byte[4] { 127, 0, 0, 1 }) // TODO: HostIP property to channels.
                        .WriteShort(destinationChannel.Port)
                        .WriteInt(characterID)
                        .WriteInt()
                        .WriteByte();

                    client.Send(oPacket);
                }
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.CheckSPWResult))
                {
                    oPacket.WriteByte();

                    client.Send(oPacket);
                }
            }
        }
    }
}
