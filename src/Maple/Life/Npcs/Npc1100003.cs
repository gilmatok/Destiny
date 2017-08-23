using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc1100003 : Npc
    {
        public Npc1100003(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowNextDialog(talker, "Eh, Hello...again. Do you want to leave Ereve and go somewhere else? If so, you've come to the right place. I operate a ferry that goes from Ereve to #bEllinia#k.");

            bool result = await this.ShowYesNoDialog(talker, "Ummm, are you trying to leave Ereve again? I can take you to #bEllinia#k if you want...\r\n\r\nYou'll have to pay a fee of #b1000#k Mesos.");

            if (result)
            {
                if (talker.Meso >= 1000)
                {
                    await this.ShowNextDialog(talker, "I'll take you off of the ride. Talk to me back any time.");

                    talker.Meso -= 1000;
                    talker.ChangeMap(130000000);
                }
                else
                {
                    await this.ShowNextDialog(talker, "I don't think you have enough mesos, double check your inventory.");
                }
            }
        }
    }
}
