namespace Destiny.Security
{
    internal static class BitTools
    {
        public static byte[] MultiplyBytes(byte[] bytes, int count, int mul)
        {
            byte[] value = new byte[count * mul];

            for (int x = 0; x < count * mul; x++)
            {
                value[x] = bytes[x % count];
            }

            return value;
        }

        public static byte RollLeft(byte value, int count)
        {
            int tmp = (int)value & 0xFF;
            tmp = tmp << (count % 8);
            return (byte)((tmp & 0xFF) | (tmp >> 8));
        }

        public static byte RollRight(byte value, int count)
        {
            int tmp = (int)value & 0xFF;
            tmp = (int)((uint)(tmp << 8) >> (count % 8));

            return (byte)((tmp & 0xFF) | (int)((uint)tmp >> 8));
        }
    }
}
