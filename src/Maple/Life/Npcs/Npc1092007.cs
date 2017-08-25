using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1092007 : Npc
    {
        public Npc1092007(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowOkDialog(talker, "The Black Magician and his followers. Kyrin and the Crew of Nautilus. \n They'll be chasing one another until one of them doesn't exist, that's for sure.");
        }
    }
}
