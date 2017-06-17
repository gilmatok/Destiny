using Destiny.Core.IO;

namespace Destiny.Maple
{
    public interface IControllable
    {
        OutPacket GetControlRequestPacket();
        OutPacket GetControlCancelPacket();
    }
}
