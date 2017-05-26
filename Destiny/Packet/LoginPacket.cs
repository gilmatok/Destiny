using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network;
using Destiny.Server;
using System.Collections.Generic;

namespace Destiny.Packet
{
    public static class LoginPacket
    {
        public static byte[] Handshake()
        {
            using (OutPacket oPacket = new OutPacket(14, 16))
            {
                oPacket
                    .WriteShort(Constants.Version)
                    .WriteString(Constants.Patch)
                    .WriteBytes(Constants.RIV)
                    .WriteBytes(Constants.SIV)
                    .WriteByte(Constants.Locale);

                return oPacket.ToArray();
            }
        }

        public static byte[] LoginError(LoginResult result)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.CheckPasswordResult))
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
            using (OutPacket oPacket = new OutPacket(SendOpcode.CheckPasswordResult))
            {
                oPacket
                    .WriteInt()
                    .WriteByte()
                    .WriteByte()
                    .WriteInt(account.AccountId)
                    .WriteByte()
                    .WriteBool()
                    .WriteByte()
                    .WriteByte()
                    .WriteString(account.Username)
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
            using (OutPacket oPacket = new OutPacket(SendOpcode.WorldInformation))
            {
                oPacket
                    .WriteByte(world.ID)
                    .WriteString(world.Name)
                    .WriteByte()
                    .WriteString(string.Empty)
                    .WriteShort(100)
                    .WriteShort(100)
                    .WriteByte()
                    .WriteByte((byte)world.Channels.Length);

                foreach (ChannelServer channel in world.Channels)
                {
                    oPacket
                        .WriteString(string.Format("{0}-{1}", world.Name, channel.ID))
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
            using (OutPacket oPacket = new OutPacket(SendOpcode.WorldInformation))
            {
                oPacket.WriteByte(byte.MaxValue);

                return oPacket.ToArray();
            }
        }

        public static byte[] CheckUserLimitResult(WorldStatus status)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.CheckUserLimitResult))
            {
                oPacket.WriteShort((short)status);

                return oPacket.ToArray();
            }
        }

        public static byte[] SelectWorldResult(List<Character> characters)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.SelectWorldResult))
            {
                oPacket
                    .WriteBool(false)
                    .WriteByte((byte)characters.Count);

                foreach (Character character in characters)
                {
                    HelpPacket.AddCharacterEntry(oPacket, character);
                }

                oPacket
                    .WriteByte(2)
                    .WriteInt(3);

                return oPacket.ToArray();
            }
        }

        public static byte[] CheckDuplicatedIDResult(string name, bool taken)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.CheckDuplicatedIDResult))
            {
                oPacket
                    .WriteString(name)
                    .WriteBool(taken);

                return oPacket.ToArray();
            }
        }

        public static byte[] CreateNewCharacterResult(bool error, Character character)
        {
            using (OutPacket oPacket = new OutPacket(SendOpcode.CreateNewCharacterResult))
            {
                oPacket.WriteBool(error);

                HelpPacket.AddCharacterEntry(oPacket, character);

                return oPacket.ToArray();
            }
        }
    }
}
