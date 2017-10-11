using Destiny.Data;
using Destiny.Maple;
using Destiny.Security;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Network
{
    public sealed class LoginClient : MapleClientHandler
    {
        public long ID { get; private set; }
        public byte World { get; private set; }
        public byte Channel { get; private set; }
        public Account Account { get; private set; }
        public string LastUsername { get; private set; }
        public string LastPassword { get; private set; }
        public string[] MacAddresses { get; private set; }

        public LoginClient(Socket socket)
            : base(socket)
        {
            this.ID = Application.Random.Next();
        }

        protected override bool IsServerAlive
        {
            get
            {
                return WvsLogin.IsAlive;
            }
        }

        protected override void Dispatch(Packet iPacket)
        {
            switch ((ClientOperationCode)iPacket.OperationCode)
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
            }
        }

        private void Login(Packet iPacket)
        {
            string username = iPacket.ReadString();
            string password = iPacket.ReadString();

            if (!username.IsAlphaNumeric())
            {
                this.SendLoginResult(LoginResult.InvalidUsername);
            }
            else
            {
                this.Account = new Account(this);

                try
                {
                    this.Account.Load(username);

                    if (SHACryptograph.Encrypt(SHAMode.SHA512, password + this.Account.Salt) != this.Account.Password)
                    {
                        this.SendLoginResult(LoginResult.InvalidPassword);
                    }
                    else if (this.Account.IsBanned)
                    {
                        this.SendLoginResult(LoginResult.Banned);
                    }
                    else if (!this.Account.EULA)
                    {
                        this.SendLoginResult(LoginResult.EULA);
                    }
                    else // TODO: Add more scenarios (require master IP, check banned IP, check logged in).
                    {
                        this.SendLoginResult(LoginResult.Valid);
                    }
                }
                catch (NoAccountException)
                {
                    if (WvsLogin.AutoRegister && username == this.LastUsername && password == this.LastPassword)
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
                        this.Account.MaxCharacters = WvsLogin.MaxCharacters;

                        this.Account.Save();
                    }
                    else
                    {
                        this.SendLoginResult(LoginResult.InvalidUsername);

                        this.LastUsername = username;
                        this.LastPassword = password;
                    }
                }
            }
        }

        private void SendLoginResult(LoginResult result)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.CheckPasswordResult))
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
                        .WriteString(this.Account.Username)
                        .WriteByte() // NOTE: Unknown.
                        .WriteByte() // NOTE: Quiet ban reason. 
                        .WriteLong() // NOTE: Quiet ban lift date.
                        .WriteDateTime(this.Account.Creation)
                        .WriteInt() // NOTE: Unknown.
                        .WriteByte((byte)(WvsLogin.RequestPin ? 0 : 2)) // NOTE: 1 seems to not do anything.
                        .WriteByte((byte)(WvsLogin.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2));
                }

                this.Send(oPacket);
            }
        }

        private void EULA(Packet iPacket)
        {
            bool accepted = iPacket.ReadBool();

            if (accepted)
            {
                this.Account.EULA = true;

                Datum datum = new Datum("accounts");

                datum["EULA"] = true;

                datum.Update("ID = {0}", this.Account.ID);

                this.SendLoginResult(LoginResult.Valid);
            }
            else
            {
                this.Stop(); // NOTE: I'm pretty sure in the real client it disconnects you if you refuse to accept the EULA.
            }
        }

        private void SetGender(Packet iPacket)
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

                this.SendLoginResult(LoginResult.Valid);
            }
        }

        private void CheckPin(Packet iPacket)
        {
            byte a = iPacket.ReadByte();
            byte b = iPacket.ReadByte();

            PinResult result;

            if (b == 0)
            {
                string pin = iPacket.ReadString();

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

            using (Packet oPacket = new Packet(ServerOperationCode.CheckPinCodeResult))
            {
                oPacket.WriteByte((byte)result);

                this.Send(oPacket);
            }
        }

        private void UpdatePin(Packet iPacket)
        {
            bool procceed = iPacket.ReadBool();
            string pin = iPacket.ReadString();

            if (procceed)
            {
                this.Account.Pin = SHACryptograph.Encrypt(SHAMode.SHA256, pin);

                Datum datum = new Datum("accounts");

                datum["Pin"] = this.Account.Pin;

                datum.Update("ID = {0}", this.Account.ID);

                using (Packet oPacket = new Packet(ServerOperationCode.UpdatePinCodeResult))
                {
                    oPacket.WriteByte(); // NOTE: All the other result types end up in a "trouble logging into the game" message.

                    this.Send(oPacket);
                }
            }
        }

        private void ListWorlds()
        {
            foreach (World world in WvsLogin.Worlds)
            {
                using (Packet oPacket = new Packet(ServerOperationCode.WorldInformation))
                {
                    oPacket
                        .WriteByte(world.ID)
                        .WriteString(world.Name)
                        .WriteByte((byte)world.Flag)
                        .WriteString(world.EventMessage)
                        .WriteShort(100) // NOTE: Event EXP rate
                        .WriteShort(100) // NOTE: Event Drop rate
                        .WriteBool(false) // NOTE: Character creation disable.
                        .WriteByte((byte)world.Count);

                    foreach (Channel channel in world)
                    {
                        oPacket
                            .WriteString($"{world.Name}-{channel.ID}")
                            .WriteInt(channel.Population)
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

                using (Packet oPacket = new Packet(ServerOperationCode.WorldInformation))
                {
                    oPacket.WriteByte(byte.MaxValue);

                    this.Send(oPacket);
                }

                // TODO: Last connected world. Get this from the database. Set the last connected world once you succesfully load a character.
                using (Packet oPacket = new Packet(ServerOperationCode.LastConnectedWorld))
                {
                    oPacket.WriteInt(); // NOTE: World ID.

                    this.Send(oPacket);
                }

                // TODO: Recommended worlds. Get this from configuration.
                using (Packet oPacket = new Packet(ServerOperationCode.RecommendedWorldMessage))
                {
                    oPacket
                        .WriteByte(1) // NOTE: Count.
                        .WriteInt() // NOTE: World ID.
                        .WriteString("Check out Scania! The best world to play - and not because it's the only one available... hehe."); // NOTE: Message.

                    this.Send(oPacket);
                }
            }
        }

        private void InformWorldStatus(Packet iPacket)
        {
            byte worldID = iPacket.ReadByte();

            World world;

            try
            {
                world = WvsLogin.Worlds[worldID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)world.Status);

                this.Send(oPacket);
            }
        }

        private void SelectWorld(Packet iPacket)
        {
            iPacket.ReadByte(); // NOTE: Connection kind (GameLaunching, WebStart, etc.).
            this.World = iPacket.ReadByte();
            this.Channel = iPacket.ReadByte();
            iPacket.ReadBytes(4); // NOTE: IPv4 Address.

            List<byte[]> characters = WvsLogin.CenterConnection.GetCharacters(this.World, this.Account.ID);

            using (Packet oPacket = new Packet(ServerOperationCode.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte((byte)characters.Count);

                foreach (byte[] characterBytes in characters)
                {
                    oPacket.WriteBytes(characterBytes);
                }

                oPacket
                    .WriteByte((byte)(WvsLogin.RequestPic ? (string.IsNullOrEmpty(this.Account.Pic) ? 0 : 1) : 2))
                    .WriteInt(this.Account.MaxCharacters);

                this.Send(oPacket);
            }
        }

        private void ViewAllChar(Packet iPacket)
        {
            //if (this.IsInViewAllChar)
            //{
            //    using (Packet oPacket = new Packet(ServerOperationCode.ViewAllCharResult))
            //    {
            //        oPacket
            //            .WriteByte((byte)VACResult.UnknownError)
            //            .WriteByte();

            //        this.Send(oPacket);
            //    }

            //    return;
            //}

            //this.IsInViewAllChar = true;

            //List<Character> characters = new List<Character>();

            //foreach (Datum datum in new Datums("characters").PopulateWith("ID", "AccountID = {0}", this.Account.ID))
            //{
            //    Character character = new Character((int)datum["ID"], this);

            //    character.Load();

            //    characters.Add(character);
            //}

            //using (Packet oPacket = new Packet(ServerOperationCode.ViewAllCharResult))
            //{
            //    if (characters.Count == 0)
            //    {
            //        oPacket
            //            .WriteByte((byte)VACResult.NoCharacters);
            //    }
            //    else
            //    {
            //        oPacket
            //            .WriteByte((byte)VACResult.SendCount)
            //            .WriteInt(MasterServer.Worlds.Length)
            //            .WriteInt(characters.Count);
            //    }

            //    this.Send(oPacket);
            //}

            //foreach (WorldServer world in MasterServer.Worlds)
            //{
            //    using (Packet oPacket = new Packet(ServerOperationCode.ViewAllCharResult))
            //    {
            //        IEnumerable<Character> worldChars = characters.Where(x => x.WorldID == world.ID);

            //        oPacket
            //            .WriteByte((byte)VACResult.CharInfo)
            //            .WriteByte(world.ID)
            //            .WriteByte((byte)worldChars.Count());

            //        foreach (Character character in worldChars)
            //        {
            //            oPacket.WriteBytes(character.ToByteArray());
            //        }

            //        this.Send(oPacket);
            //    }
            //}
        }

        private void SetViewAllChar(Packet iPacket)
        {
            //this.IsInViewAllChar = iPacket.ReadBool();
        }

        private void CheckCharacterName(Packet iPacket)
        {
            string name = iPacket.ReadString();
            bool unusable = WvsLogin.CenterConnection.IsNameTaken(name);

            using (Packet oPacket = new Packet(ServerOperationCode.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteString(name)
                    .WriteBool(unusable);

                this.Send(oPacket);
            }
        }

        private void CreateCharacter(Packet iPacket)
        {
            byte[] characterData = iPacket.ReadBytes();

            using (Packet outPacket = new Packet(ServerOperationCode.CreateNewCharacterResult))
            {
                outPacket.WriteByte(); // NOTE: 1 for failure. Could be implemented as anti-packet editing.
                outPacket.WriteBytes(WvsLogin.CenterConnection.CreateCharacter(this.World, this.Account.ID, characterData));

                this.Send(outPacket);
            }
        }

        // TODO: Proper character deletion with all the necessary checks (cash items, guilds, etcetera). 
        private void DeleteCharacter(Packet iPacket)
        {
            string pic = iPacket.ReadString();
            int characterID = iPacket.ReadInt();

            CharacterDeletionResult result;

            if (SHACryptograph.Encrypt(SHAMode.SHA256, pic) == this.Account.Pic || !WvsLogin.RequestPic)
            {
                //NOTE: As long as foreign keys are set to cascade, all child entries related to this CharacterID will also be deleted.
                Database.Delete("characters", "ID = {0}", characterID);

                result = CharacterDeletionResult.Valid;
            }
            else
            {
                result = CharacterDeletionResult.InvalidPic;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.DeleteCharacterResult))
            {
                oPacket
                    .WriteInt(characterID)
                    .WriteByte((byte)result);

                this.Send(oPacket);
            }
        }

        private void SelectCharacter(Packet iPacket, bool fromViewAll = false, bool requestPic = false, bool registerPic = false)
        {
            string pic = string.Empty;

            if (requestPic)
            {
                pic = iPacket.ReadString();
            }
            else if (registerPic)
            {
                iPacket.ReadByte();
            }

            int characterID = iPacket.ReadInt();

            //if (this.IsInViewAllChar)
            //{
            //    this.WorldID = (byte)iPacket.ReadInt();
            //    this.ChannelID = 0; // TODO: Least loaded channel.
            //}

            this.MacAddresses = iPacket.ReadString().Split(new char[] { ',', ' ' });

            if (registerPic)
            {
                iPacket.ReadString();
                pic = iPacket.ReadString();

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
                if (!WvsLogin.CenterConnection.Migrate(this.RemoteEndPoint.Address.ToString(), this.Account.ID, characterID))
                {
                    this.Stop();

                    return;
                }

                using (Packet oPacket = new Packet(ServerOperationCode.SelectCharacterResult))
                {
                    oPacket
                        .WriteByte()
                        .WriteByte()
                        .WriteBytes(127, 0, 0, 1)
                        .WriteUShort(WvsLogin.Worlds[this.World][this.Channel].Port)
                        .WriteInt(characterID)
                        .WriteInt()
                        .WriteByte();

                    this.Send(oPacket);
                }
            }
            else
            {
                using (Packet oPacket = new Packet(ServerOperationCode.CheckSPWResult))
                {
                    oPacket.WriteByte();

                    this.Send(oPacket);
                }
            }
        }

    }
}
