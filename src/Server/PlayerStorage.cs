using Destiny.Maple.Characters;
using System.Collections.Generic;
using System.Collections;

namespace Destiny.Network
{
    public sealed class PlayerStorage : IEnumerable<Character>
    {
        private Dictionary<int, Character> StorageByID { get; set; }
        private Dictionary<string, Character> StorageByName { get; set; }

        public PlayerStorage()
        {
            this.StorageByID = new Dictionary<int, Character>();
            this.StorageByName = new Dictionary<string, Character>();
        }

        public void Register(Character character)
        {
            this.StorageByID.Add(character.ID, character);
            this.StorageByName.Add(character.Name, character);
        }

        public void Unregister(Character character)
        {
            this.StorageByID.Remove(character.ID);
            this.StorageByName.Remove(character.Name);
        }

        public Character GetCharacter(int id)
        {
            Character ret = null;

            this.StorageByID.TryGetValue(id, out ret);

            return ret;
        }

        public Character GetCharacter(string name)
        {
            Character ret = null;

            this.StorageByName.TryGetValue(name, out ret);

            return ret;
        }

        public IEnumerator<Character> GetEnumerator()
        {
            return this.StorageByID.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.StorageByID.Values.GetEnumerator();
        }
    }
}
