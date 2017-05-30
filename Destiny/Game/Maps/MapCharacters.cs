using Destiny.Packet;

namespace Destiny.Game.Maps
{
    public sealed class MapCharacters : MapObjects<Character>
    {
        public MapCharacters(Map map) : base(map) { }

        protected override void InsertItem(int index, Character item)
        {
            base.InsertItem(index, item);

            foreach (Npc npc in this.Map.Npcs)
            {
                item.Client.Send(NpcPacket.NpcEnterField(npc));
            }

            foreach (Mob mob in this.Map.Mobs)
            {
                item.Client.Send(MobPacket.MobEnterField(mob));
            }
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
