using Destiny.Maple.Characters;
using Destiny.Network;
using Destiny.Server;

namespace Destiny.Maple.Commands
{
    public sealed class TickerCommand : Command
    {
        public override string Name
        {
            get
            {
                return "ticker";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ message ]";
            }
        }

        public override bool IsRestricted
        {
            get
            {
                return true;
            }
        }

        public override void Execute(Character caller, string[] args)
        {
            string message = args.Length > 0 ? args[0] : string.Empty;

            foreach (WorldServer world in MasterServer.Worlds)
            {
                foreach (ChannelServer channel in world)
                {
                    foreach (Character character in channel.Players)
                    {
                        character.Notify(message, NoticeType.Ticker);
                    }
                }
            }
        }
    }
}
