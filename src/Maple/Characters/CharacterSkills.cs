using Destiny.Core.IO;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterSkills : KeyedCollection<int, Skill>
    {
        public Character Parent { get; private set; }

        public CharacterSkills(Character parent)
             : base()
        {
            this.Parent = parent;
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
            oPacket.WriteShort((short)this.Count);

            foreach (Skill loopSkill in this)
            {
                loopSkill.Encode(oPacket);
            }

            oPacket.WriteShort(); // NOTE: Cooldowns.
        }

        protected override int GetKeyForItem(Skill item)
        {
            return item.MapleID;
        }
    }
}

