using Destiny.Data;
using Destiny.IO;
using System.Collections.Generic;

namespace Destiny.Maple.Data
{
    public sealed class AvailableStyles
    {
        public List<byte> Skins { get; private set; }
        public List<int> MaleHairs { get; private set; }
        public List<int> FemaleHairs { get; private set; }
        public List<int> MaleFaces { get; private set; }
        public List<int> FemaleFaces { get; private set; }

        public AvailableStyles()
        {
            this.Skins = new List<byte>();
            this.MaleHairs = new List<int>();
            this.FemaleHairs = new List<int>();
            this.MaleFaces = new List<int>();
            this.FemaleFaces = new List<int>();

            using (Log.Load("Styles"))
            {
                foreach (Datum datum in new Datums("character_skin_data", Database.SchemaMCDB).Populate())
                {
                    this.Skins.Add((byte)(sbyte)datum["skinid"]);
                }

                foreach (Datum datum in new Datums("character_hair_data", Database.SchemaMCDB).Populate())
                {
                    switch ((string)datum["gender"])
                    {
                        case "male":
                            this.MaleHairs.Add((int)datum["hairid"]);
                            break;

                        case "female":
                            this.FemaleHairs.Add((int)datum["hairid"]);
                            break;
                    }
                }

                foreach (Datum datum in new Datums("character_face_data", Database.SchemaMCDB).Populate())
                {
                    switch ((string)datum["gender"])
                    {
                        case "male":
                            this.MaleFaces.Add((int)datum["faceid"]);
                            break;

                        case "female":
                            this.FemaleFaces.Add((int)datum["faceid"]);
                            break;
                    }
                }
            }
        }
    }
}
