using Destiny.Collections;
using Destiny.Data;
using Destiny.IO;
using Destiny.Maple;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Network;
using Destiny.Security;
using System;
using System.Linq;
using System.Net;

namespace Destiny.Interoperability
{
    public class CenterServer : ServerHandler<InteroperabilityOperationCode, InteroperabilityOperationCode, BlankCryptograph>
    {
        public CenterServer(IPEndPoint remoteEP, string code) : base(remoteEP, "Center server", new object[] { code }) { }

        protected override bool IsServerAlive
        {
            get
            {
                return WvsGame.IsAlive;
            }
        }

        protected override void StopServer()
        {
            WvsGame.Stop();
        }

        public static void Main()
        {
            try
            {
                WvsGame.CenterConnection = new CenterServer(new IPEndPoint(
                        Settings.GetIPAddress("Center/IP"),
                        Settings.GetInt("Center/Port")),
                        Settings.GetString("Center/SecurityCode"));

                WvsGame.CenterConnection.Loop();
            }
            catch (Exception e)
            {
                Log.Error("Server connection failed: \n{0}", e.Message);

                WvsGame.Stop();
            }
            finally
            {
                WvsGame.CenterConnectionDone.Set();
            }
        }

        protected override void Initialize(params object[] args)
        {
            using (Packet Packet = new Packet(InteroperabilityOperationCode.RegistrationRequest))
            {
                Packet.WriteByte((byte)ServerType.Channel);
                Packet.WriteString((string)args[0]);

                this.Send(Packet);
            }
        }

        protected override void Terminate()
        {
            WvsGame.Stop();
        }

        protected override void Dispatch(Packet inPacket)
        {
            switch ((InteroperabilityOperationCode)inPacket.OperationCode)
            {
                case InteroperabilityOperationCode.RegistrationResponse:
                    this.Register(inPacket);
                    break;

                case InteroperabilityOperationCode.UpdateChannelID:
                    this.UpdateChannelID(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterNameCheckRequest:
                    this.CheckCharacterName(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterEntriesRequest:
                    this.SendCharacters(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterCreationRequest:
                    this.CreateCharacter(inPacket);
                    break;

                case InteroperabilityOperationCode.MigrationResponse:
                    this.Migrate(inPacket);
                    break;

                case InteroperabilityOperationCode.ChannelPortResponse:
                    this.ChannelPortResponse(inPacket);
                    break;
            }
        }

        private void Register(Packet inPacket)
        {
            ServerRegsitrationResponse response = (ServerRegsitrationResponse)inPacket.ReadByte();

            switch (response)
            {
                case ServerRegsitrationResponse.Valid:
                    {
                        WvsGame.WorldID = inPacket.ReadByte();
                        WvsGame.WorldName = inPacket.ReadString();
                        WvsGame.TickerMessage = inPacket.ReadString();
                        WvsGame.ChannelID = inPacket.ReadByte();
                        WvsGame.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, inPacket.ReadUShort());
                        WvsGame.Listen();

                        WvsGame.AllowMultiLeveling = inPacket.ReadBool();
                        Log.Inform("Characters will {0}be able to continuously level-up.", WvsGame.AllowMultiLeveling ? "" : "not ");

                        WvsGame.ExperienceRate = inPacket.ReadInt();
                        WvsGame.QuestExperienceRate = inPacket.ReadInt();
                        WvsGame.PartyQuestExperienceRate = inPacket.ReadInt();
                        WvsGame.MesoRate = inPacket.ReadInt();
                        WvsGame.DropRate = inPacket.ReadInt();
                        Log.Inform("Rates: {0}x / {1}x / {2}x / {3}x / {4}x.",
                            WvsGame.ExperienceRate,
                            WvsGame.QuestExperienceRate,
                            WvsGame.PartyQuestExperienceRate,
                            WvsGame.MesoRate,
                            WvsGame.DropRate);

                        Log.Success("Registered Channel Server ({0} [{1}]-{2}).", WvsGame.WorldName, WvsGame.WorldID, WvsGame.ChannelID);
                    }
                    break;

                default:
                    {
                        Log.Error("Unable to register as Channel Server: {0}", ServerRegistrationResponseResolver.Explain(response));

                        WvsGame.Stop();
                    }
                    break;
            }

            WvsGame.CenterConnectionDone.Set();
        }

        private void UpdateChannelID(Packet inPacket)
        {
            WvsGame.ChannelID = inPacket.ReadByte();
        }

        private void CheckCharacterName(Packet inPacket)
        {
            string name = inPacket.ReadString();
            bool unusable = Database.Exists("characters", "Name = {0}", name);

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterNameCheckResponse))
            {
                outPacket
                    .WriteString(name)
                    .WriteBool(unusable);

                this.Send(outPacket);
            }
        }

        private void SendCharacters(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesResponse))
            {
                outPacket.WriteInt(accountID);

                foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = {0} AND WorldID = {1}", accountID, WvsGame.WorldID))
                {
                    Character character = new Character((int)datum["ID"]);
                    character.Load();

                    byte[] entry = character.ToByteArray();

                    outPacket.WriteByte((byte)entry.Length);
                    outPacket.WriteBytes(entry);
                }

                this.Send(outPacket);
            }
        }

        // TODO: Name & items validation.
        private void CreateCharacter(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            string name = inPacket.ReadString();
            JobType jobType = (JobType)inPacket.ReadInt();
            int face = inPacket.ReadInt();
            int hair = inPacket.ReadInt();
            int hairColor = inPacket.ReadInt();
            byte skin = (byte)inPacket.ReadInt();
            int topID = inPacket.ReadInt();
            int bottomID = inPacket.ReadInt();
            int shoesID = inPacket.ReadInt();
            int weaponID = inPacket.ReadInt();
            Gender gender = (Gender)inPacket.ReadByte();

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

            Character character = new Character();

            character.AccountID = accountID;
            character.WorldID = WvsGame.WorldID;
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
            character.Map = DataProvider.Maps[jobType == JobType.Cygnus ? 130030000 : jobType == JobType.Explorer ? 10000 : 914000000];
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

            character.Save();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationResponse))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteBytes(character.ToByteArray());

                this.Send(outPacket);
            }
        }

        private void ChannelPortResponse(Packet inPacket)
        {
            byte id = inPacket.ReadByte();
            ushort port = inPacket.ReadUShort();

            this.ChannelPortPool.Enqueue(id, port);
        }

        public void UpdatePopulation(int population)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.UpdateChannelPopulation))
            {
                outPacket.WriteInt(population);

                this.Send(outPacket);
            }
        }

        private PendingKeyedQueue<byte, ushort> ChannelPortPool = new PendingKeyedQueue<byte, ushort>();

        public ushort GetChannelPort(byte channelID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.ChannelPortRequest))
            {
                outPacket.WriteByte(channelID);

                this.Send(outPacket);
            }

            return this.ChannelPortPool.Dequeue(channelID);
        }

        private PendingKeyedQueue<string, int> MigrationValidationPool = new PendingKeyedQueue<string, int>();

        public int ValidateMigration(string host, int characterID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.MigrationRequest))
            {
                outPacket
                    .WriteString(host)
                    .WriteInt(characterID);

                this.Send(outPacket);
            }

            return this.MigrationValidationPool.Dequeue(host);
        }

        private void Migrate(Packet inPacket)
        {
            string host = inPacket.ReadString();
            int accountID = inPacket.ReadInt();

            this.MigrationValidationPool.Enqueue(host, accountID);
        }
    }
}
