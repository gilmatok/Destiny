using Destiny.Maple.Characters;
using Destiny.Data;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class SearchCommand : Command
    {
        public override string Name
        {
            get
            {
                return "search";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ -item | -map | -mob | -npc | -quest ] label";
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
            if (args.Length == 0)
            {
                this.ShowSyntax(caller);
            }
            else
            {
                string query;

                if (args[0].StartsWith("-"))
                {
                    query = this.CombineArgs(args, 1).ToLower();
                }
                else
                {
                    query = this.CombineArgs(args).ToLower();
                }

                if (query.Length < 2)
                {
                    caller.Notify("[Command] Please enter at least 2 characters.");
                }
                else
                {
                    bool hasResult = false;

                    caller.Notify("[Results]");

                    using (Database.TemporarySchema(Database.SchemaMCDB))
                    {
                        foreach (Datum datum in new Datums("strings").Populate("`label` LIKE '%{0}%'", query))
                        {
                            caller.Notify(string.Format("   -{0}: {1}", (string)datum["label"], (int)datum["objectid"]));

                            hasResult = true;
                        }
                    }

                    if (!hasResult)
                    {
                        caller.Notify("   No result found.");
                    }
                }
            }
        }
    }
}
