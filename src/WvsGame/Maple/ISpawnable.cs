using Destiny.Network;

namespace Destiny.Maple
{
    public interface ISpawnable
    {
        Packet GetCreatePacket();
        Packet GetSpawnPacket();
        Packet GetDestroyPacket();
    }
}
