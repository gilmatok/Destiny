using System;

namespace Destiny
{
    public static class Constants
    {
        public const short Version = 83;
        public const string Patch = "1";
        public const byte Locale = 8;

        public static Random Random = new Random();

        public const char CommandIndiciator = '!';

        public static readonly byte[] RIV = new byte[] { 0x52, 0x61, 0x6A, 0x61 }; //Raja
        public static readonly byte[] SIV = new byte[] { 0x6E, 0x52, 0x30, 0x58 }; //nR0X
    }
}