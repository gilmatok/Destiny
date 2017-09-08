using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
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
                return "{ -map | -channel | -world } message";
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
            if (args.Length < 2)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                string message = this.CombineArgs(args, 1);

                switch (args[0].ToLower())
                {
                    case "-map":
                        caller.Map.Notify(message);
                        break;

                    case "-channel":
                        //caller.Client.Channel.Notify(message);
                        break;

                    case "-world":
                        //caller.Client.World.Notify(message);
                        break;

                    default:
                        this.ShowSyntax(caller);
                        break;
                }
            }
        }
    }
}
