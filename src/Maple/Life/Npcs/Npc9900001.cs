using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc9900001 : Npc
    {
        public Npc9900001(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            if (talker.IsMaster)
            {
                int choice = await this.ShowChoiceDialog(talker, "What would you like to do?", "Test instances", "Adavnce to Game Master");

                if (choice == 0)
                {
                    string name = "Test";

                    if (talker.Client.Channel.Instances.Contains(name))
                    {
                        await this.ShowOkDialog(talker, "I'm sorry, #h #, but there seems someone in already inside. Please try again in a little bit.");
                    }
                    else
                    {
                        if (talker.Client.Channel.Instances.Create(name, 15 * 1000))
                        {
                            talker.Client.Channel.Instances[name].AddCharacter(talker);

                            talker.ChangeMap(0);
                        }
                        else
                        {
                            await this.ShowOkDialog(talker, "There seems to be something wrong. Please check again later.");
                        }
                    }
                }
                else if (choice == 1)
                {
                    bool advanced = talker.Job == Job.SuperGM;

                    foreach (int loopWizetItem in DataProvider.Items.WizetItemIDs)
                    {
                        if (!talker.Items.Contains(loopWizetItem))
                        {
                            advanced = false;
                        }
                    }

                    if (advanced)
                    {
                        await this.ShowOkDialog(talker, "You have already fully advanced to #bGameMaster#k.");
                    }
                    else
                    {
                        if (talker.Job != Job.SuperGM)
                        {
                            talker.Job = Job.SuperGM;
                        }

                        talker.Strength = talker.Dexterity = talker.Intelligence = talker.Luck = 30000;
                        talker.MaxHealth = talker.MaxMana = 30000;
                        talker.Health = talker.Mana = 30000;

                        // TODO: Max all skills.

                        foreach (int loopWizetItem in DataProvider.Items.WizetItemIDs)
                        {
                            if (!talker.Items.Contains(loopWizetItem))
                            {
                                talker.Items.Add(new Item(loopWizetItem));
                            }
                        }

                        foreach (Item loopItem in talker.Items)
                        {
                            foreach (int loopWizetItem in DataProvider.Items.WizetItemIDs)
                            {
                                if (loopItem.MapleID == loopWizetItem)
                                {
                                    loopItem.Equip();
                                }
                            }
                        }

                        await this.ShowOkDialog(talker, "You have been successfully advanced as a #bGameMaster#k.");
                    }
                }
            }
            else
            {
                bool result = await this.ShowYesNoDialog(talker, "You seem lost. Do you want to be warped out of here?");

                if (result)
                {
                    talker.ChangeMap(100000000);
                }
            }
        }
    }
}
