using Destiny.Game;

namespace Destiny.Server.Commands
{
    public sealed class MesosCommand : Command
    {
        public override string Name
        {
            get
            {
                return "mesos";
            }
        }

        public override string Parameters
        {
            get
            {
                return "amount";
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.Gm;
            }
        }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                caller.Stats.Mesos += int.Parse(args[0]);
            }
        }
    }
}
