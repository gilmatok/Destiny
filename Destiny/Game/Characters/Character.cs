using Destiny.Network;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Packet;

namespace Destiny.Game
{
    public sealed class Character : MapObject
    {
        public MapleClient Client { get; private set; }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public byte SpawnPoint { get; set; }
        public bool IsInitialized { get; set; }

        public CharacterStats Stats { get; private set; }
        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }

        public bool IsGm
        {
            get
            {
                return this.Client.Account.GmLevel >= GmLevel.Intern;
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
        }

        public void Save()
        {

        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            this.Client.Send(UserPacket.BrodcastMsg(message, type));
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
