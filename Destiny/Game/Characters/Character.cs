using Destiny.Network;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network.Packet;

namespace Destiny.Game.Characters
{
    public sealed class Character : MapObject, IMoveable
    {
        public MapleClient Client { get; private set; }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public byte SpawnPoint { get; set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public byte Portals { get; set; }
        public bool IsInitialized { get; set; }

        public CharacterStats Stats { get; private set; }
        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public ControlledMobs ControlledMobs { get; private set; }
        public ControlledNpcs ControlledNpcs { get; private set; }

        public bool IsGm
        {
            get
            {
                return this.Client.Account.GmLevel >= GmLevel.Intern;
            }
        }

        public bool FacesLeft
        {
            get
            {
                return this.Stance % 2 == 0;
            }
        }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Character;
            }
        }

        public Character(MapleClient client, DatabaseQuery query)
             : base()
        {
            this.Client = client;

            this.ID = query.GetInt("character_id");
            this.AccountID = query.GetInt("account_id");
            this.Name = query.GetString("name");
            this.Map = MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[query.GetInt("map")];
            this.SpawnPoint = query.GetByte("map_spawn");

            this.Stats = new CharacterStats(this, query);

            using (DatabaseQuery itemQuery = Database.Query("SELECT * FROM `items` WHERE `character_id` = @character_id", new MySqlParameter("character_id", this.ID)))
            {
                byte[] slots = new byte[(byte)InventoryType.Count];

                slots[(byte)InventoryType.Equipment] = query.GetByte("equipment_slots");
                slots[(byte)InventoryType.Usable] = query.GetByte("usable_slots");
                slots[(byte)InventoryType.Setup] = query.GetByte("setup_slots");
                slots[(byte)InventoryType.Etcetera] = query.GetByte("etcetera_slots");
                slots[(byte)InventoryType.Cash] = query.GetByte("cash_slots");

                this.Items = new CharacterItems(this, slots, itemQuery);
            }

            using (DatabaseQuery skillQuery = null)
            {
                this.Skills = new CharacterSkills(this, skillQuery);
            }

            using (DatabaseQuery questQuery = null)
            {
                this.Quests = new CharacterQuests(this, questQuery);
            }

            this.ControlledMobs = new ControlledMobs(this);
            this.ControlledNpcs = new ControlledNpcs(this);
        }

        public void Save()
        {

        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            this.Client.Send(UserPacket.BrodcastMsg(message, type));
        }

        public void ChangeMap(int mapID, byte portalID = 0)
        {
            this.SpawnPoint = portalID;

            this.Map.Characters.Remove(this);

            this.Map = MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[mapID];

            this.Client.Send(MapPacket.SetField(this, false));

            this.Map.Characters.Add(this);
        }

        public void Encode(OutPacket oPacket, long flag = long.MaxValue)
        {
            oPacket
                .WriteLong(flag)
                .WriteByte(); // NOTE: Unknown.

            this.Stats.Encode(oPacket);

            oPacket
                .WriteByte(20) // NOTE: Max buddylist size.
                .WriteBool(false) // NOTE: Blessing of Fairy.
                .WriteInt(); // NOTE: Mesos.

            this.Items.Encode(oPacket);
            this.Skills.Encode(oPacket);
            this.Quests.Encode(oPacket);

            oPacket
                .WriteShort() // NOTE: Mini games record.
                .WriteShort() // NOTE: Rings (1).
                .WriteShort() // NOTE: Rings (2). 
                .WriteShort(); // NOTE: Rings (3).

            // NOTE: Teleport rock locations.
            for (int i = 0; i < 15; i++)
            {
                oPacket.WriteInt(999999999);
            }

            oPacket
                .WriteInt() // NOTE: Monster book cover ID.
                .WriteByte() // NOTE: Unknown.
                .WriteShort() // NOTE: Monster book cards count.
                .WriteShort() // NOTE: New year cards.
                .WriteShort() // NOTE: Area information.
                .WriteShort(); // NOTE: Unknown.
        }
    }
}
