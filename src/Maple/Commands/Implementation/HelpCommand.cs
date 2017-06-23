using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    public sealed class HelpCommand : Command
    {
        public override string Name
        {
            get
            {
                return "help";
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
                return GmLevel.None;
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
                caller.Notify("[Help]");

                foreach (Command command in CommandFactory.Commands)
                {
                    if (!(command is HelpCommand))
                    {
                        caller.Notify(string.Format("    !{0} {1}", command.Name, command.Parameters.ClearFormatters()));
                    }
                }
            }
        }
    }
}
