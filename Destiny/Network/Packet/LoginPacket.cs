using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Network.Packet
{
    public static class LoginPacket
    {
        public static byte[] LoginError(LoginResult result)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.CheckPasswordResult))
            {
                oPacket
                    .WriteInt((int)result)
                    .WriteByte()
                    .WriteByte();

                return oPacket.ToArray();
            }
        }

        public static byte[] LoginSuccess(Account account)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.CheckPasswordResult))
            {
                oPacket
                    .WriteInt()
                    .WriteByte()
                    .WriteByte()
                    .WriteInt(account.ID)
                    .WriteByte()
                    .WriteBool()
                    .WriteByte()
                    .WriteByte()
                    .WriteMapleString(account.Username)
                    .WriteByte()
                    .WriteBool()
                    .WriteLong()
                    .WriteLong()
                    .WriteInt()
                    .WriteShort(2);

                return oPacket.ToArray();
            }
        }

        public static byte[] WorldInformation(WorldServer world)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.WorldInformation))
            {
                oPacket
                    .WriteByte(world.ID)
                    .WriteMapleString(world.Name)
                    .WriteByte()
                    .WriteMapleString(string.Empty)
                    .WriteShort(100)
                    .WriteShort(100)
                    .WriteByte()
                    .WriteByte((byte)world.Channels.Length);

                foreach (ChannelServer channel in world.Channels)
                {
                    oPacket
                        .WriteMapleString(string.Format("{0}-{1}", world.Name, channel.ID))
                        .WriteInt()
                        .WriteByte(1)
                        .WriteShort(channel.ID);
                }

                oPacket.WriteShort();

                return oPacket.ToArray();
            }
        }

        public static byte[] WorldEnd()
        {
            using (OutPacket oPacket = new OutPacket(SendOps.WorldInformation))
            {
                oPacket.WriteByte(byte.MaxValue);

                return oPacket.ToArray();
            }
        }

        public static byte[] CheckUserLimitResult(WorldStatus status)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)status);

                return oPacket.ToArray();
            }
        }

        public static byte[] SelectWorldResult(int accountID, byte worldID)
        {
            byte characterCount = (byte)(long)Database.Scalar("SELECT COUNT(*) FROM `characters` WHERE `account_id` = @account_id", new MySqlParameter("@account_id", accountID));

            using (OutPacket oPacket = new OutPacket(SendOps.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte(characterCount);

                if (characterCount > 0)
                {
                    using (DatabaseQuery query = Database.Query("SELECT * FROM `characters` WHERE `account_id` = @account_id AND `world_id` = @world_id", new MySqlParameter("@account_id", accountID), new MySqlParameter("world_id", worldID)))
                    {
                        while (query.NextRow())
                        {
                            HelpPacket.AddCharacterEntry(oPacket, query);
                        }
                    }
                }

                oPacket
                    .WriteByte(2)
                    .WriteInt(3); // TODO: Account specific character creation slots. For now, use default 3.

                return oPacket.ToArray();
            }
        }

        public static byte[] CheckDuplicatedIDResult(string name, bool unusable)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteMapleString(name)
                    .WriteBool(unusable);

                return oPacket.ToArray();
            }
        }

        public static byte[] CreateNewCharacterResult(bool error, DatabaseQuery query)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.CreateNewCharacterResult))
            {
                oPacket.WriteBool(error);

                HelpPacket.AddCharacterEntry(oPacket, query);

                return oPacket.ToArray();
            }
        }

        public static byte[] SelectCharacterResult(short port, int characterID)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.SelectCharacterResult))
            {
                oPacket
                    .WriteByte()
                    .WriteByte()
                    .WriteBytes(new byte[4] { 127, 0, 0, 1 })
                    .WriteShort(port)
                    .WriteInt(characterID)
                    .WriteInt()
                    .WriteByte();

                return oPacket.ToArray();
            }
        }
    }
}
