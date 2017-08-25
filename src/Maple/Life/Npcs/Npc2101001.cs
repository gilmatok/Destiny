using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2101001 : Npc
    {
        public Npc2101001(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowNextDialog(talker, "I miss my sister... She's always working at the palace as the servant and I only get to see her on Sundays. The King and Queen are so selfish.");
        }
    }
}
