using Destiny.Data;
using Destiny.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Destiny.Constants;

namespace Destiny.Maple.Data
{
    public sealed class CreationData
    {
        public List<string> ForbiddenNames { get; private set; }

        public List<Tuple<CharacterConstants.JobType, byte>> MaleSkins { get; private set; }
        public List<Tuple<CharacterConstants.JobType, byte>> FemaleSkins { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleFaces { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleFaces { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleHairs { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleHairs { get; private set; }
        public List<Tuple<CharacterConstants.JobType, byte>> MaleHairColors { get; private set; }
        public List<Tuple<CharacterConstants.JobType, byte>> FemaleHairColors { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleTops { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleTops { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleBottoms { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleBottoms { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleShoes { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleShoes { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> MaleWeapons { get; private set; }
        public List<Tuple<CharacterConstants.JobType, int>> FemaleWeapons { get; private set; }

        public CreationData()
        {
            this.MaleSkins = new List<Tuple<CharacterConstants.JobType, byte>>();
            this.FemaleSkins = new List<Tuple<CharacterConstants.JobType, byte>>();
            this.MaleFaces = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleFaces = new List<Tuple<CharacterConstants.JobType, int>>();
            this.MaleHairs = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleHairs = new List<Tuple<CharacterConstants.JobType, int>>();
            this.MaleHairColors = new List<Tuple<CharacterConstants.JobType, byte>>();
            this.FemaleHairColors = new List<Tuple<CharacterConstants.JobType, byte>>();
            this.MaleTops = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleTops = new List<Tuple<CharacterConstants.JobType, int>>();
            this.MaleBottoms = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleBottoms = new List<Tuple<CharacterConstants.JobType, int>>();
            this.MaleShoes = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleShoes = new List<Tuple<CharacterConstants.JobType, int>>();
            this.MaleWeapons = new List<Tuple<CharacterConstants.JobType, int>>();
            this.FemaleWeapons = new List<Tuple<CharacterConstants.JobType, int>>();

            using (Log.Load("Character Creation Data"))
            {
                this.ForbiddenNames = new Datums("character_forbidden_names").Populate().Select(x => (string)x["forbidden_name"]).ToList();

                foreach (Datum datum in new Datums("character_creation_data").Populate())
                {
                    string gender = (string)datum["gender"];
                    string charType = (string)datum["character_type"];
                    CharacterConstants.JobType jobType = charType == "aran" ? CharacterConstants.JobType.Aran : (charType == "cygnus" ? CharacterConstants.JobType.Cygnus : CharacterConstants.JobType.Explorer);

                    switch ((string)datum["object_type"])
                    {
                        case "skin":
                            if(gender == "male" || gender == "both")
                                this.MaleSkins.Add(new Tuple<CharacterConstants.JobType, byte>(jobType, (byte)(int)datum["objectid"]));
                            if(gender == "female" || gender == "both")
                                this.FemaleSkins.Add(new Tuple<CharacterConstants.JobType, byte>(jobType, (byte)(int)datum["objectid"]));
                            break;
                        case "face":
                            if (gender == "male" || gender == "both")
                                this.MaleFaces.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleFaces.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                        case "hair":
                            if (gender == "male" || gender == "both")
                                this.MaleHairs.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleHairs.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                        case "haircolor":
                            if (gender == "male" || gender == "both")
                                this.MaleHairColors.Add(new Tuple<CharacterConstants.JobType, byte>(jobType, (byte)(int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleHairColors.Add(new Tuple<CharacterConstants.JobType, byte>(jobType, (byte)(int)datum["objectid"]));
                            break;
                        case "top":
                            if (gender == "male" || gender == "both")
                                this.MaleTops.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleTops.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                        case "bottom":
                            if (gender == "male" || gender == "both")
                                this.MaleBottoms.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleBottoms.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                        case "shoes":
                            if (gender == "male" || gender == "both")
                                this.MaleShoes.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleShoes.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                        case "weapon":
                            if (gender == "male" || gender == "both")
                                this.MaleWeapons.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            if (gender == "female" || gender == "both")
                                this.FemaleWeapons.Add(new Tuple<CharacterConstants.JobType, int>(jobType, (int)datum["objectid"]));
                            break;
                    }
                }
            }
        }
    }
}
