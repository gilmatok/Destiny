using System;
using System.Security.Cryptography;
using System.Text;

// CREDITS: Loki
namespace Destiny.Core.Security
{
    public static class HashGenerator
    {
        public static string GenerateMD5(string input = null)
        {
            if (input == null)
            {
                input = Constants.Random.Next().ToString();
            }

            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(input))).Replace("-", "").ToLower();
        }
    }
}
