using Destiny.Maple.Characters;
using System;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class JobCommand : Command
    {
        public override string Name
        {
            get
            {
                return "job";
            }
        }

        public override string Parameters
        {
            get
            {
                return "{ id | name}";
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
                try
                {
                    short jobID = short.Parse(args[0]);

                    if (Enum.IsDefined(typeof(Job), jobID))
                    {
                        caller.Job = (Job)jobID;
                    }
                    else
                    {
                        caller.Notify("[Command] Invalid job ID.");
                    }
                }
                catch (FormatException)
                {
                    try
                    {
                        caller.Job = (Job)Enum.Parse(typeof(Job), args[0], true);
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
