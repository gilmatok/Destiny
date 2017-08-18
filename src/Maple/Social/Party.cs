using Destiny.Maple.Characters;
using System.Collections.Generic;

namespace Destiny.Maple.Social
{
    public sealed class Party : List<Character>
    {
        public int ID { get; set; }

        public Character Leader
        {
            get
            {
                return this[0];
            }
        }

        public Party(int id)
            : base(6)
        {
            this.ID = id;
        }
    }
}
