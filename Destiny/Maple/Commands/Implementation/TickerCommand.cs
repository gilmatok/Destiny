using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Commands
{
    public sealed class TickerCommand : Command
    {
        public override string Name => "ticker";
        public override string Parameters => "message";
        public override GmLevel RequiredLevel => GmLevel.SuperGm;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length < 1)
                this.ShowSyntax(caller);
            else
            {
                //string message = this.CombineArgs(args);

                //MasterServer.Instance.Worlds[caller.Client.World].TickerMessage = message;
                //MasterServer.Instance.Worlds[caller.Client.World].Notify(message, NoticeType.Ticker);
            }
        }
    }
}
