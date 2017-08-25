using Destiny.Core.Network;

namespace Destiny.Maple
{
    public interface IControllable
    {
        OutPacket GetControlRequestPacket();
        OutPacket GetControlCancelPacket();
    }
}
