using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Parameters { get; }
        public abstract GmLevel RequiredLevel { get; }

        public abstract void Execute(Character caller, string[] args);

        public void ShowSyntax(Character caller)
        {
            caller.Notify(string.Format("!{0} {1}", this.Name, this.Parameters));
        }

        public string CombineArgs(string[] args, int start = 0)
        {
            string result = string.Empty;

            for (int i = start; i < args.Length; i++)
            {
                result += args[i] + ' ';
            }

            return result.Trim();
        }

        public string CombineArgs(string[] args, int start, int length)
        {
            string result = string.Empty;

            for (int i = start; i < length; i++)
            {
                result += args[i] + ' ';
            }

            return result.Trim();
        }
    }

}
