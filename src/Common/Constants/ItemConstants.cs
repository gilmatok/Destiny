using System;

namespace Destiny.Constants
{
    public class ItemConstants
    {
        #region ItemFlags
        [Flags]
        public enum ItemFlags : short
        {
            Sealed = 0x01,
            AddPreventSlipping = 0x02,
            AddPreventColdness = 0x04,
            Untradeable = 0x08,
            Scissored = 0x10
        }
        #endregion

        #region ItemType
        public enum ItemType : byte
        {
            Equipment = 1,
            Usable = 2,
            Setup = 3,
            Etcetera = 4,
            Cash = 5,
            Count = 6
        }
        #endregion

        #region InventoryOperationType
        public enum InventoryOperationType : byte
        {
            AddItem,
            ModifyQuantity,
            ModifySlot,
            RemoveItem
        }
        #endregion

        #region MemoAction
        public enum MemoAction : byte
        {
            Send = 0,
            Delete = 1
        }
        #endregion

        #region MemoResult
        public enum MemoResult : byte
        {
            Send = 3,
            Sent = 4,
            Error = 5
        }
        #endregion

        #region MemoError
        public enum MemoError : byte
        {
            ReceiverOnline,
            ReceiverInvalidName,
            ReceiverInboxFull
        }
        #endregion

        #region TrockAction
        public enum TrockAction : byte
        {
            Remove = 0,
            Add = 1
        }
        #endregion

        #region TrockType
        public enum TrockType : byte
        {
            Regular = 0,
            VIP = 1
        }
        #endregion

        #region TrockResult
        public enum TrockResult : byte
        {
            Success,
            Unknown = 2,
            Unknown2 = 3,
            CannotGo2 = 5,
            DifficultToLocate = 6,
            DifficultToLocate2 = 7,
            CannotGo = 8,
            AlreadyThere = 9,
            CannotSaveMap = 10,
            NoobsCannotLeaveMapleIsland = 11
        }
        #endregion

        #region UsableCashItems
        public enum UsableCashItems : int
        {
            TeleportRock = 5040000,
            CokeTeleportRock,
            VIPTeleportRock = 5041000,

            APReset = 5050000,
            SPReset1stJob,
            SPReset2stJob,
            SPReset3stJob,
            SPReset4stJob,

            ItemTag = 5060000,
            ItemGuard,
            Incubator,

            ItemGuard7Days = 5061000,
            ItemGuard30Days,
            ItemGuard90Days,

            CheapMegaphone = 5070000,
            Megaphone = 5071000,
            SuperMegaphone = 5072000,
            HeartMegaphone = 5073000,
            SkullMegaphone = 5074000,

            MapleTVMessenger = 5075000,
            MapleTVStarMessenger,
            MapleTVHeartMessenger,
            Megassenger,
            StarMegassenger,
            HeartMegassenger,
            ItemMegaphone = 5076000,

            KoreanKite = 5080000,
            HeartBalloon,
            GraduationBanner,
            AdmissionBanner,

            Note = 5090000,      // Memo
            CongratulatorySong = 5100000,
            PetNameTag = 5170000,

            BronzeSackofMesos = 5200000,
            SilverSackofMesos,
            GoldSackofMesos,

            FungusScroll = 5300000,
            OinkerDelight,
            ZetaNightmare,

            ChalkBoard = 5370000,
            ChalkBoard2,

            ScissorsofKarma = 5520000,
            ViciousHammer = 5570000
        }
        #endregion

        #region Equipment
        public enum EquipmentSlot : sbyte
        {
            Hat = -1,
            Face = -2,
            Eye = -3,
            Mantle = -4,
            Top = -5,
            Bottom = -6,
            Shoes = -7,
            Gloves = -8,
            Cape = -9,
            Shield = -10,
            Weapon = -11,
            Ring1 = -12,
            Ring2 = -13,
            // -14 ??
            Ring3 = -15,
            Ring4 = -16,
            Necklace = -17,
            Mount = -18,
            Saddle = -19,
            Medal = -49,
            Belt = -50,
            CashHat = -101,
            CashFace = -102,
            CashEye = -103,
            CashTop = -104,
            CashOverall = -105,
            CashBottom = -106,
            CashShoes = -107,
            CashGloves = -108,
            CashCape = -109,
            CashShield = -110,
            CashWeapon = -111,
            CashRing = -112,
            CashNecklace = -117,
            CashMount = -118
        }

        public enum EquippedQueryMode
        {
            Any,
            Cash,
            Normal
        }

        public enum WeaponType
        {
            NotAWeapon,
            Bow,
            Claw,
            Dagger,
            Crossbow,
            Axe1H,
            Sword1H,
            Blunt1H,
            Axe2H,
            Sword2H,
            Blunt2H,
            PoleArm,
            Spear,
            Staff,
            Wand,
            Knuckle,
            Gun
        }
        #endregion

        #region DropType
        public enum DropType : int
        {
            normalDrop,
            partyDrop,
            FFADrop,
            explosiveDrop
        }
        #endregion
    }   
}