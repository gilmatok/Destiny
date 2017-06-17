using Destiny.Core.IO;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterQuests
    {
        public Character Parent { get; private set; }

        public CharacterQuests(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {

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

