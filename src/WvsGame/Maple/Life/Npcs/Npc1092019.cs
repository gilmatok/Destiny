using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1092019 : Npc
    {
        public Npc1092019(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "Who are you talking to me? If you're just bored, go bother somebody else.");
        }
    }
}
