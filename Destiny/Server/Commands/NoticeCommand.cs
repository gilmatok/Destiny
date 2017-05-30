using Destiny.Game.Characters;

namespace Destiny.Server.Commands
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
                MasterServer.Instance.Worlds[caller.Client.World].Notify(this.CombineArgs(args), NoticeType.Notice);
            }
        }
    }
}
