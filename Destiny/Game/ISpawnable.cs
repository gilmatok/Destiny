using Destiny.Core.IO;

namespace Destiny.Game
{
    public interface ISpawnable
    {
        OutPacket GetCreatePacket();
        OutPacket GetSpawnPacket();
        OutPacket GetDestroyPacket();
    }
}
