using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    public sealed class NoticeCommand : Command
    {
        public override string Name
        {
            get
            {
                return "notice";
            }
        }

        public override string Parameters
        {
            get
            {
                return "message";
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
            if (args.Length < 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                //foreach (ChannelServer channel in MasterServer.Channels)
                //{
                //    lock (channel.Clients)
                //    {
                //        foreach (MapleClient client in channel.Clients)
                //        {
                //            client.Character.Notify(args[0], NoticeType.Notice);
                //        }
                //    }
                //}
            }
        }
    }
}
