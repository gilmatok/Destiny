using System;

namespace Destiny.Network
{
    public sealed class OutPacket
    {
        public OutPacket WriteBool(bool value = false) { return this; }
        public OutPacket WriteByte(byte value = 0) { return this; }
        public OutPacket WriteSByte(sbyte value = 0) { return this; }
        public OutPacket WriteShort(short value = 0) { return this; }
        public OutPacket WriteUShort(ushort value = 0) { return this; }
        public OutPacket WriteInt(int value =0) { return this; }
        public OutPacket WriteUInt(uint value = 0) { return this; }
        public OutPacket WriteLong(long value = 0) { return this; }
        public OutPacket WriteULong(ulong value = 0) { return this; }
        public OutPacket WriteString(string value) { return this; }
        public OutPacket WriteDateTime(DateTime value) { return this; }
    }
}
