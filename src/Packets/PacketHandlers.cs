using Destiny.Core.Data;
using Destiny.Core.IO;
using Destiny.Core.Security;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Commands;
using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.Maple.Maps;
using Destiny.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Destiny.Packets
{
    public static class PacketHandlers
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
                        .WriteString(client.Account.Username)
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
                        .WriteString(world.Name)
                        .WriteByte((byte)world.Flag)
                        .WriteString(world.EventMessage)
                        .WriteShort(100) // NOTE: Event EXP rate.
                        .WriteShort(100) // NOTE: Event drop rate.
                        .WriteBool(false) // NOTE: Disables character creation.
                        .WriteByte((byte)world.Count);

                    foreach (ChannelServer channel in world)
                    {
                        oPacket
                            .WriteString(channel.Label)
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
                    .WriteString(name)
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

        public static void HandleChannelMigrate(MapleClient client, InPacket iPacket)
        {
            int accountID;
            int characterID = iPacket.ReadInt();
            iPacket.Skip(2); // NOTE: Unknown.

            if ((accountID = MasterServer.Worlds[client.World][client.Channel].Migrations.Validate(client.Host, characterID)) == -1)
            {
                client.Close();

                return;
            }

            client.Account = new Account(client);
            client.Account.Load(accountID);

            client.Character = new Character(characterID, client);
            client.Character.Load();

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SetField))
            {
                oPacket
                    .WriteInt(client.Channel)
                    .WriteByte(++client.Character.Portals)
                    .WriteBool(true)
                    .WriteShort(); // NOTE: Floating messages at top corner.

                for (int i = 0; i < 3; i++)
                {
                    oPacket.WriteInt(Constants.Random.Next());
                }

                client.Character.EncodeData(oPacket);

                oPacket.WriteDateTime(DateTime.Now);

                client.Send(oPacket);
            }

            client.Character.IsInitialized = true;

            client.Character.Map.Characters.Add(client.Character);

            client.Character.Keymap.Send();

            client.Character.Memos.Send();
        }

        public static void HandleMapChange(MapleClient client, InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != client.Character.Portals)
            {
                return;
            }

            int destinationID = iPacket.ReadInt();

            switch (destinationID)
            {
                case -1:
                    {
                        string label = iPacket.ReadMapleString();

                        Portal portal;

                        try
                        {
                            portal = client.Character.Map.Portals[label];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        client.Character.ChangeMap(portal.DestinationMapID, portal.Link.ID);
                    }
                    break;
            }
        }

        public static void HandleMovement(MapleClient client, InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != client.Character.Portals)
            {
                return;
            }

            iPacket.ReadInt(); // NOE: Unknown.

            Movements movements = Movements.Decode(iPacket);

            client.Character.Position = movements.Position;
            client.Character.Foothold = movements.Foothold;
            client.Character.Stance = movements.Stance;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserMove))
            {
                oPacket.WriteInt(client.Character.ID);

                movements.Encode(oPacket);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }

            if (client.Character.Foothold == 0)
            {
                // NOTE: Player is floating in the air.
                // GMs might be legitmately in this state due to GM fly.
                // We shouldn't mess with them because they have the tools toget out of falling off the map anyway.

                // TODO: Attempt to find foothold.
                // If none found, check the player fall counter.
                // If it's over 3, reset the player's map.
            }
        }

        public static void HandleMeleeAttack(MapleClient client, InPacket iPacket)
        {
            Attack attack = new Attack(iPacket, AttackType.Melee);

            if (attack.Portals != client.Character.Portals)
            {
                return;
            }

            Skill skill = null;

            if (attack.SkillID > 0)
            {
                skill = client.Character.Skills[attack.SkillID];

                skill.Cast();
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CloseRangeAttack))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteByte((byte)((attack.Targets * 0x10) + attack.Hits))
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte((byte)(attack.SkillID != 0 ? skill.CurrentLevel : 0)); // NOTE: Skill level.

                if (attack.SkillID != 0)
                {
                    oPacket.WriteInt(attack.SkillID);
                }

                oPacket
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte(attack.Display)
                    .WriteByte(attack.Animation)
                    .WriteByte(attack.WeaponSpeed)
                    .WriteByte() // NOTE: Skill mastery.
                    .WriteInt(); // NOTE: Unknown.

                foreach (var target in attack.Damages)
                {
                    oPacket
                        .WriteInt(target.Key)
                        .WriteByte(6);

                    foreach (uint hit in target.Value)
                    {
                        oPacket.WriteUInt(hit);
                    }
                }

                client.Character.Map.Broadcast(oPacket, client.Character);
            }

            foreach (KeyValuePair<int, List<uint>> target in attack.Damages)
            {
                Mob mob;

                try
                {
                    mob = client.Character.Map.Mobs[target.Key];
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }

                mob.IsProvoked = true;
                mob.SwitchController(client.Character);

                foreach (uint hit in target.Value)
                {
                    if (mob.Damage(client.Character, hit))
                    {
                        mob.Die();
                    }
                }
            }
        }

        private const sbyte BumpDamage = -1;
        private const sbyte MapDamage = -2;

        public static void HandleHit(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(4); // NOTE: Ticks.
            sbyte type = (sbyte)iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Elemental type.
            int damage = iPacket.ReadInt();
            bool damageApplied = false;
            bool deadlyAttack = false;
            byte hit = 0;
            byte stance = 0;
            int disease = 0;
            byte level = 0;
            short mpBurn = 0;
            int mobObjectID = 0;
            int mobID = 0;
            int noDamageSkillID = 0;

            if (type != MapDamage)
            {
                mobID = iPacket.ReadInt();
                mobObjectID = iPacket.ReadInt();

                Mob mob;

                try
                {
                    mob = client.Character.Map.Mobs[mobObjectID];
                }
                catch (KeyNotFoundException)
                {
                    return;
                }

                if (mobID != mob.MapleID)
                {
                    return;
                }

                if (type != BumpDamage)
                {
                    // TODO: Get mob attack and apply to disease/level/mpBurn/deadlyAttack.
                }
            }

            hit = iPacket.ReadByte();
            byte reduction = iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Unknown.

            if (reduction != 0)
            {
                // TODO: Return damage (Power Guard).
            }

            if (type == MapDamage)
            {
                level = iPacket.ReadByte();
                disease = iPacket.ReadInt();
            }
            else
            {
                stance = iPacket.ReadByte();

                if (stance > 0)
                {
                    // TODO: Power Stance.
                }
            }

            if (damage == -1)
            {
                // TODO: Validate no damage skills.
            }

            if (disease > 0 && damage != 0)
            {
                // NOTE: Fake/Guardian don't prevent disease.
                // TODO: Add disease buff.
            }

            if (damage > 0)
            {
                // TODO: Check for Meso Guard.
                // TODO: Check for Magic Guard.
                // TODO: Check for Achilles.

                if (!damageApplied)
                {
                    if (deadlyAttack)
                    {
                        // TODO: Deadly attack function.
                    }
                    else
                    {
                        client.Character.Health -= (short)damage;
                    }

                    if (mpBurn > 0)
                    {
                        client.Character.Mana -= (short)mpBurn;
                    }
                }

                // TODO: Apply damage to buffs.
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserHit))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteSByte(type);

                switch (type)
                {
                    case MapDamage:
                        {
                            oPacket
                                .WriteInt(damage)
                                .WriteInt(damage);
                        }
                        break;

                    default:
                        {
                            oPacket
                                .WriteInt(damage) // TODO: ... or PGMR damage.
                                .WriteInt(mobID)
                                .WriteByte(hit)
                                .WriteByte(reduction);

                            if (reduction > 0)
                            {
                                // TODO: PGMR stuff.
                            }

                            oPacket
                                .WriteByte(stance)
                                .WriteInt(damage);

                            if (noDamageSkillID > 0)
                            {
                                oPacket.WriteInt(noDamageSkillID);
                            }
                        }
                        break;
                }

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }

        public static void HandleChat(MapleClient client, InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(client.Character, text);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserChat))
                {
                    oPacket
                        .WriteInt(client.Character.ID)
                        .WriteBool(client.Character.IsGm)
                        .WriteString(text)
                        .WriteBool(shout);

                    client.Character.Map.Broadcast(oPacket);
                }
            }
        }

        public static void HandleFacialExpression(MapleClient client, InPacket iPacket)
        {
            int expressionID = iPacket.ReadInt();

            if (expressionID > 7) // NOTE: Cash facial expression.
            {
                int mapleID = 5159992 + expressionID;

                if (!client.Character.Items.Contains(mapleID))
                {
                    return;
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserEmotion))
            {
                oPacket
                    .WriteInt(client.Character.ID)
                    .WriteInt(expressionID);

                client.Character.Map.Broadcast(oPacket, client.Character);
            }
        }
    }
}
