using System.Collections.Generic;
using System.IO;

namespace Destiny.Game.Data
{
    public sealed class MapFootholdData
    {
        public short ID { get; set; }
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public short NextID { get; set; }
        public short PreviousID { get; set; }

        public void Load(BinaryReader reader)
        {
            this.ID = reader.ReadInt16();
            this.Point1 = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.Point2 = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.NextID = reader.ReadInt16();
            this.PreviousID = reader.ReadInt16();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.ID);
            writer.Write(this.Point1.X);
            writer.Write(this.Point1.Y);
            writer.Write(this.Point2.X);
            writer.Write(this.Point2.Y);
            writer.Write(this.NextID);
            writer.Write(this.PreviousID);
        }
    }

    public class MapSpawnData
    {
        public int MapleID { get; set; }
        public short Foothold { get; set; }
        public Point Positon { get; set; }
        public bool Flip { get; set; }
        public bool Hide { get; set; }

        public virtual void Load(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.Foothold = reader.ReadInt16();
            this.Positon = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.Flip = reader.ReadBoolean();
            this.Hide = reader.ReadBoolean();
        }

        public virtual void Save(BinaryWriter writer)
        {
            writer.Write(this.MapleID);
            writer.Write(this.Foothold);
            writer.Write(this.Positon.X);
            writer.Write(this.Positon.Y);
            writer.Write(this.Flip);
            writer.Write(this.Hide);
        }
    }

    public sealed class MapNpcSpawnData : MapSpawnData
    {
        public short MinimumClickX { get; set; }
        public short MaximumClickX { get; set; }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            this.MinimumClickX = reader.ReadInt16();
            this.MaximumClickX = reader.ReadInt16();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            writer.Write(this.MinimumClickX);
            writer.Write(this.MaximumClickX);
        }
    }

    public sealed class MapMobSpawnData : MapSpawnData
    {
        public int RespawnTime { get; set; }

        public override void Load(BinaryReader reader)
        {
            base.Load(reader);

            this.RespawnTime = reader.ReadInt32();
        }

        public override void Save(BinaryWriter writer)
        {
            base.Save(writer);

            writer.Write(this.RespawnTime);
        }
    }

    public sealed class MapPortalData
    {
        public byte ID { get; set; }
        public string Label { get; set; }
        public int DestinationMap { get; set; }
        public string DestinationLabel { get; set; }
        public string Script { get; set; }
        public Point Position { get; set; }

        public void Load(BinaryReader reader)
        {
            this.ID = reader.ReadByte();
            this.Label = reader.ReadString();
            this.DestinationMap = reader.ReadInt32();
            this.DestinationLabel = reader.ReadString();
            this.Script = reader.ReadString();
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.ID);
            writer.Write(this.Label);
            writer.Write(this.DestinationMap);
            writer.Write(this.DestinationLabel);
            writer.Write(this.Script);
            writer.Write(this.Position.X);
            writer.Write(this.Position.Y);
        }
    }

    public sealed class MapReactorData
    {
        public void Load(BinaryReader reader)
        {

        }

        public void Save(BinaryWriter writer)
        {

        }
    }

    public sealed class MapSeatData
    {
        public short ID { get; set; }
        public Point Position { get; set; }

        public void Load(BinaryReader reader)
        {
            this.ID = reader.ReadInt16();
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.ID);
            writer.Write(this.Position.X);
            writer.Write(this.Position.Y);
        }
    }

    public sealed class MapData
    {
        public int MapleID { get; set; }
        public int ReturnMapID { get; set; }
        public int ForcedReturnMapID { get; set; }
        public List<MapFootholdData> Footholds { get; set; }
        public List<MapMobSpawnData> Mobs { get; set; }
        public List<MapNpcSpawnData> Npcs { get; set; }
        public List<MapPortalData> Portals { get; set; }
        public List<MapReactorData> Reactors { get; set; }
        public List<MapSeatData> Seats { get; set; }

        public void Load(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.ReturnMapID = reader.ReadInt32();
            this.ForcedReturnMapID = reader.ReadInt32();

            this.Footholds = new List<MapFootholdData>();
            int footholdsCount = reader.ReadInt32();
            while (footholdsCount-- > 0)
            {
                MapFootholdData foothold = new MapFootholdData();
                foothold.Load(reader);
                this.Footholds.Add(foothold);
            }

            this.Mobs = new List<MapMobSpawnData>();
            int mobsCount = reader.ReadInt32();
            while (mobsCount-- > 0)
            {
                MapMobSpawnData mob = new MapMobSpawnData();
                mob.Load(reader);
                this.Mobs.Add(mob);
            }

            this.Npcs = new List<MapNpcSpawnData>();
            int npcsCount = reader.ReadInt32();
            while (npcsCount-- > 0)
            {
                MapNpcSpawnData npc = new MapNpcSpawnData();
                npc.Load(reader);
                this.Npcs.Add(npc);
            }

            this.Portals = new List<MapPortalData>();
            int portalsCount = reader.ReadInt32();
            while (portalsCount-- > 0)
            {
                MapPortalData portal = new MapPortalData();
                portal.Load(reader);
                this.Portals.Add(portal);
            }

            this.Reactors = new List<MapReactorData>();
            int reactorsCount = reader.ReadInt32();
            while (footholdsCount-- > 0)
            {
                MapReactorData reactor = new MapReactorData();
                reactor.Load(reader);
                this.Reactors.Add(reactor);
            }

            this.Seats = new List<MapSeatData>();
            int seatsCount = reader.ReadInt32();
            while (seatsCount-- > 0)
            {
                MapSeatData seat = new MapSeatData();
                seat.Load(reader);
                this.Seats.Add(seat);
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(this.MapleID);
            writer.Write(this.ReturnMapID);
            writer.Write(this.ForcedReturnMapID);

            writer.Write(this.Footholds.Count);
            foreach (MapFootholdData foothold in this.Footholds)
            {
                foothold.Save(writer);
            }

            writer.Write(this.Mobs.Count);
            foreach (MapMobSpawnData foothold in this.Mobs)
            {
                foothold.Save(writer);
            }

            writer.Write(this.Npcs.Count);
            foreach (MapNpcSpawnData foothold in this.Npcs)
            {
                foothold.Save(writer);
            }

            writer.Write(this.Portals.Count);
            foreach (MapPortalData foothold in this.Portals)
            {
                foothold.Save(writer);
            }

            writer.Write(this.Reactors.Count);
            foreach (MapReactorData foothold in this.Reactors)
            {
                foothold.Save(writer);
            }

            writer.Write(this.Seats.Count);
            foreach (MapSeatData foothold in this.Seats)
            {
                foothold.Save(writer);
            }
        }
    }
}
