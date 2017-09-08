using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2007 : Npc
    {
        public Npc2007(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            bool result = await this.ShowYesNoDialog(talker, "Would you like to skip the tutorials and head straight to Lith Harbor?");

            if (result)
            {
                talker.ChangeMap(104000000);

                // TODO: "Enjoy your trip" message.
                // I'm not sure where it's supposed to go.
            }
        }
    }
}
