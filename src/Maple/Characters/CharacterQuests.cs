using Destiny.Core.IO;
using System;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterQuests
    {
        public Character Parent { get; private set; }

        public Dictionary<ushort, Dictionary<int, short>> Started { get; private set; }
        public Dictionary<ushort, DateTime> Completed { get; private set; }

        public CharacterQuests(Character parent)
        {
            this.Parent = parent;

            this.Started = new Dictionary<ushort, Dictionary<int, short>>();
            this.Completed = new Dictionary<ushort, DateTime>();
        }

        public void Load()
        {

        }

        public void Save()
        {

        }

        public void Delete()
        {

        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteShort() // NOTE: Started.
                .WriteShort(); // NOTE: Completed.
        }
    }
}

