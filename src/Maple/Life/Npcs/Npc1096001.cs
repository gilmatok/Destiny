using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1096001 : Npc
    {
        public Npc1096001(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "Will I ever finish cleaning? This ship is just too big...");
        }
    }
}
