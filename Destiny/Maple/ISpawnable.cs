using Destiny.Core.IO;

namespace Destiny.Maple
{
    public interface ISpawnable
    {
        OutPacket GetCreatePacket();
        OutPacket GetSpawnPacket();
        OutPacket GetDestroyPacket();
    }
}
