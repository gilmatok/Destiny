using Destiny.Core.Network;

namespace Destiny.Maple
{
    public interface ISpawnable
    {
        OutPacket GetCreatePacket();
        OutPacket GetSpawnPacket();
        OutPacket GetDestroyPacket();
    }
}
