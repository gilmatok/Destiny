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
