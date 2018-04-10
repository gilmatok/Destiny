using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
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

        public override bool IsRestricted
        {
            get
            {
                return true;
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
               
            }
        }
    }
}
