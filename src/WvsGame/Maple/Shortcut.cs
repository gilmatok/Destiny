using Destiny.Data;
using Destiny.Data;

namespace Destiny.Maple
{
    public sealed class Shortcut
    {
        public KeymapKey Key { get; private set; }
        public KeymapType Type { get; set; }
        public KeymapAction Action { get; set; }

        public Shortcut(Datum datum)
        {
            this.Key = (KeymapKey)datum["Key"];
            this.Type = (KeymapType)datum["Type"];
            this.Action = (KeymapAction)datum["Action"];
        }

        public Shortcut(KeymapKey key, KeymapAction action, KeymapType type = KeymapType.None)
        {
            this.Key = key;

            if (type == KeymapType.None)
            {
                this.Type = this.GetTypeFromAction(action);
            }
            else
            {
                this.Type = type;
            }

            this.Action = action;
        }

        private KeymapType GetTypeFromAction(KeymapAction action)
        {
            switch (action)
            {
                case KeymapAction.Cockeyed:
                case KeymapAction.Happy:
                case KeymapAction.Sarcastic:
                case KeymapAction.Crying:
                case KeymapAction.Outraged:
                case KeymapAction.Shocked:
                case KeymapAction.Annoyed:
                    return KeymapType.BasicFace;

                case KeymapAction.PickUp:
                case KeymapAction.Sit:
                case KeymapAction.Attack:
                case KeymapAction.Jump:
                case KeymapAction.NpcChat:
                    return KeymapType.BasicAction;

                case KeymapAction.EquipmentMenu:
                case KeymapAction.ItemMenu:
                case KeymapAction.AbilityMenu:
                case KeymapAction.SkillMenu:
                case KeymapAction.BuddyList:
                case KeymapAction.WorldMap:
                case KeymapAction.Messenger:
                case KeymapAction.MiniMap:
                case KeymapAction.QuestMenu:
                case KeymapAction.SetKey:
                case KeymapAction.AllChat:
                case KeymapAction.WhisperChat:
                case KeymapAction.PartyChat:
                case KeymapAction.BuddyChat:
                case KeymapAction.Shortcut:
                case KeymapAction.QuickSlot:
                case KeymapAction.ExpandChat:
                case KeymapAction.GuildList:
                case KeymapAction.GuildChat:
                case KeymapAction.PartyList:
                case KeymapAction.QuestHelper:
                case KeymapAction.SpouseChat:
                case KeymapAction.MonsterBook:
                case KeymapAction.CashShop:
                case KeymapAction.AllianceChat:
                case KeymapAction.PartySearch:
                case KeymapAction.FamilyList:
                case KeymapAction.Medal:
                    return KeymapType.Menu;
            }


            return KeymapType.None;
        }
    }
}
