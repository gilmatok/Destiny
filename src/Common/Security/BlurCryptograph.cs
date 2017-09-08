namespace Destiny.Security
{
    internal static class BlurCryptograph
    {
        public static void Encrypt(byte[] data)
        {
            for (int j = 0; j < 6; j++)
            {
                byte remember = 0;
                byte length = (byte)data.Length;

                if (j % 2 == 0)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte current = data[i];
                        current = BitTools.RollLeft(current, 3);
                        current += length;
                        current ^= remember;
                        remember = current;
                        current = BitTools.RollRight(current, length);
                        current = (byte)~current;
                        current += 0x48;
                        length--;
                        data[i] = current;
                    }
                }
                else
                {
                    for (int i = data.Length - 1; i >= 0; i--)
                    {
                        byte current = data[i];
                        current = BitTools.RollLeft(current, 4);
                        current += length;
                        current ^= remember;
                        remember = current;
                        current ^= 0x13;
                        current = BitTools.RollRight(current, 3);
                        length--;
                        data[i] = current;
                    }
                }
            }
        }

        public static void Decrypt(byte[] data)
        {
            for (int j = 1; j <= 6; j++)
            {
                byte remember = 0;
                byte length = (byte)data.Length;
                byte nextRemember = 0;

                if (j % 2 == 0)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        byte current = data[i];
                        current -= 0x48;
                        current = (byte)~current;

                        current = BitTools.RollLeft(current, length);

                        nextRemember = current;
                        current ^= remember;
                        remember = nextRemember;
                        current -= length;

                        current = BitTools.RollRight(current, 3);

                        data[i] = current;
                        length--;
                    }
                }
                else
                {
                    for (int i = data.Length - 1; i >= 0; i--)
                    {
                        byte current = data[i];

                        current = BitTools.RollLeft(current, 3);

                        current ^= 0x13;
                        nextRemember = current;
                        current ^= remember;
                        remember = nextRemember;
                        current -= length;

                        current = BitTools.RollRight(current, 4);

                        data[i] = current;
                        length--;
                    }
                }
            }
        }
    }
}
