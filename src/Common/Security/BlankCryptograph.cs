namespace Destiny.Security
{
    public class BlankCryptograph : Cryptograph
    {
        public override byte[] Encrypt(byte[] data)
        {
            return data;
        }

        public override byte[] Decrypt(byte[] data)
        {
            return data;
        }
    }
}
