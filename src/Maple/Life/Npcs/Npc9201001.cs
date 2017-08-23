using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc9201001 : Npc
    {
        public Npc9201001(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            await this.ShowNextDialog(talker, "Nice to meet you! I am Nana the Fairy from Amoria. I am waiting for you to prove your devotion to your loved one by obtaining a Proff of Love! To start, you'll have to venture to Amoria to find my good friend, Moony the Ringmaker. Even if you are not interested in marriage yet, Amoria is open for everyone! Go visit Thomas Swift at Henesys to head to Amoria if you are interested in weddings, be sure to speak with Ames the Wise once you get there!");
        }
    }
}
