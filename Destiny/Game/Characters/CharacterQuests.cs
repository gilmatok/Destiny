using Destiny.Core.IO;
using Destiny.Utility;

namespace Destiny.Game
{
    public sealed class CharacterQuests
    {
        public Character Parent { get; private set; }

        public CharacterQuests(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteShort() // NOTE: Started.
                .WriteShort(); // NOTE: Completed.
        }
    }
}

