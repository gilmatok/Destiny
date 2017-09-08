using System;

namespace Destiny.Security
{
    public class CryptographyException : Exception
    {
        public CryptographyException() : base("A cryptography error occured.") { }

        public CryptographyException(string message) : base(message) { }
    }
}
