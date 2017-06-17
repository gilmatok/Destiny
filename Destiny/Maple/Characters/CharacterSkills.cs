using Destiny.Core.IO;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterSkills
    {
        public Character Parent { get; private set; }

        private List<Skill> mSkills;

        public CharacterSkills(Character parent)
        {
            this.Parent = parent;

            mSkills = new List<Skill>();
        }

        public void Load()
        {

        }

        public void Save()
        {

        }

        public void Encode(OutPacket oPacket)
        {
            oPacket.WriteShort((short)mSkills.Count);

            foreach (Skill skill in mSkills)
            {
                skill.Encode(oPacket);
            }

            oPacket.WriteShort(); // NOTE: Cooldowns.
        }
    }
}

