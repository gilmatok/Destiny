using Destiny.Maple.Characters;
using Destiny.Maple.Life;

namespace Destiny.Maple.Script
{
    public sealed class NpcScript : ScriptBase
    {
        private Npc mNpc;

        public NpcScript(Npc npc, Character character)
            : base(npc.ScriptPath, character)
        {
            mNpc = npc;
        }
    }
}
