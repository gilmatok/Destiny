using Destiny.Maple.Characters;

namespace Destiny.Maple.Commands.Implementation
{
    public sealed class ShopCommand : Command
    {
        public override string Name
        {
            get
            {
                return "shop";
            }
        }

        public override string Parameters
        {
            get
            {
                return "[ gear | scrolls | nx | face | ring | chair | mega | pet ]";
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
                int shopID = -1;

                if (args[0] == "gear") shopID = 9999999;
                else if (args[0] == "scrolls") shopID = 9999998;
                else if (args[0] == "nx") shopID = 9999997;
                else if (args[0] == "face") shopID = 9999996;
                else if (args[0] == "ring") shopID = 9999995;
                else if (args[0] == "chair") shopID = 9999994;
                else if (args[0] == "mega") shopID = 9999993;
                else if (args[0] == "pet") shopID = 9999992;

                if (shopID == -1)
                {
                    this.ShowSyntax(caller);

                    return;
                }

                // TODO: Shop the desired shop.
                // As we assign shops to NPCs, we need to modify the MCDB values
                // so each shop will be matched to a different exclusive NPC.
            }
        }
    }
}
