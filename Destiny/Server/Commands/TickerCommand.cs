using Destiny.Game;

namespace Destiny.Server.Commands
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
                return "message";
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.SuperGm;
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
                MasterServer.Instance.Worlds[caller.Client.World].Notify(this.CombineArgs(args), NoticeType.Ticker);
            }
        }
    }
}
