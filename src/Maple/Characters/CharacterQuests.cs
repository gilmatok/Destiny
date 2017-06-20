using Destiny.Core.IO;
using Destiny.Utility;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterQuests
    {
        public Character Parent { get; private set; }

        public CharacterQuests(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;
        }

        public void Save()
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

