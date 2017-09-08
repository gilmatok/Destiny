using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2101000 : Npc
    {
        public Npc2101000(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "Just dancing well is not enough for me. I want to do a marvelous brilliant dance!");
        }
    }
}
