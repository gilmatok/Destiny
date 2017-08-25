using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc22000 : Npc
    {
        public Npc22000(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            bool result = await this.ShowYesNoDialog(talker, "Take this ship and you'll head off to a bigger continent. For #e150 mesos#n, I'll take you to #bVictoria Island#k. The thing is, once you leave this place, you can't ever come back. What do you think? Do you want to go to Victoria Island?");

            if (result)
            {
                bool hasRecommendationLetter = talker.Items.Contains(4031801);

                if (hasRecommendationLetter)
                {
                    await this.ShowNextDialog(talker, "Okay, now give me 150 mesos... Hey, what's that? Is that the recommendation letter from Lucas, the chief of Amherst? Hey, you should have told me you had this. I, Shanks, recognize greatness when I see one, and since you have been recommended by Lucas, I see that you have a great, great potential as an adventurer. No way would I charge you for this trip!");
                    await this.ShowNextPreviousDialog(talker, "Since you have the recommendation letter, I won't charge you for this. Alright, buckle up, because we're going to head to Victoria Island right now, and it might get a bit turbulent!!");
                }
                else
                {
                    await this.ShowNextDialog(talker, "Bored of this place? Here... Give me #e150 mesos#n first...");

                    if (talker.Level > 6)
                    {
                        if (talker.Meso < 150)
                        {
                            await this.ShowOkDialog(talker, "What? You're telling me you wanted to go without any money? You're one weirdo...");

                            return;
                        }

                        await this.ShowNextDialog(talker, "Awesome! #e150#n mesos accepted! Alright, off to Victoria Island!");
                    }
                    else
                    {
                        await this.ShowOkDialog(talker, "Let's see... I don't think you are strong enough. You'll have to be at least Level 7 to go to Victoria Island.");

                        return;
                    }

                    if (hasRecommendationLetter)
                    {
                        talker.Items.Remove(4031801, 1);
                    }
                    else
                    {
                        talker.Meso -= 150;
                    }

                    talker.ChangeMap(2010000);
                }
            }
            else
            {
                await this.ShowOkDialog(talker, "Hmm... I guess you still have things to do here?");
            }
        }
    }
}
