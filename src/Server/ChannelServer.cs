using System;
using System.Net.Sockets;
using Destiny.Maple.Maps;
using Destiny.Network;
using Destiny.Handlers;
using Destiny.Core.Network;

namespace Destiny.Server
{
    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public byte WorldID { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public MapFactory Maps { get; private set; }
        public PlayerStorage Players { get; private set; }

        public ChannelServer(byte id, byte worldID, short port)
            : base(string.Format("{0}-{1}", "Scania", id), port)
        {
            this.ID = id;
            this.WorldID = worldID;
            this.Migrations = new MigrationRegistery();
            this.Maps = new MapFactory();
            this.Players = new PlayerStorage();
        }

        protected override void SpawnHandlers()
        {
            mProcessor.Add(ClientOperationCode.CharacterLoad, ServerHandlers.HandleChannelMigrate);
            mProcessor.Add(ClientOperationCode.MapChange, PlayerHandlers.HandleMapChange);
            mProcessor.Add(ClientOperationCode.PlayerMovement, PlayerHandlers.HandleMovement);
            mProcessor.Add(ClientOperationCode.MeleeAttack, PlayerHandlers.HandleMeleeAttack);
            mProcessor.Add(ClientOperationCode.TakeDamage, PlayerHandlers.HandleHit);
            mProcessor.Add(ClientOperationCode.PlayerChat, PlayerHandlers.HandleChat);
            mProcessor.Add(ClientOperationCode.FaceExpression, PlayerHandlers.HandleFacialExpression);
        }

        protected override void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, this, mProcessor)
            {
                World = this.WorldID,
                Channel = this.ID
            };

            client.Handshake();
        }
    }
}
