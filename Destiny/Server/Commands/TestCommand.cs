using Destiny.Game;

namespace Destiny.Server.Commands
{
    public sealed class TestCommand : Command
    {
        public override string Name
        {
            get
            {
                return "test";
            }
        }

        public override string Parameters
        {
            get
            {
                return string.Empty;
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.Intern;
            }
        }

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 0)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                caller.Notify("Hey, the command works!");

                caller.Stats.Level++;
            }
        }
    }
}
