using Destiny.Security;
using System.Net.Sockets;
using Destiny.Maple;
using System;
using System.Collections.Generic;
using Destiny.IO;

namespace Destiny.Network
{
    public sealed class CenterClient : ClientHandler<InteroperabilityOperationCode, InteroperabilityOperationCode, BlankCryptograph>
    {
        public static string SecurityCode { get; set; }

        public ServerType Type { get; private set; }

        public World World { get; private set; }
        public byte ID { get; set; }
        public ushort Port { get; set; }
        public int Population { get; private set; }

        public CenterClient(Socket socket) : base(socket) { }

        protected override bool IsServerAlive
        {
            get
            {
                return WvsCenter.IsAlive;
            }
        }

        protected override void Prepare(params object[] args)
        {
            base.Prepare(args);
        }

        protected override void Register()
        {
            WvsCenter.Clients.Add(this);
        }

        protected override void Terminate()
        {
            switch (this.Type)
            {
                case ServerType.Login:
                    {
                        WvsCenter.Login = null;

                        Log.Warn("Unregistered Login Server.");
                    }
                    break;

                case ServerType.Channel:
                    {
                        this.World.Remove(this);

                        using (Packet Packet = new Packet(InteroperabilityOperationCode.UpdateChannel))
                        {
                            Packet.WriteByte(this.World.ID);
                            Packet.WriteBool(false);
                            Packet.WriteByte(this.ID);

                            WvsCenter.Login?.Send(Packet);
                        }

                        Log.Warn("Unregistered Channel Server ({0}-{1}).", this.World.Name, this.ID);
                    }
                    break;

                case ServerType.Shop:
                    {
                        this.World.Shop = null;

                        Log.Warn("Unregistered Shop Server ({0}).", this.World.Name);
                        break;
                    }
            }
        }

        protected override void Unregister()
        {
            WvsCenter.Clients.Remove(this);
        }

        protected override void Dispatch(Packet inPacket)
        {
            switch ((InteroperabilityOperationCode)inPacket.OperationCode)
            {
                case InteroperabilityOperationCode.RegistrationRequest:
                    this.Register(inPacket);
                    break;

                case InteroperabilityOperationCode.UpdateChannelPopulation:
                    this.UpdateChannelPopulation(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterNameCheckRequest:
                    this.CharacterNameCheckRequest(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterNameCheckResponse:
                    this.CharacterNameCheckResponse(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterEntriesRequest:
                    this.CharacterEntriesRequest(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterEntriesResponse:
                    this.CharacterEntiresResponse(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterCreationRequest:
                    this.CharacterCreationRequest(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterCreationResponse:
                    this.CharacterCreationResponse(inPacket);
                    break;

                case InteroperabilityOperationCode.MigrationRegisterRequest:
                    this.Migrate(inPacket);
                    break;

                case InteroperabilityOperationCode.MigrationRequest:
                    this.MigrateRequest(inPacket);
                    break;

                case InteroperabilityOperationCode.ChannelPortRequest:
                    this.ChannelPortRequest(inPacket);
                    break;
            }
        }

        private void Register(Packet inPacket)
        {
            ServerType type = (ServerType)inPacket.ReadByte();
            string securityCode = inPacket.ReadString();

            bool valid = true;

            using (Packet Packet = new Packet(InteroperabilityOperationCode.RegistrationResponse))
            {
                if (!Enum.IsDefined(typeof(ServerType), type))
                {
                    Packet.WriteByte((byte)ServerRegsitrationResponse.InvalidType);

                    valid = false;
                }
                else if (securityCode != CenterClient.SecurityCode)
                {
                    Packet.WriteByte((byte)ServerRegsitrationResponse.InvalidCode);

                    valid = false;
                }
                else
                {
                    switch (type)
                    {
                        case ServerType.Login:
                            {
                                if (WvsCenter.Login != null)
                                {
                                    Packet.WriteByte((byte)ServerRegsitrationResponse.Full);

                                    valid = false;
                                }
                                else
                                {
                                    Packet.WriteByte((byte)ServerRegsitrationResponse.Valid);

                                    WvsCenter.Login = this;
                                }
                            }
                            break;

                        case ServerType.Channel:
                        case ServerType.Shop:
                            {
                                World world = WvsCenter.Worlds.Next(type);

                                if (world == null)
                                {
                                    Packet.WriteByte((byte)ServerRegsitrationResponse.Full);

                                    valid = false;
                                }
                                else
                                {
                                    this.World = world;

                                    switch (type)
                                    {
                                        case ServerType.Channel:
                                            this.World.Add(this);
                                            break;

                                        case ServerType.Shop:
                                            this.World.Shop = this;
                                            break;
                                    }

                                    Packet.WriteByte((byte)ServerRegsitrationResponse.Valid);
                                    Packet.WriteByte(this.World.ID);
                                    Packet.WriteString(this.World.Name);

                                    if (type == ServerType.Channel)
                                    {
                                        Packet.WriteString(this.World.TickerMessage);
                                        Packet.WriteByte(this.ID);
                                    }

                                    Packet.WriteUShort(this.Port);

                                    if (type == ServerType.Channel)
                                    {
                                        Packet.WriteBool(this.World.AllowMultiLeveling);
                                        Packet.WriteInt(this.World.ExperienceRate);
                                        Packet.WriteInt(this.World.QuestExperienceRate);
                                        Packet.WriteInt(this.World.PartyQuestExperienceRate);
                                        Packet.WriteInt(this.World.MesoRate);
                                        Packet.WriteInt(this.World.DropRate);
                                    }
                                }
                            }
                            break;
                    }
                }

                this.Send(Packet);
            }

            if (valid)
            {
                this.Type = type;

                switch (type)
                {
                    case ServerType.Login:
                        {
                            byte count = inPacket.ReadByte();

                            for (byte b = 0; b < count; b++)
                            {
                                if (WvsCenter.Worlds.Contains(b))
                                {
                                    continue;
                                }

                                WvsCenter.Worlds.Add(new World(inPacket));
                            }

                            foreach (World loopWorld in WvsCenter.Worlds)
                            {
                                foreach (CenterClient loopChannel in loopWorld)
                                {
                                    using (Packet Packet = new Packet(InteroperabilityOperationCode.UpdateChannel))
                                    {
                                        Packet.WriteByte(loopChannel.World.ID);
                                        Packet.WriteBool(true);
                                        Packet.WriteByte(loopChannel.ID);
                                        Packet.WriteUShort(loopChannel.Port);
                                        Packet.WriteInt(loopChannel.Population);

                                        WvsCenter.Login.Send(Packet);
                                    }
                                }
                            }

                            Log.Success("Registered Login Server.");
                        }
                        break;

                    case ServerType.Channel:
                        {
                            using (Packet Packet = new Packet(InteroperabilityOperationCode.UpdateChannel))
                            {
                                Packet.WriteByte(this.World.ID);
                                Packet.WriteBool(true);
                                Packet.WriteByte(this.ID);
                                Packet.WriteUShort(this.Port);
                                Packet.WriteInt(this.Population);

                                WvsCenter.Login.Send(Packet);
                            }

                            Log.Success("Registered Channel Server ({0}-{1}).", this.World.Name, this.ID);
                        }
                        break;

                    case ServerType.Shop:
                        {
                            Log.Success("Registered Shop Server ({0}).", this.World.Name);
                        }
                        break;
                }
            }
        }

        private void UpdateChannelPopulation(Packet inPacket)
        {
            int population = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.UpdateChannelPopulation))
            {
                outPacket.WriteByte(this.World.ID);
                outPacket.WriteByte(this.ID);
                outPacket.WriteInt(population);

                WvsCenter.Login.Send(outPacket);
            }
        }

        private void CharacterNameCheckRequest(Packet inPacket)
        {
            string name = inPacket.ReadString();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterNameCheckRequest))
            {
                outPacket.WriteString(name);

                WvsCenter.Worlds[0][0].Send(outPacket);
            }
        }

        private void CharacterNameCheckResponse(Packet inPacket)
        {
            string name = inPacket.ReadString();
            bool unusable = inPacket.ReadBool();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterNameCheckResponse))
            {
                outPacket
                    .WriteString(name)
                    .WriteBool(unusable);

                WvsCenter.Login.Send(outPacket);
            }
        }

