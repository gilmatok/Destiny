using Destiny.Collections;
using Destiny.Core.IO;
using Destiny.Maple;
using Destiny.Network;
using Destiny.Security;
using System;
using System.Collections.Generic;
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
                return WvsLogin.IsAlive;
            }
        }

        protected override void StopServer()
        {
            WvsLogin.Stop();
        }

        public static void Main()
        {
            try
            {
                WvsLogin.CenterConnection = new CenterServer(new IPEndPoint(
                        Settings.GetIPAddress("Center/IP"),
                        Settings.GetInt("Center/Port")),
                        Settings.GetString("Center/SecurityCode"));

                WvsLogin.CenterConnection.Loop();
            }
            catch (Exception e)
            {
                Log.Error("Server connection failed: \n{0}", e.Message);

                WvsLogin.Stop();
            }
            finally
            {
                WvsLogin.CenterConnectionDone.Set();
            }
        }

        protected override void Initialize(params object[] args)
        {
            using (Packet Packet = new Packet(InteroperabilityOperationCode.RegistrationRequest))
            {
                Packet.WriteByte((byte)ServerType.Login);
                Packet.WriteString((string)args[0]);
                Packet.WriteByte((byte)WvsLogin.Worlds.Count);

                foreach (World loopWorld in WvsLogin.Worlds)
                {
                    Packet.WriteByte(loopWorld.ID);
                    Packet.WriteString(loopWorld.Name);
                    Packet.WriteUShort(loopWorld.Port);
                    Packet.WriteUShort(loopWorld.ShopPort);
                    Packet.WriteByte(loopWorld.Channels);
                    Packet.WriteString(loopWorld.TickerMessage);
                    Packet.WriteBool(loopWorld.AllowMultiLeveling);
                    Packet.WriteInt(loopWorld.ExperienceRate);
                    Packet.WriteInt(loopWorld.QuestExperienceRate);
                    Packet.WriteInt(loopWorld.PartyQuestExperienceRate);
                    Packet.WriteInt(loopWorld.MesoRate);
                    Packet.WriteInt(loopWorld.DropRate);
                }

                this.Send(Packet);
            }
        }

        protected override void Terminate()
        {
            WvsLogin.Stop();
        }

        protected override void Dispatch(Packet inPacket)
        {
            switch ((InteroperabilityOperationCode)inPacket.OperationCode)
            {
                case InteroperabilityOperationCode.RegistrationResponse:
                    this.Register(inPacket);
                    break;

                case InteroperabilityOperationCode.UpdateChannel:
                    this.UpdateChannel(inPacket);
                    break;

                case InteroperabilityOperationCode.UpdateChannelPopulation:
                    this.UpdateChannelPopulation(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterEntriesResponse:
                    this.GetCharacters(inPacket);
                    break;

                case InteroperabilityOperationCode.CharacterCreationResponse:
                    this.CreateCharacter(inPacket);
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
                        WvsLogin.Listen();
                        WvsLogin.CenterConnectionDone.Set();

                        Log.Success("Registered Login Server.");
                    }
                    break;

                default:
                    {
                        Log.Error(ServerRegistrationResponseResolver.Explain(response));

                        WvsLogin.Stop();
                    }
                    break;
            }
        }
        
        private void UpdateChannel(Packet inPacket)
        {
            byte worldID = inPacket.ReadByte();
            bool add = inPacket.ReadBool();

            World world = WvsLogin.Worlds[worldID];

            if (add)
            {
                world.Add(new Channel(inPacket));
            }
            else
            {
                byte channelID = inPacket.ReadByte();

                world.Remove(channelID);
            }
        }

        private void UpdateChannelPopulation(Packet inPacket)
        {
            byte worldID = inPacket.ReadByte();
            byte channelID = inPacket.ReadByte();
            int population = inPacket.ReadInt();

            WvsLogin.Worlds[worldID][channelID].Population = population;
        }

        private PendingKeyedQueue<int, List<byte[]>> CharacterEntriesPool = new PendingKeyedQueue<int, List<byte[]>>();

        private void GetCharacters(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();

            List<byte[]> entires = new List<byte[]>();

            while (inPacket.Remaining > 0)
            {
                entires.Add(inPacket.ReadBytes(inPacket.ReadByte()));
            }

            this.CharacterEntriesPool.Enqueue(accountID, entires);
        }

        public List<byte[]> GetCharacters(byte worldID, int accountID)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterEntriesRequest))
            {
                outPacket.WriteByte(worldID);
                outPacket.WriteInt(accountID);

                this.Send(outPacket);
            }

            return this.CharacterEntriesPool.Dequeue(accountID);
        }

        private PendingKeyedQueue<int, byte[]> CharacterCreationPool = new PendingKeyedQueue<int, byte[]>();

        private void CreateCharacter(Packet inPacket)
        {
            int accountID = inPacket.ReadInt();
            byte[] characterData = inPacket.ReadBytes(inPacket.Remaining);

            this.CharacterCreationPool.Enqueue(accountID, characterData);
        }

        public byte[] CreateCharacter(byte worldID, int accountID, byte[] characterData)
        {
            using (Packet outPacket = new Packet(InteroperabilityOperationCode.CharacterCreationRequest))
            {
                outPacket.WriteByte(worldID);
                outPacket.WriteInt(accountID);
                outPacket.WriteBytes(characterData);

                this.Send(outPacket);
            }

            return this.CharacterCreationPool.Dequeue(accountID);
        }
    }
}
