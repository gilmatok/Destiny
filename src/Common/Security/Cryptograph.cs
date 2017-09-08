using System;

namespace Destiny.Security
{
    public abstract class Cryptograph : IDisposable
    {
        public abstract byte[] Encrypt(byte[] data);
        public abstract byte[] Decrypt(byte[] data);

        protected virtual void CustomDispose() { }

        public void Dispose()
        {
            this.CustomDispose();
        }
    }
}
