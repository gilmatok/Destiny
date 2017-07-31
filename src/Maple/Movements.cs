using Destiny.Core.IO;
using System.Collections.Generic;
using System.Linq;

namespace Destiny.Maple
{
    public enum MovementType : byte
    {
        Normal,
        Unknown1,
        Unknown2,
        Unknown3,
        Unknown4,
        Unknown5,
        Unknown6,
        Unknown7,
        Unknown8,
        Unknown9,
        Equipment,
        Chair,
        Unknown12,
        Unknown13,
        Unknown14,
        JumpDown,
        Unknown16,
        Unknown17,
        Unknown18,
        Unknown19,
        Unknown20,
        Unknown21,
        Unknown22
    }

    public sealed class Movement
    {
        public MovementType Type { get; set; }
        public Point Position { get; set; }
        public Point Velocity { get; set; }
        public short Foothold { get; set; }
        public byte Stance { get; set; }
        public short Duration { get; set; }
        public byte Statistic { get; set; }
    }

    public sealed class Movements : List<Movement>
    {
        public static Movements Decode(InPacket iPacket)
        {
            return new Movements(iPacket);
        }

        public Point Position { get; private set; }
        public Point Velocity { get; private set; }
        public short LastFoothold { get; private set; }

        public Movements(InPacket iPacket)
            : base()
        {
            this.Position = iPacket.ReadPoint();
            this.Velocity = iPacket.ReadPoint();
            iPacket.Skip(1); // NOTE: Unknown.

            byte count = iPacket.ReadByte();

            while (count-- > 0)
            {
                Movement movement = new Movement();

                movement.Type = (MovementType)iPacket.ReadByte();

                switch (movement.Type)
                {
                    case MovementType.Normal:
                    case MovementType.Unknown5:
                    case MovementType.JumpDown:
                    case MovementType.Unknown17:
                        {
                            movement.Position = iPacket.ReadPoint();
                            movement.Velocity = iPacket.ReadPoint();
                            movement.Foothold = iPacket.ReadShort();
                            this.LastFoothold = movement.Foothold;

                            if (movement.Type != MovementType.JumpDown)
                            {
                                movement.Stance = iPacket.ReadByte();
                                movement.Duration = iPacket.ReadShort();
                            }
                        }
                        break;

                    case MovementType.Unknown1:
                    case MovementType.Unknown2:
                    case MovementType.Unknown6:
                    case MovementType.Unknown12:
                    case MovementType.Unknown13:
                    case MovementType.Unknown16:
                    case MovementType.Unknown18:
                    case MovementType.Unknown19:
                    case MovementType.Unknown20:
                    case MovementType.Unknown22:
                        {
                            movement.Velocity = iPacket.ReadPoint();
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Unknown3:
                    case MovementType.Unknown4:
                    case MovementType.Unknown7:
                    case MovementType.Unknown8:
                    case MovementType.Unknown9:
                    case MovementType.Chair:
                        {
                            movement.Position = iPacket.ReadPoint();
                            movement.Foothold = iPacket.ReadShort();
                            this.LastFoothold = movement.Foothold;
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Unknown14:
                        {
                            // TODO: Jump down or equipment, not sure.
                        }
                        break;

                    case MovementType.Equipment:
                        {
                            movement.Statistic = iPacket.ReadByte();
                        }
                        break;
                }

                this.Add(movement);
            }
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WritePoint(this.Position)
                .WriteByte((byte)this.Count);

            foreach (Movement movement in this)
            {
                oPacket.WriteByte((byte)movement.Type);

                switch (movement.Type)
                {
                    case MovementType.Normal:
                    case MovementType.Unknown5:
                    case MovementType.JumpDown:
                    case MovementType.Unknown17:
                        {
                            oPacket
                                .WritePoint(movement.Position)
                                .WritePoint(movement.Velocity)
                                .WriteShort(movement.Foothold);

                            if (movement.Type != MovementType.JumpDown)
                            {
                                oPacket
                                    .WriteByte(movement.Stance)
                                    .WriteShort(movement.Duration);
                            }
                        }
                        break;

                    case MovementType.Unknown1:
                    case MovementType.Unknown2:
                    case MovementType.Unknown6:
                    case MovementType.Unknown12:
                    case MovementType.Unknown13:
                    case MovementType.Unknown16:
                    case MovementType.Unknown18:
                    case MovementType.Unknown19:
                    case MovementType.Unknown20:
                    case MovementType.Unknown22:
                        {
                            oPacket
                                .WritePoint(movement.Velocity)
                                .WriteByte(movement.Stance)
                                .WriteShort(movement.Duration);
                        }
                        break;

                    case MovementType.Unknown3:
                    case MovementType.Unknown4:
                    case MovementType.Unknown7:
                    case MovementType.Unknown8:
                    case MovementType.Unknown9:
                    case MovementType.Chair:
                        {
                            oPacket
                                .WritePoint(movement.Position)
                                .WriteShort(movement.Foothold)
                                .WriteByte(movement.Stance)
                                .WriteShort(movement.Duration);
                        }
                        break;

                    case MovementType.Unknown14:
                        {
                            // TODO: Jump down or equipment, not sure.
                        }
                        break;

                    case MovementType.Equipment:
                        {
                            oPacket.WriteByte(movement.Statistic);
                        }
                        break;
                }
            }
        }
    }
}
