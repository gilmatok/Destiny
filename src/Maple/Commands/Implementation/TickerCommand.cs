using Destiny.Maple.Characters;
using Destiny.Network;

namespace Destiny.Maple.Commands.Implementation
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

            //foreach (ChannelServer channel in MasterServer.Channels)
            //{
            //    lock (channel.Clients)
            //    {
            //        foreach (MapleClient client in channel.Clients)
            //        {
            //            client.Character.Notify(message, NoticeType.Ticker);
            //        }
            //    }
            //}
        }
    }
}
