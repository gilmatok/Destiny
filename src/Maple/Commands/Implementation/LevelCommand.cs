using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class LevelCommand : Command
    {
        public override string Name
        {
            get
            {
                return "level";
            }
        }

        public override string Parameters
        {
            get
            {
                return "level";
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
            if (args.Length != 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                caller.Level = byte.Parse(args[0]);
            }
        }
    }
}
