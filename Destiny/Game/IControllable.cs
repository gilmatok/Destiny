using Destiny.Core.IO;

namespace Destiny.Game
{
    public interface IControllable
    {
        OutPacket GetControlRequestPacket();
        OutPacket GetControlCancelPacket();
    }
}
