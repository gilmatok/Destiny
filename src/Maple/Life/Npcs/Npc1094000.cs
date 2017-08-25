using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1094000 : Npc
    {
        public Npc1094000(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "I need to keep my eyes wide open to look for the enemy although my sea gull friends help me out so it's not all that bad.");
        }
    }
}
