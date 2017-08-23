using System.Threading.Tasks;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Core.IO;
using Destiny.Core.Network;

namespace Destiny.Maple.Life.Npcs
{
    public sealed class Npc2010008 : Npc
    {
        public Npc2010008(Datum datum) : base(datum) { }

        public override async Task Converse(Character talker)
        {
            int choice = await this.ShowChoiceDialog(talker, "What would you like to do?", "Create/Change your Guild Emblem");

            if (choice == 0)
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.GuildResult))
                {
                    oPacket.WriteByte((byte)GuildResult.ChangeEmblem);

                    talker.Client.Send(oPacket);
                }
            }
        }
    }
}
