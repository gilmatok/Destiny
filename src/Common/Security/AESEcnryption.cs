using System.Security.Cryptography;

namespace Destiny.Core.Security
{
    public sealed class AESEncryption
    {
        private static readonly byte[] sUserKey = new byte[32]
        {
            0x13, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0xB4, 0x00, 0x00, 0x00,
            0x1B, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0x00, 0x52, 0x00, 0x00, 0x00
        };

        private static ICryptoTransform sTransformer;

        static AESEncryption()
        {
            RijndaelManaged aes = new RijndaelManaged()
            {
                Key = sUserKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using (aes)
            {
                sTransformer = aes.CreateEncryptor();
            }
        }

        public static void Transform(byte[] data, byte[] IV)
        {
            byte[] morphKey = new byte[16];
            int remaining = data.Length;
            int start = 0;
            int length = 0x5B0;

            while (remaining > 0)
            {
                for (int i = 0; i < 16; i++)
                    morphKey[i] = IV[i % 4];

                if (remaining < length)
                    length = remaining;

                for (int index = start; index < (start + length); index++)
                {
                    if ((index - start) % 16 == 0)
                        sTransformer.TransformBlock(morphKey, 0, 16, morphKey, 0);

                    data[index] ^= morphKey[(index - start) % 16];
                }

                start += length;
                remaining -= length;
                length = 0x5B4;
            }
        }
    }
}
