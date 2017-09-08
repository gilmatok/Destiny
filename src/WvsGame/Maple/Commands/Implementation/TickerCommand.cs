using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
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
                return "[ message ]";
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
            string message = args.Length > 0 ? args[0] : string.Empty;

            WvsGame.TickerMessage = message;

            //caller.Client.World.Notify(message, NoticeType.Ticker);
        }
    }
}
