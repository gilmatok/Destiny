using Destiny.Maple.Characters;
using Destiny.Server;

namespace Destiny.Maple.Commands
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
                return "message";
            }
        }

        public override GmLevel RequiredLevel
        {
            get
            {
                return GmLevel.SuperGm;
            }
        }

        public override void Execute(Character caller, string[] args)
        {

        }
    }
}
