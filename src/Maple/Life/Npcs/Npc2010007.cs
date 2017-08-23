using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Core.IO;
using Destiny.Core.Network;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2010007 : Npc
    {
        public Npc2010007(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            int choice = await this.ShowChoiceDialog(talker, "What would you like to do?", "Create a Guild", "Disband your Guild", "Increase your Guild's capacity");

            switch (choice)
            {
                case 0:
                    {
                        if (talker.Guild == null)
                        {
                            bool result = await this.ShowYesNoDialog(talker, "Creating a Guild costs #b1500000 mesos#k, are you sure you want to continue?");

                            if (result)
                            {
                                // TODO: Do we have to check for meso?

                                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                                {
                                    oPacket.WriteByte((byte)GuildResult.Create);

                                    talker.Client.Send(oPacket);
                                }
                            }
                        }
                        else
                        {
                            await this.ShowOkDialog(talker, "You may not create a new Guild while you are in one.");
                        }
                    }
                    break;

                case 1:
                    {
                        bool result = await this.ShowYesNoDialog(talker, "Are you sure you want to disband your Guild? You will not be able to recover it afterward and all your GP will be gone.");

                        if (result)
                        {
                            // TODO: Disband guild.
                        }
                    }
                    break;

                case 2:
                    {
                        if (talker.Guild != null || talker.GuildRank == 1)
                        {
                            bool result = await this.ShowYesNoDialog(talker, "Increasing your Guild capacity by #b5#k costs #b500000 mesos#k, are you sure you want to continue?");

                            if (result)
                            {
                                // TODO: Increase guild's capacity.
                            }
                        }
                        else
                        {
                            await this.ShowOkDialog(talker, "You can only increase your Guild's capacity if you are the leader.");
                        }
                    }
                    break;
            }
        }
    }
}
