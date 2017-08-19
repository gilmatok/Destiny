using System.Net.Sockets;
using Destiny.Maple.Maps;
using Destiny.Server.Migration;
using Destiny.Maple.Characters;
using System.Collections.Generic;
using Destiny.Core.IO;
using System;
using System.Collections;

namespace Destiny.Server
{
    public sealed class ChannelCharacters : IEnumerable<Character>
    {
        private Dictionary<int, Character> CharactersByID { get; set; }
        private Dictionary<string, Character> CharactersByName { get; set; }

        public ChannelCharacters()
        {
            this.CharactersByID = new Dictionary<int, Character>();
            this.CharactersByName = new Dictionary<string, Character>();
        }

        public void Register(Character character)
        {
            this.CharactersByID.Add(character.ID, character);
            this.CharactersByName.Add(character.Name, character);
        }

        public void Unregister(Character character)
        {
            this.CharactersByID.Remove(character.ID);
            this.CharactersByName.Remove(character.Name);
        }

        public bool Contains(int id)
        {
            return this.CharactersByID.ContainsKey(id);
        }

        public Character GetCharacter(int id)
        {
            Character character = null;

            this.CharactersByID.TryGetValue(id, out character);

            return character;
        }

        public bool Contains(string name)
        {
            return this.CharactersByName.ContainsKey(name);
        }

        public Character GetCharacter(string name)
        {
            Character character = null;

            this.CharactersByName.TryGetValue(name, out character);

            return character;
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return this.CharactersByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.CharactersByID.Values.GetEnumerator();
        }
    }

    public sealed class ChannelServer : ServerBase
    {
        public byte ID { get; private set; }
        public WorldServer World { get; private set; }
        public MigrationRegistery Migrations { get; private set; }
        public ChannelCharacters Characters { get; private set; }
        public MapFactory Maps { get; private set; }

        public ChannelServer(byte id, WorldServer world, short port)
            : base(string.Format("{0}-{1}", world.Name, id), port)
        {
            this.ID = id;
            this.World = world;
            this.Migrations = new MigrationRegistery();
            this.Characters = new ChannelCharacters();
            this.Maps = new MapFactory();
        }

        public void Broadcast(OutPacket oPacket)
        {
            foreach (Character character in this.Characters)
            {
                character.Client.Send(oPacket);
            }
        }

        public void Notify(string text, NoticeType type)
        {
            foreach (Character character in this.Characters)
            {
                character.Notify(text, type);
            }
        }

        protected override void OnClientAccepted(Socket socket)
        {
            MapleClient client = new MapleClient(socket, this)
            {
                WorldID = this.World.ID,
                ChannelID = this.ID
            };

            client.Handshake();
        }
    }
}
