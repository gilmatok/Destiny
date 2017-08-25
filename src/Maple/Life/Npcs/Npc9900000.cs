using System.Threading.Tasks;
using Destiny.Core.Data;
using Destiny.Maple.Characters;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc9900000 : Npc
    {
        public Npc9900000(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            if (talker.IsMaster)
            {
                int choice = await this.ShowChoiceDialog(talker, "Please select a style category.", "Skin", "Face", "Hair");

                switch (choice)
                {
                    case 0:
                        {
                            choice = await this.ShowStyleRequestDialog(talker, "Please choose a skin color.", 0, 1, 2, 3, 4, 5, 9, 10);

                            talker.Skin = (byte)choice;
                        }
                        break;

                    case 1:
                        {
                            choice = await this.ShowChoiceDialog(talker, "Please select a face operation.", "Style", "Color");

                            if (choice == 0)
                            {
                                int fco = talker.FaceColorOffset;

                                switch (talker.Gender)
                                {
                                    case Gender.Male:
                                        choice = await this.ShowStyleRequestDialog(talker, "Please choose a facial style.",
                                            20000 + fco, 20001 + fco, 20002 + fco, 20003 + fco, 20004 + fco, 20005 + fco,
                                            20006 + fco, 20007 + fco, 20008 + fco, 20009 + fco, 20010 + fco, 20011 + fco,
                                            20012 + fco, 20013 + fco, 20014 + fco, 20016 + fco, 20017 + fco, 20018 + fco,
                                            20019 + fco, 20020 + fco, 20021 + fco, 20022 + fco, 20023 + fco, 20024 + fco,
                                            20026 + fco);
                                        break;

                                    case Gender.Female:
                                        choice = await this.ShowStyleRequestDialog(talker, "Please choose a facial style.",
                                            21000 + fco, 21001 + fco, 21002 + fco, 21003 + fco, 21004 + fco, 21005 + fco,
                                            21006 + fco, 21007 + fco, 21008 + fco, 21009 + fco, 21010 + fco, 21011 + fco,
                                            21012 + fco, 21013 + fco, 21014 + fco, 21016 + fco, 21017 + fco, 21018 + fco,
                                            21019 + fco, 21020 + fco, 21021 + fco, 21022 + fco, 21024 + fco, 21025 + fco);
                                        break;
                                }

                                talker.Face = choice;
                            }
                            else if (choice == 1)
                            {
                                int fso = talker.FaceStyleOffset;

                                choice = await this.ShowStyleRequestDialog(talker, "Please choose a face color.",
                                    fso + 0, fso + 100, fso + 200, fso + 300, fso + 400, fso + 500, fso + 600, fso + 700, fso + 800);

                                talker.Face = choice;
                            }
                        }
                        break;

                    case 2:
                        {
                            choice = await this.ShowChoiceDialog(talker, "Please select a hair operation.", "Style", "Color");

                            if (choice == 0)
                            {
                                int hco = talker.HairColorOffset;

                                switch (talker.Gender)
                                {
                                    case Gender.Male:
                                        choice = await this.ShowStyleRequestDialog(talker, "Please choose a hair style.",
                                            30000 + hco, 30010 + hco, 30020 + hco, 30030 + hco, 30040 + hco, 30050 + hco,
                                            30060 + hco, 30070 + hco, 30080 + hco, 30090 + hco, 30110 + hco, 30120 + hco,
                                            30130 + hco, 30140 + hco, 30150 + hco, 30160 + hco, 30170 + hco, 30180 + hco,
                                            30190 + hco, 30200 + hco, 30210 + hco, 30220 + hco, 30230 + hco, 30240 + hco,
                                            30250 + hco, 30260 + hco, 30270 + hco, 30280 + hco, 30290 + hco, 30300 + hco,
                                            30310 + hco, 30320 + hco, 30330 + hco, 30340 + hco, 30350 + hco, 30360 + hco,
                                            30370 + hco, 30400 + hco, 30410 + hco, 30420 + hco, 30440 + hco, 30450 + hco,
                                            30460 + hco, 30470 + hco, 30480 + hco, 30490 + hco, 30510 + hco, 30520 + hco,
                                            30530 + hco, 30540 + hco, 30550 + hco, 30560 + hco, 30570 + hco, 30580 + hco,
                                            30590 + hco, 30600 + hco, 30610 + hco, 30620 + hco, 30630 + hco, 30640 + hco,
                                            30650 + hco, 30660 + hco, 30670 + hco, 30680 + hco, 30690 + hco, 30700 + hco,
                                            30710 + hco, 30720 + hco, 30730 + hco, 30740 + hco, 30750 + hco, 30760 + hco,
                                            30770 + hco, 30780 + hco, 30790 + hco, 30800 + hco, 30810 + hco, 30820 + hco,
                                            30840 + hco);
                                        break;

                                    case Gender.Female:
                                        choice = await this.ShowStyleRequestDialog(talker, "Please choose a hair style.",
                                            31000 + hco, 31010 + hco, 31020 + hco, 31030 + hco, 31040 + hco, 31050 + hco,
                                            31060 + hco, 31070 + hco, 31080 + hco, 31090 + hco, 31100 + hco, 31110 + hco,
                                            31120 + hco, 31130 + hco, 31140 + hco, 31150 + hco, 31160 + hco, 31170 + hco,
                                            31180 + hco, 31190 + hco, 31200 + hco, 31210 + hco, 31220 + hco, 31230 + hco,
                                            31240 + hco, 31250 + hco, 31260 + hco, 31270 + hco, 31280 + hco, 31290 + hco,
                                            31300 + hco, 31310 + hco, 31320 + hco, 31330 + hco, 31340 + hco, 31350 + hco,
                                            31400 + hco, 31410 + hco, 31420 + hco, 31440 + hco, 31450 + hco, 31460 + hco,
                                            31470 + hco, 31480 + hco, 31490 + hco, 31510 + hco, 31520 + hco, 31530 + hco,
                                            31540 + hco, 31550 + hco, 31560 + hco, 31570 + hco, 31580 + hco, 31590 + hco,
                                            31600 + hco, 31610 + hco, 31620 + hco, 31630 + hco, 31640 + hco, 31650 + hco,
                                            31670 + hco, 31680 + hco, 31690 + hco, 31700 + hco, 31710 + hco, 31720 + hco,
                                            31730 + hco, 31740 + hco, 31750 + hco, 31760 + hco, 31770 + hco, 31780 + hco,
                                            31790 + hco, 31800 + hco, 31810 + hco);
                                        break;
                                }

                                talker.Hair = choice;
                            }
                            else if (choice == 1)
                            {
                                int hso = talker.HairStyleOffset;

                                choice = await this.ShowStyleRequestDialog(talker, "Please choose a hair color.",
                                    hso + 0, hso + 1, hso + 2, hso + 3, hso + 4, hso + 5, hso + 6, hso + 7);

                                talker.Hair = choice;
                            }
                        }
                        break;
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
