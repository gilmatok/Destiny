using Destiny.Core.Collections;
using Destiny.Game.Characters;
using Destiny.Game.Maps;
using System.Collections.Generic;

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
}
