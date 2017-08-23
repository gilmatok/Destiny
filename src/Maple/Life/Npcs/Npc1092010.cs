using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1092010 : Npc
    {
        public Npc1092010(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "(Scratch scratch...)");
        }
    }
}
