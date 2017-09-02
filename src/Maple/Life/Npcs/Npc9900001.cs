using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using System.Collections.Generic;
using System;

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

                    List<Tuple<int, byte>> skills = new List<Tuple<int, byte>>()
                    {
                        new Tuple<int, byte>(8, 1),
                        new Tuple<int, byte>(12, 20),
                        new Tuple<int, byte>(1003, 1),
                        new Tuple<int, byte>(1004, 1),
                        new Tuple<int, byte>(1005, 1),
                        new Tuple<int, byte>(1002, 1),
                        new Tuple<int, byte>(1001, 1),
                        new Tuple<int, byte>(1000, 3)
                    };

                    foreach (Tuple<int, byte> skill in skills)
                    {
                        if (!talker.Skills.Contains(skill.Item1))
                        {
                            talker.Skills.Add(new Skill(skill.Item1));
                        }

                        talker.Skills[skill.Item1].CurrentLevel = skill.Item2;
                    }

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
