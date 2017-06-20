namespace Destiny.Core.Security
{
    public sealed class ShandaCryptograph
    {
        public static void Encrypt(byte[] data)
        {
            int size = data.Length;
            int j;
            byte a, c;
            for (int i = 0; i < 3; i++)
            {
                a = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[size - j];
                    c = RollLeft(c, 3);
                    c = (byte)(c + j);
                    c ^= a;
                    a = c;
                    c = RollRight(a, j);
                    c ^= 0xFF;
                    c += 0x48;
                    data[size - j] = c;
                }
                a = 0;
                for (j = data.Length; j > 0; j--)
                {
                    c = data[j - 1];
                    c = RollLeft(c, 4);
                    c = (byte)(c + j);
                    c ^= a;
                    a = c;
                    c ^= 0x13;
                    c = RollRight(c, 3);
                    data[j - 1] = c;
                }
            }
        }
        public static void Decrypt(byte[] data)
        {
            int size = data.Length;
            int j;
            byte a, b, c;
            for (int i = 0; i < 3; i++)
            {
                a = 0;
                b = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[j - 1];
                    c = RollLeft(c, 3);
                    c ^= 0x13;
                    a = c;
                    c ^= b;
                    c = (byte)(c - j);
                    c = RollRight(c, 4);
                    b = a;
                    data[j - 1] = c;
                }
                a = 0;
                b = 0;
                for (j = size; j > 0; j--)
                {
                    c = data[size - j];
                    c -= 0x48;
                    c ^= 0xFF;
                    c = RollLeft(c, j);
                    a = c;
                    c ^= b;
                    c = (byte)(c - j);
                    c = RollRight(c, 3);
                    b = a;
                    data[size - j] = c;
                }
            }
        }

        private static byte RollLeft(byte value, int shift)
        {
            uint num = (uint)(value << (shift % 8));
            return (byte)((num & 0xff) | (num >> 8));
        }
        private static byte RollRight(byte value, int shift)
        {
            uint num = (uint)((value << 8) >> (shift % 8));
            return (byte)((num & 0xff) | (num >> 8));
        }
    }
}

