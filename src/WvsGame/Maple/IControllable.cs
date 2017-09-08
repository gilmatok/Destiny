using Destiny.Network;

namespace Destiny.Maple
{
    public interface IControllable
    {
        Packet GetControlRequestPacket();
        Packet GetControlCancelPacket();
    }
}
