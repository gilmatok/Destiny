using Destiny.Core.IO;
using Destiny.Utility;

namespace Destiny.Game
{
    public sealed class CharacterSkills
    {
        public Character Parent { get; private set; }

        public CharacterSkills(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteShort() // NOTE: Skills.
                .WriteShort(); // NOTE: Cooldowns.
        }
    }
}

