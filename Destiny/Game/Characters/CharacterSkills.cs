using Destiny.Core.IO;
using Destiny.Utility;
using System.Collections.Generic;

namespace Destiny.Game.Characters
{
    public sealed class CharacterSkills
    {
        public Character Parent { get; private set; }

        private List<Skill> mSkills;

        public CharacterSkills(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;

            mSkills = new List<Skill>();

            while (query.NextRow())
            {
                mSkills.Add(new Skill(query));
            }
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

