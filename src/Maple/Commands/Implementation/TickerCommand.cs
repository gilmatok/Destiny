using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
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

        }
    }
}
