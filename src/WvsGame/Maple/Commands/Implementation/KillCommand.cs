using Destiny.Maple.Characters;
using System.Collections.Generic;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class KillCommand : Command
    {
        public override string Name
        {
            get
            {
                return "kill";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ -map | -character ]";
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
                switch (args[0])
                {
                    case "-map":
                        {
                            foreach (Character character in caller.Map.Characters)
                            {
                                if (character != caller && !character.IsMaster)
                                {
                                    character.Health = 0;
                                }
                            }
                        }
                        break;

                    case "-character":
                        {
                            if (args.Length == 1)
                            {
                                this.ShowSyntax(caller);

                                return;
                            }

                            string targetName = args[1];

                            Character target;

                            try
                            {
                                target = caller.Map.Characters[targetName];
                            }
                            catch (KeyNotFoundException)
                            {
                                caller.Notify("[Command] " + targetName + " cannot be found.");

                                return;
                            }

                            target.Health = 0;
                        }
                        break;

                    default:
                        this.ShowSyntax(caller);
                        break;
                }
            }
        }
    }
}
