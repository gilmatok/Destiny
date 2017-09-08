using System;

namespace Destiny.Network
{
    public class NetworkException : Exception
    {
        public NetworkException() : base("A network error occured.") { }

        public NetworkException(string message) : base(message) { }
    }
}
