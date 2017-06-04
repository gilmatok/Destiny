using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server.Data
{
    public sealed class BeautyDataProvider
    {
        private List<byte> mSkin;
        private List<int> mFace;
        private List<int> mHair;

        public BeautyDataProvider()
        {
            mSkin = new List<byte>();
            mFace = new List<int>();
            mHair = new List<int>();
        }

        public void Load()
        {
            mSkin.Clear();
            mFace.Clear();
            mHair.Clear();

            int count;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(Config.Instance.Binary, "Beauty.bin"))))
            {
                /*count = reader.ReadInt32();
                while (count-- > 0) mSkin.Add(reader.ReadByte());*/

                count = reader.ReadInt32();
                while (count-- > 0) mFace.Add(reader.ReadInt32());

                count = reader.ReadInt32();
                while (count-- > 0) mHair.Add(reader.ReadInt32());
            }
        }

        public bool IsValidSkin(byte skin)
        {
            return mSkin.Contains(skin);
        }

        public bool IsValidFace(int face)
        {
            return mFace.Contains(face);
        }

        public bool IsValidHair(int hair)
        {
            return mHair.Contains(hair);
        }
    }
}
