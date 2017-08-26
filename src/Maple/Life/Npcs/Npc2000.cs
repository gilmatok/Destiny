using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2000 : Npc
    {
        public Npc2000(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            if (talker.Quests.Started.ContainsKey(talker.LastQuest.MapleID))
            {
                await this.ShowNextDialog(talker, "How easy is it to consume the item? Simple, right? You can set a #bhotkey#k on the right bottom slot. Haha you didn't know that! right? Oh, and if you are a beginner, HP will automatically recover itself as time goes by. Well it takes time but this is one of the strategies for the beginners.");
                await this.ShowNextPreviousDialog(talker, "Alright! Now that you have learned alot, I will give you a present. This is a must for your travel in Maple World, so thank me! Please use this under emergency cases!");
                await this.ShowNextPreviousDialog(talker, "Okay, this is all I can teach you.I know it's sad but it is time to say good bye. Well take care if yourself and Good luck my friend!\r\n\r\n#fUI/UIWindow.img/QuestIcon/4/0#\r\n#v2010000# 3 #t2010000#\r\n#v2010009# 3 #t2010009#\r\n\r\n#fUI/UIWindow.img/QuestIcon/8/0# 10 exp");

                talker.Quests.Complete(talker.LastQuest, 0);
                talker.Items.Add(new Item(2010000, 3));
                talker.Items.Add(new Item(2010009, 3));
                talker.Experience += 10;
            }
            else
            {
                if (talker.Gender == Gender.Male)
                {
                    await this.ShowNextDialog(talker, "Hey, Man~ What's up? Haha! I am #p2000# who can teach you adorable new Maplers lots of information.");
                    await this.ShowNextPreviousDialog(talker, "You are asking who made me do this?  Ahahahaha!  Myself!  I wanted to do this and just be kind to you new travellers.");
                }
                else
                {
                    await this.ShowNextDialog(talker, "Hey there, Pretty~ I am #p2000# who teaches you adorable new Maplers lots of information.");
                    await this.ShowNextPreviousDialog(talker, "I know you are busy! Please spare me some time~ I can teach you some useful information! Ahahaha!");
                }

                bool result = await this.ShowYesNoDialog(talker, "So.....  Let me just do this for fun! Abaracadabra~!");

                if (result)
                {
                    talker.Health = (short)(talker.Health / 2);
                    talker.Items.Add(new Item(2010007));
                    talker.Quests.Start(talker.LastQuest, this.MapleID);

                    await this.ShowNextDialog(talker, "Surprised?  If HP becomes 0, then you are in trouble.  Now, I will give you #r#t2010007##k.  Please take it.  You will feel stronger.  Open the Item window and double click to consume.  Hey it's v ery simple to open the Item window.  Just press #bI#k on your keyboard.#I");
                    await this.ShowNextPreviousDialog(talker, "Please take all #t2010007#s that I gave you.  You will be able to see the HP bar increasing.  Please talk to me again when you recover your HP 100%#I");
                }
                else
                {
                    if (talker.Gender == Gender.Male)
                    {
                        await this.ShowNextDialog(talker, "I can't believe an attractive guy like myself, got turned down!");
                    }
                    else
                    {
                        await this.ShowNextDialog(talker, "I can't believe you have just turned down an attractive guy like me!");
                    }
                }
            }
        }
    }
}
