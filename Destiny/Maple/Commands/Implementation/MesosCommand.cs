using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands
{
    public sealed class MesosCommand : Command
    {
        public override string Name => "mesos";
        public override string Parameters => "amount";
        public override GmLevel RequiredLevel => GmLevel.Gm;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
                this.ShowSyntax(caller);
            else
                caller.Stats.Mesos += int.Parse(args[0]);
        }
    }
}
