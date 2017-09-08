using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class SayCommand : Command
    {
        public override string Name
        {
            get
            {
                return "say";
            }
        }

        public override string Parameters
        {
            get
            {
                return "message";
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
            if (args.Length < 1)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                string message = this.CombineArgs(args);

                //caller.Client.World.Notify(string.Format("{0}: {1}", caller.Name, message));
            }
        }
    }
}
