using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1101001 : Npc
    {
        public Npc1101001(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            // TODO: Gain buff 2022458.

            await this.ShowOkDialog(talker, "Don't stop training. Every ounce of your energy is required to protect the world of Maple....");
        }
    }
}
