using System;
using System.Security.Cryptography;
using System.Text;

namespace Destiny.Security
{
    public enum SHAMode
    {
        SHA256,
        SHA512
    }

    public static class SHACryptograph
    {
        public static string Encrypt(SHAMode mode, string input)
        {
            switch (mode)
            {
                case SHAMode.SHA256:
                    using (SHA256Managed sha = new SHA256Managed())
                    {
                        return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", "").ToLower();
                    }

                case SHAMode.SHA512:
                    using (SHA512Managed sha = new SHA512Managed())
                    {
                        return BitConverter.ToString(sha.ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", "").ToLower();
                    }

                default:
                    return string.Empty;
            }
        }
    }
}
