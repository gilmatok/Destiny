using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class NpcTemplate : Npc
    {
        public NpcTemplate(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {

        }
    }
}
