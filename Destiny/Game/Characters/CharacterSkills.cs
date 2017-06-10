using Destiny.Core.IO;
using Destiny.Utility;
using System.Collections.Generic;

namespace Destiny.Game.Characters
{
    public sealed class CharacterSkills
    {
        public Character Parent { get; private set; }

        private List<Skill> m_skills;

        public CharacterSkills(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;

            m_skills = new List<Skill>();

            while (query.NextRow())
            {
                m_skills.Add(new Skill(query));
            }
        }

        public void Save()
        {

        }

        public void Encode(OutPacket oPacket)
        {
            oPacket.WriteShort((short)m_skills.Count);

            foreach (Skill skill in m_skills)
            {
                skill.Encode(oPacket);
            }

            oPacket.WriteShort(); // NOTE: Cooldowns.
        }
    }
}

