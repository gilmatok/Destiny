using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Commands
{
    public sealed class NoticeCommand : Command
    {
        public override string Name => "notice";
        public override string Parameters => "message";
        public override GmLevel RequiredLevel => GmLevel.SuperGm;

        public override void Execute(Character caller, string[] args)
        {
            //if (args.Length < 1)
            //    this.ShowSyntax(caller);
            //else
            //    MasterServer.Instance.Worlds[caller.Client.World].Notify(this.CombineArgs(args), NoticeType.Notice);
        }
    }
}