        private void CharacterEntriesRequest(Packet inPacket)
        {
            byte worldID = inPacket.ReadByte();
            int accountID = inPacket.ReadInt();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesRequest))
            {
                outPacket.WriteInt(accountID);

                WvsCenter.Worlds[worldID].RandomChannel.Send(outPacket);
            }
        }

        private void CharacterEntiresResponse(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            List<byte[]> entires = new List<byte[]>();

            while (inPacket.Remaining > 0)
            {
                entires.Add(inPacket.ReadBytes(inPacket.ReadByte()));
            }

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesResponse))
            {
                outPacket.WriteInt(accountID);

                foreach (var entry in entires)
                {
                    outPacket.WriteByte((byte)entry.Length);
                    outPacket.WriteBytes(entry);
                }

                WvsCenter.Login.Send(outPacket);
            }
        }

        private void CharacterCreationRequest(Packet inPacket)
        {
            byte worldID = inPacket.ReadByte();
            int accountID = inPacket.ReadInt();
            byte[] characterData = inPacket.ReadBytes(inPacket.Remaining);

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationRequest))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteBytes(characterData);

                WvsCenter.Worlds[worldID].RandomChannel.Send(outPacket);
            }
        }

        private void CharacterCreationResponse(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            byte[] characterData = inPacket.ReadBytes(inPacket.Remaining);

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationResponse))
            {
                outPacket.WriteInt(accountID);
                outPacket.WriteBytes(characterData);

                WvsCenter.Login.Send(outPacket);
            }
        }

        private void Migrate(Packet inPacket)
        {
            string host = inPacket.ReadString();
            int accountID = inPacket.ReadInt();
            int characterID = inPacket.ReadInt();

            bool valid;

            if (WvsCenter.Migrations.Contains(host))
            {
                valid = false;
            }
            else
            {
                valid = true;

                WvsCenter.Migrations.Add(new Migration(host, accountID, characterID));
            }

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.MigrationRegisterResponse))
            {
                outPacket
                    .WriteString(host)
                    .WriteBool(valid);

                this.Send(outPacket);
            }
        }

        private void MigrateRequest(Packet inPacket)
        {
            string host = inPacket.ReadString();
            int characterID = inPacket.ReadInt();

            int accountID =  WvsCenter.Migrations.Validate(host, characterID);

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.MigrationResponse))
            {
                outPacket
                    .WriteString(host)
                    .WriteInt(accountID);

                this.Send(outPacket);
            }
        }

        private void ChannelPortRequest(Packet inPacket)
        {
            byte id = inPacket.ReadByte();

            using (Packet outPacket = new Packet(InteroperabilityOperationCode.ChannelPortResponse))
            {
                outPacket.WriteByte(id);
                outPacket.WriteUShort(this.World[id].Port);

                this.Send(outPacket);
            }
        }
    }
}
