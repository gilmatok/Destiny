using Destiny.Data;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterVariables : KeyedCollection<string, Variable>
    {
        public Character Parent { get; private set; }

        public CharacterVariables(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            foreach (Datum datum in new Datums("variables").Populate("CharacterID = {0}", this.Parent.ID))
            {
                this.Add(new Variable(datum));
            }
        }

        public void Save()
        {
            Database.Delete("variables", "CharacterID = {0}", this.Parent.ID);

            foreach (Variable variable in this)
            {
                Datum datum = new Datum("variables");

                datum["CharacterID"] = this.Parent.ID;
                datum["Key"] = variable.Key;
                datum["Value"] = variable.Value;

                datum.Insert();
            }
        }

        protected override string GetKeyForItem(Variable item)
        {
            return item.Key;
        }
    }
}
