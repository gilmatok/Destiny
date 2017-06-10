using Destiny.Game.Characters;
using Destiny.Server;

namespace Destiny.Game.Commands
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
                string message = this.CombineArgs(args);

                MasterServer.Instance.Worlds[caller.Client.World].TickerMessage = message;

                MasterServer.Instance.Worlds[caller.Client.World].Notify(message, NoticeType.Ticker);
            }
        }
    }
}
