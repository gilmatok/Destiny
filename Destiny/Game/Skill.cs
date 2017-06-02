using Destiny.Core.IO;
using Destiny.Utility;
using System;

namespace Destiny.Game
{
    public sealed class Skill
    {
        public int MapleID { get; private set; }
        public byte Level { get; private set; }
        public byte MaxLevel { get; private set; }
        public DateTime Expiration { get; private set; }

        public Skill(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("maple_id");
            this.Level = query.GetByte("level");
            this.MaxLevel = query.GetByte("max_level");
            this.Expiration = query.GetDateTime("expiration");
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.MapleID)
                .WriteByte(this.Level);

            if (false) // TODO: Check if skill is fourth-job related.
            {
                oPacket.WriteInt(this.MaxLevel);
            }
        }
    }
}
