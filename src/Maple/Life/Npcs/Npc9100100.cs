using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc9100100 : Npc
    {
        public Npc9100100(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            if (talker.Items.Contains(5451000))
            {
                // NOTE: This is a remote gachapon ticket.
                // When it is used, this npc is called and the
                // gamble process should be immiediate.

                // TODO: Process gachapon gamble.
            }
            else if (talker.Items.Contains(5220000))
            {
                bool result = await this.ShowYesNoDialog(talker, "You may use Gachapon. Would you like to use your Gachapon ticket?");

                if (result)
                {
                    // TODO: Process gachapon gamble.
                }
            }
            else
            {
     choiceSelection:
                int choice = await this.ShowChoiceDialog(talker, "Welcome to the #m" + talker.Map.MapleID + "# Gachapon. How may I help you?", "What is Gachapon?", "Where can you buy Gachapon tickets?");

                if (choice == 0)
                {
                    await this.ShowNextDialog(talker, "Play Gachapon to earn rare scrolls, equipment, chairs, mastery books, and other cool items! All you need is a #bGachapon Ticket#k to be the winner of a random mix of items.");
                    await this.ShowNextPreviousDialog(talker, "You'll find a variety of items from the #m" + talker.Map.MapleID + "# Gachapon, but you'll most likely find several related items and scrolls since #m" + talker.Map.MapleID + "# is known as the town.");

                    goto choiceSelection;
                }
                else if (choice == 1)
                {
                    await this.ShowNextDialog(talker, "Gachapon Tickets are available in the #rCash Shop#k and can be purchased using NX or Maple Points. Click on the red SHOP at the lower right hand corner of the screen to visit the #rCash Shop #kwhere you can purchase tickets.");

                    goto choiceSelection;
                }
            }
        }
    }
}
