using Destiny.Core.Collections;
using Destiny.Core.Network;
using Destiny.Game.Characters;
using Destiny.Game.Maps;
using Destiny.Network;
using Destiny.Network.Handler;
using Destiny.Network.Packet;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Destiny.Server
{
    public sealed class ChannelCharacters : EnumerationHelper<int, Character>
    {
        public byte World { get; private set; }
        public byte Channel { get; private set; }

        public ChannelCharacters(byte world, byte channel)
        {
            this.World = world;
            this.Channel = channel;
        }

        public override IEnumerator<Character> GetEnumerator()
        {
            foreach (Map map in MasterServer.Instance.Worlds[this.World].Channels[this.Channel].Maps.Values)
            {
                foreach (Character character in map.Characters)
                {
                    yield return character;
                }
            }
        }

        public override int GetKeyForObject(Character item)
        {
            return item.ID;
        }
    }

    public sealed class ChannelServer
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public short Port { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }
        public ChannelCharacters Characters { get; private set; }

        private Acceptor mAcceptor;
        private List<MapleClient> mClients;
        private PacketProcessor mProcessor;

        public ChannelServer(byte id, byte worldID, short port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Port = port;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory(this.WorldID, this.ID);
            this.Characters = new ChannelCharacters(this.WorldID, this.ID);

            mAcceptor = new Acceptor(this.Port);
            mAcceptor.OnClientAccepted = this.OnClientAccepted;

            mClients = new List<MapleClient>();

            this.SpawnHandlers();
        }

        public void Start()
        {
            mAcceptor.Start();
        }

        public void Stop()
        {
            mAcceptor.Stop();

            foreach (MapleClient client in mClients)
            {
                client.Close();
            }
        }

        private void SpawnHandlers()
        {
            mProcessor = new PacketProcessor("Channel");

            mProcessor.Add(RecvOps.MigrateIn, ServerHandler.HandleMigrateChannel);
            mProcessor.Add(RecvOps.TransferFieldRequest, UserHandler.OnTransferFieldRequest);
            mProcessor.Add(RecvOps.ChangeChannel, ServerHandler.HandleChangeChannel);
            mProcessor.Add(RecvOps.EnterCashShop, ServerHandler.HandleCashShop);
            mProcessor.Add(RecvOps.EnterMts, ServerHandler.HandleMTS);
            mProcessor.Add(RecvOps.UserChat, UserHandler.OnChat);
            mProcessor.Add(RecvOps.UserMove, UserHandler.OnMove);
            mProcessor.Add(RecvOps.ItemMove, InventoryHandler.HandleItemMove);
            mProcessor.Add(RecvOps.NpcMove, NpcHandler.HandleNpcMove);
            mProcessor.Add(RecvOps.MobMove, MobHandler.OnMobMove);
        }

        private void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, mProcessor, mClients.Remove)
            {
                World = this.WorldID,
                Channel = this.ID
            };

            mClients.Add(client);

            client.SendRaw(CommonPacket.Handshake());

            Logger.Write(LogLevel.Connection, "[Channel] Accepted client {0}.", client.Host);
        }

        public void Notify(string message, NoticeType type)
        {
            foreach (Character character in this.Characters)
            {
                character.Notify(message, type);
            }
        }
    }
}
