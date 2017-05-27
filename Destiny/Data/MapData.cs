using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class MapData
    {
        public const int INVALID_MAP_IDENTIFIER = 999999999;

        [Flags]
        public enum EMapFlags : ushort
        {
            None = 0 << 0,
            Town = 1 << 0,
            Clock = 1 << 1,
            Swim = 1 << 2,
            Fly = 1 << 3,
            Everlast = 1 << 4,
            No_Party_Leader_Pass = 1 << 5,
            NoPartyLeaderPass = 1 << 5,
            Shop = 1 << 6,
            Scroll_Disable = 1 << 7,
            ScrollDisable = 1 << 7,
            Shuffle_Reactors = 1 << 8,
            ShuffleReactors = 1 << 8
        }

        [Flags]
        public enum EMapFieldType : ushort
        {
            None = 0 << 0,
            No_Clue1 = 1 << 0,
            No_Clue2 = 1 << 1,
            No_Clue3 = 1 << 2,
            No_Clue4 = 1 << 3,
            No_Clue5 = 1 << 4,
            No_Clue6 = 1 << 5,
            Force_Map_Equip = 1 << 6,
            ForceMapEquip = 1 << 6,
            No_Clue7 = 1 << 7,
            No_Clue8 = 1 << 8,
            No_Clue9 = 1 << 9
        }

        [Flags]
        public enum EMapFieldLimit : long
        {
            None = 0 << 0,
            Jump = 1 << 0,
            Movement_Skills = 1 << 1,
            MovementSkills = 1 << 1,
            Summoning_Bag = 1 << 2,
            SummoningBag = 1 << 2,
            Mystic_Door = 1 << 3,
            MysticDoor = 1 << 3,
            Channel_Switching = 1 << 4,
            ChannelSwitching = 1 << 4,
            Regular_EXP_Loss = 1 << 5,
            RegularEXPLoss = 1 << 5,
            VIP_Rock = 1 << 6,
            VIPRock = 1 << 6,
            MiniGames = 1 << 7,
            No_Clue1 = 1 << 8,
            Mount = 1 << 9,
            No_Clue2 = 1 << 10,
            No_Clue3 = 1 << 11,
            Potion_Use = 1 << 12,
            PotionUse = 1 << 12,
            No_Clue4 = 1 << 13,
            Unused1 = 1 << 14,
            No_Clue5 = 1 << 15,
            No_Clue6 = 1 << 16,
            Drop_Down = 1 << 17,
            DropDown = 1 << 17,
            No_Clue7 = 1 << 18,
            No_Clue8 = 1 << 19,
            Unused2 = 1 << 20,
            Unused3 = 1 << 21,
            Chalkboard = 1 << 22,
            Unused4 = 1 << 23,
            Unused5 = 1 << 24,
            Unused6 = 1 << 25,
            Unused7 = 1 << 26,
            Unused8 = 1 << 27,
            Unused9 = 1 << 28,
            Unused10 = 1 << 29,
            Unused11 = 1 << 30,
            Unused12 = 1 << 31,
        }

        public sealed class MapFootholdData
        {
            [Flags]
            public enum EMapFootholdFlags : byte
            {
                None = 0 << 0,
                Forbid_Downward_Jump = 1 << 0,
                ForbidDownwardJump = 1 << 0
            }


            public ushort Identifier { get; set; }
            public EMapFootholdFlags Flags { get; set; }
            public ushort PreviousIdentifier { get; set; }
            public ushort NextIdentifier { get; set; }
            public short DragForce { get; set; }
            public short X1 { get; set; }
            public short Y1 { get; set; }
            public short X2 { get; set; }
            public short Y2 { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(Identifier);
                pWriter.Write((byte)Flags);
                pWriter.Write(PreviousIdentifier);
                pWriter.Write(NextIdentifier);
                pWriter.Write(DragForce);
                pWriter.Write(X1);
                pWriter.Write(Y1);
                pWriter.Write(X2);
                pWriter.Write(Y2);
            }

            public void Load(BinaryReader pReader)
            {
                Identifier = pReader.ReadUInt16();
                Flags = (EMapFootholdFlags)pReader.ReadByte();
                PreviousIdentifier = pReader.ReadUInt16();
                NextIdentifier = pReader.ReadUInt16();
                DragForce = pReader.ReadInt16();
                X1 = pReader.ReadInt16();
                Y1 = pReader.ReadInt16();
                X2 = pReader.ReadInt16();
                Y2 = pReader.ReadInt16();
            }
        }

        public sealed class MapNPCData
        {
            [Flags]
            public enum EMapNPCFlags : byte
            {
                None = 0 << 0,
                Faces_Left = 1 << 0,
                FacesLeft = 1 << 0
            }


            public int NPCIdentifier { get; set; }
            public EMapNPCFlags Flags { get; set; }
            public ushort Foothold { get; set; }
            public short X { get; set; }
            public short Y { get; set; }
            public short MinClickX { get; set; }
            public short MaxClickX { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(NPCIdentifier);
                pWriter.Write((byte)Flags);
                pWriter.Write(Foothold);
                pWriter.Write(X);
                pWriter.Write(Y);
                pWriter.Write(MinClickX);
                pWriter.Write(MaxClickX);
            }

            public void Load(BinaryReader pReader)
            {
                NPCIdentifier = pReader.ReadInt32();
                Flags = (EMapNPCFlags)pReader.ReadByte();
                Foothold = pReader.ReadUInt16();
                X = pReader.ReadInt16();
                Y = pReader.ReadInt16();
                MinClickX = pReader.ReadInt16();
                MaxClickX = pReader.ReadInt16();
            }
        }

        public sealed class MapReactorData
        {
            [Flags]
            public enum EMapReactorFlags : byte
            {
                None = 0 << 0,
                Faces_Left = 1 << 0,
                FacesLeft = 1 << 0
            }


            public int ReactorIdentifier { get; set; }
            public EMapReactorFlags Flags { get; set; }
            public ushort Foothold { get; set; }
            public short X { get; set; }
            public short Y { get; set; }
            public short MinClickX { get; set; }
            public short MaxClickX { get; set; }
            public int RespawnTime { get; set; }
            public string Name { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(ReactorIdentifier);
                pWriter.Write((byte)Flags);
                pWriter.Write(Foothold);
                pWriter.Write(X);
                pWriter.Write(Y);
                pWriter.Write(MinClickX);
                pWriter.Write(MaxClickX);
                pWriter.Write(RespawnTime);
                pWriter.Write(Name);
            }

            public void Load(BinaryReader pReader)
            {
                ReactorIdentifier = pReader.ReadInt32();
                Flags = (EMapReactorFlags)pReader.ReadByte();
                Foothold = pReader.ReadUInt16();
                X = pReader.ReadInt16();
                Y = pReader.ReadInt16();
                MinClickX = pReader.ReadInt16();
                MaxClickX = pReader.ReadInt16();
                RespawnTime = pReader.ReadInt32();
                Name = pReader.ReadString();
            }
        }

        public sealed class MapMobData
        {
            [Flags]
            public enum EMapMobFlags : byte
            {
                None = 0 << 0,
                Faces_Left = 1 << 0,
                FacesLeft = 1 << 0
            }


            public int MobIdentifier { get; set; }
            public EMapMobFlags Flags { get; set; }
            public ushort Foothold { get; set; }
            public short X { get; set; }
            public short Y { get; set; }
            public short MinClickX { get; set; }
            public short MaxClickX { get; set; }
            public int RespawnTime { get; set; }
            public byte StartHour { get; set; }
            public byte EndHour { get; set; }
            public string Announcement { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(MobIdentifier);
                pWriter.Write((byte)Flags);
                pWriter.Write(Foothold);
                pWriter.Write(X);
                pWriter.Write(Y);
                pWriter.Write(MinClickX);
                pWriter.Write(MaxClickX);
                pWriter.Write(RespawnTime);
                pWriter.Write(StartHour);
                pWriter.Write(EndHour);
                pWriter.Write(Announcement);
            }

            public void Load(BinaryReader pReader)
            {
                MobIdentifier = pReader.ReadInt32();
                Flags = (EMapMobFlags)pReader.ReadByte();
                Foothold = pReader.ReadUInt16();
                X = pReader.ReadInt16();
                Y = pReader.ReadInt16();
                MinClickX = pReader.ReadInt16();
                MaxClickX = pReader.ReadInt16();
                RespawnTime = pReader.ReadInt32();
                StartHour = pReader.ReadByte();
                EndHour = pReader.ReadByte();
                Announcement = pReader.ReadString();
            }
        }

        public sealed class MapPortalData
        {
            [Flags]
            public enum EMapPortalFlags : byte
            {
                None = 0 << 0,
                Only_Once = 1 << 0,
                OnlyOnce = 1 << 0
            }


            public EMapPortalFlags Flags { get; set; }
            public short X { get; set; }
            public short Y { get; set; }
            public string Name { get; set; }
            public int ToMapIdentifier { get; set; }
            public string ToName { get; set; }
            public string Script { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(X);
                pWriter.Write(Y);
                pWriter.Write(Name);
                pWriter.Write(ToMapIdentifier);
                pWriter.Write(ToName);
                pWriter.Write(Script);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EMapPortalFlags)pReader.ReadByte();
                X = pReader.ReadInt16();
                Y = pReader.ReadInt16();
                Name = pReader.ReadString();
                ToMapIdentifier = pReader.ReadInt32();
                ToName = pReader.ReadString();
                Script = pReader.ReadString();
            }
        }

        public sealed class MapSeatData
        {
            public short X { get; set; }
            public short Y { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(X);
                pWriter.Write(Y);
            }

            public void Load(BinaryReader pReader)
            {
                X = pReader.ReadInt16();
                Y = pReader.ReadInt16();
            }
        }


        public int Identifier { get; set; }
        public EMapFlags Flags { get; set; }
        public string ShuffleName { get; set; }
        public string Music { get; set; }
        public byte MinLevelLimit { get; set; }
        public ushort TimeLimit { get; set; }
        public byte RegenRate { get; set; }
        public float Traction { get; set; }
        public short LeftTopX { get; set; }
        public short LeftTopY { get; set; }
        public short RightBottomX { get; set; }
        public short RightBottomY { get; set; }
        public int ReturnMapIdentifier { get; set; }
        public int ForcedReturnMapIdentifier { get; set; }
        public EMapFieldType FieldTypes { get; set; }
        public EMapFieldLimit FieldLimits { get; set; }
        public byte DecreaseHP { get; set; }
        public ushort DamagePerSecond { get; set; }
        public int ProtectItemIdentifier { get; set; }
        public float MobRate { get; set; }
        public int LinkIdentifier { get; set; }
        public List<MapFootholdData> Footholds { get; set; }
        public List<MapNPCData> NPCs { get; set; }
        public List<MapReactorData> Reactors { get; set; }
        public List<MapMobData> Mobs { get; set; }
        public List<MapPortalData> Portals { get; set; }
        public List<MapSeatData> Seats { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((ushort)Flags);
            pWriter.Write(ShuffleName);
            pWriter.Write(Music);
            pWriter.Write(MinLevelLimit);
            pWriter.Write(TimeLimit);
            pWriter.Write(RegenRate);
            pWriter.Write(Traction);
            pWriter.Write(LeftTopX);
            pWriter.Write(LeftTopY);
            pWriter.Write(RightBottomX);
            pWriter.Write(RightBottomY);
            pWriter.Write(ReturnMapIdentifier);
            pWriter.Write(ForcedReturnMapIdentifier);
            pWriter.Write((ushort)FieldTypes);
            pWriter.Write((uint)FieldLimits);
            pWriter.Write(DecreaseHP);
            pWriter.Write(DamagePerSecond);
            pWriter.Write(ProtectItemIdentifier);
            pWriter.Write(MobRate);
            pWriter.Write(LinkIdentifier);

            pWriter.Write(Footholds.Count);
            Footholds.ForEach(f => f.Save(pWriter));

            pWriter.Write(NPCs.Count);
            NPCs.ForEach(n => n.Save(pWriter));

            pWriter.Write(Reactors.Count);
            Reactors.ForEach(r => r.Save(pWriter));

            pWriter.Write(Mobs.Count);
            Mobs.ForEach(m => m.Save(pWriter));

            pWriter.Write(Portals.Count);
            Portals.ForEach(p => p.Save(pWriter));

            pWriter.Write(Seats.Count);
            Seats.ForEach(s => s.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (EMapFlags)pReader.ReadUInt16();
            ShuffleName = pReader.ReadString();
            Music = pReader.ReadString();
            MinLevelLimit = pReader.ReadByte();
            TimeLimit = pReader.ReadUInt16();
            RegenRate = pReader.ReadByte();
            Traction = pReader.ReadSingle();
            LeftTopX = pReader.ReadInt16();
            LeftTopY = pReader.ReadInt16();
            RightBottomX = pReader.ReadInt16();
            RightBottomY = pReader.ReadInt16();
            ReturnMapIdentifier = pReader.ReadInt32();
            ForcedReturnMapIdentifier = pReader.ReadInt32();
            FieldTypes = (EMapFieldType)pReader.ReadUInt16();
            FieldLimits = (EMapFieldLimit)pReader.ReadUInt32();
            DecreaseHP = pReader.ReadByte();
            DamagePerSecond = pReader.ReadUInt16();
            ProtectItemIdentifier = pReader.ReadInt32();
            MobRate = pReader.ReadSingle();
            LinkIdentifier = pReader.ReadInt32();

            int footholdsCount = pReader.ReadInt32();
            Footholds = new List<MapFootholdData>(footholdsCount);
            while (footholdsCount-- > 0)
            {
                MapFootholdData foothold = new MapFootholdData();
                foothold.Load(pReader);
                Footholds.Add(foothold);
            }

            int npcsCount = pReader.ReadInt32();
            NPCs = new List<MapNPCData>(npcsCount);
            while (npcsCount-- > 0)
            {
                MapNPCData npc = new MapNPCData();
                npc.Load(pReader);
                NPCs.Add(npc);
            }

            int reactorsCount = pReader.ReadInt32();
            Reactors = new List<MapReactorData>(reactorsCount);
            while (reactorsCount-- > 0)
            {
                MapReactorData reactor = new MapReactorData();
                reactor.Load(pReader);
                Reactors.Add(reactor);
            }

            int mobsCount = pReader.ReadInt32();
            Mobs = new List<MapMobData>(mobsCount);
            while (mobsCount-- > 0)
            {
                MapMobData mob = new MapMobData();
                mob.Load(pReader);
                Mobs.Add(mob);
            }

            int portalsCount = pReader.ReadInt32();
            Portals = new List<MapPortalData>(portalsCount);
            while (portalsCount-- > 0)
            {
                MapPortalData portal = new MapPortalData();
                portal.Load(pReader);
                Portals.Add(portal);
            }

            int seatsCount = pReader.ReadInt32();
            Seats = new List<MapSeatData>(seatsCount);
            while (seatsCount-- > 0)
            {
                MapSeatData seat = new MapSeatData();
                seat.Load(pReader);
                Seats.Add(seat);
            }
        }
    }
}
