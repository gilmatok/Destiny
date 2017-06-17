using Destiny.Maple.Characters;
using System;

namespace Destiny.Maple.Commands
{
    public sealed class JobCommand : Command
    {
        public override string Name => "job";
        public override string Parameters => "{ id | name }";
        public override GmLevel RequiredLevel => GmLevel.Gm;

        public override void Execute(Character caller, string[] args)
        {
            if (args.Length != 1)
                this.ShowSyntax(caller);
            else
            {
                try
                {
                    short jobID = short.Parse(args[0]);

                    if (Enum.IsDefined(typeof(Job), jobID))
                        caller.Stats.Job = (Job)jobID;
                    else
                        caller.Notify("[Command] Invalid job ID.");
                }
                catch (FormatException)
                {
                    try
                    {
                        caller.Stats.Job = (Job)Enum.Parse(typeof(Job), args[0], true);
                    }
                    catch (ArgumentException)
                    {
                        caller.Notify("[Command] Invalid job name.");
                    }
                }
            }
        }
    }
}
