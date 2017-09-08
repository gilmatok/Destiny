using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using System.Collections.Generic;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class KillMobsCommand : Command
    {
        public override string Name
        {
            get
            {
                return "killmobs";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ - drop ]";
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
            if (args.Length > 1)
            {
                ShowSyntax(caller);
            }
            else
            {
                bool drop = false;

                if (args.Length == 1)
                {
                    if (args[0].ToLower() == "-drop" || args[0].ToLower() == "-drops")
                    {
                        drop = true;
                    }
                    else
                    {
                        this.ShowSyntax(caller);

                        return;
                    }
                }

                lock (caller.Map.Mobs)
                {
                    List<Mob> toKill = new List<Mob>();

                    foreach (Mob loopMob in caller.Map.Mobs)
                    {
                        toKill.Add(loopMob);
                    }

                    foreach (Mob loopMob in toKill)
                    {
                        loopMob.CanDrop = drop;
                        loopMob.Die();
                    }
                }
            }
        }
    }
}
