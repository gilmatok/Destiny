using Destiny.Core.IO;
using System.Collections.Generic;

namespace Destiny.Maple
{
    public enum MovementType : byte
    {
        Normal = 0,
        Jump = 1,
        JumpKnockback = 2,
        Immediate = 3,
        Teleport = 4,
        Normal2 = 5,
        FlashJump = 6,
        Assaulter = 7,
        Assassinate = 8,
        Rush = 9,
        Falling = 10,
        Chair = 11,
        ExcessiveKnockback = 12,
        RecoilShot = 13,
        Unknown = 14,
        JumpDown = 15,
        Wings = 16,
        WingsFalling = 17,
        Unknown2 = 18,
        Unknown3 = 19,
        Aran = 20,
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
                    case MovementType.Normal2:
                    case MovementType.JumpDown:
                    case MovementType.WingsFalling:
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

                    case MovementType.Jump:
                    case MovementType.JumpKnockback:
                    case MovementType.FlashJump:
                    case MovementType.ExcessiveKnockback:
                    case MovementType.RecoilShot:
                    case MovementType.Aran:
                        {
                            movement.Velocity = iPacket.ReadPoint();
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Immediate:
                    case MovementType.Teleport:
                    case MovementType.Assaulter:
                    case MovementType.Assassinate:
                    case MovementType.Rush:
                    case MovementType.Chair:
                        {
                            movement.Position = iPacket.ReadPoint();
                            movement.Foothold = iPacket.ReadShort();
                            this.LastFoothold = movement.Foothold;
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
                        }
                        break;

                    case MovementType.Falling:
                        {
                            movement.Statistic = iPacket.ReadByte();
                        }
                        break;

                    case MovementType.Unknown:
                        {
                            movement.Velocity = iPacket.ReadPoint();
                            movement.Foothold = iPacket.ReadShort();
                            this.LastFoothold = movement.Foothold;
                            movement.Stance = iPacket.ReadByte();
                            movement.Duration = iPacket.ReadShort();
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
                    case MovementType.Normal2:
                    case MovementType.JumpDown:
                    case MovementType.WingsFalling:
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

                    case MovementType.Jump:
                    case MovementType.JumpKnockback:
                    case MovementType.FlashJump:
                    case MovementType.ExcessiveKnockback:
                    case MovementType.RecoilShot:
                    case MovementType.Aran:
                        {
                            oPacket
                                .WritePoint(movement.Velocity)
                                .WriteByte(movement.Stance)
                                .WriteShort(movement.Duration);
                        }
                        break;

                    case MovementType.Immediate:
                    case MovementType.Teleport:
                    case MovementType.Assaulter:
                    case MovementType.Assassinate:
                    case MovementType.Rush:
                    case MovementType.Chair:
                        {
                            oPacket
                                .WritePoint(movement.Position)
                                .WriteShort(movement.Foothold)
                                .WriteByte(movement.Stance)
                                .WriteShort(movement.Duration);
                        }
                        break;

                    case MovementType.Falling:
                        {
                            oPacket.WriteByte(movement.Statistic);
                        }
                        break;

                    case MovementType.Unknown:
                        {
                            oPacket
                                .WritePoint(movement.Velocity)
                                .WriteShort(movement.Foothold)
                                .WriteByte(movement.Stance)
                                .WriteShort(movement.Duration);
                        }
                        break;
                }
            }
        }
    }
}
