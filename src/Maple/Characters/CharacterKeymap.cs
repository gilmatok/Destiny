using Destiny.Data;
using System.Collections.ObjectModel;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterKeymap : KeyedCollection<KeymapKey, Shortcut>
    {
        public Character Parent { get; private set; }

        public CharacterKeymap(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            foreach (Datum datum in new Datums("keymaps").Populate("CharacterID = {0}", this.Parent.ID))
            {
                this.Add(new Shortcut(datum));
            }
        }

        public void Save()
        {
            this.Delete();

            foreach (Shortcut entry in this)
            {
                Datum datum = new Datum("keymaps");

                datum["CharacterID"] = this.Parent.ID;
                datum["Key"] = (int)entry.Key;
                datum["Type"] = (byte)entry.Type;
                datum["Action"] = (int)entry.Action;

                datum.Insert();
            }
        }

        public void Delete()
        {
            Database.Delete("keymaps", "CharacterID = {0}", this.Parent.ID);
        }

        public void AddDefault()
        {
            this.Add(new Shortcut(KeymapKey.One, KeymapAction.AllChat));
            this.Add(new Shortcut(KeymapKey.Two, KeymapAction.PartyChat));
            this.Add(new Shortcut(KeymapKey.Three, KeymapAction.BuddyChat));
            this.Add(new Shortcut(KeymapKey.Four, KeymapAction.GuildChat));
            this.Add(new Shortcut(KeymapKey.Five, KeymapAction.AllianceChat));
            this.Add(new Shortcut(KeymapKey.Six, KeymapAction.SpouseChat));
            this.Add(new Shortcut(KeymapKey.Q, KeymapAction.QuestMenu));
            this.Add(new Shortcut(KeymapKey.W, KeymapAction.WorldMap));
            this.Add(new Shortcut(KeymapKey.E, KeymapAction.EquipmentMenu));
            this.Add(new Shortcut(KeymapKey.R, KeymapAction.BuddyList));
            this.Add(new Shortcut(KeymapKey.I, KeymapAction.ItemMenu));
            this.Add(new Shortcut(KeymapKey.O, KeymapAction.PartySearch));
            this.Add(new Shortcut(KeymapKey.P, KeymapAction.PartyList));
            this.Add(new Shortcut(KeymapKey.BracketLeft, KeymapAction.Shortcut));
            this.Add(new Shortcut(KeymapKey.BracketRight, KeymapAction.QuickSlot));
            this.Add(new Shortcut(KeymapKey.LeftCtrl, KeymapAction.Attack));
            this.Add(new Shortcut(KeymapKey.S, KeymapAction.AbilityMenu));
            this.Add(new Shortcut(KeymapKey.F, KeymapAction.FamilyList));
            this.Add(new Shortcut(KeymapKey.G, KeymapAction.GuildList));
            this.Add(new Shortcut(KeymapKey.H, KeymapAction.WhisperChat));
            this.Add(new Shortcut(KeymapKey.K, KeymapAction.SkillMenu));
            this.Add(new Shortcut(KeymapKey.L, KeymapAction.QuestHelper));
            this.Add(new Shortcut(KeymapKey.Quote, KeymapAction.ExpandChat));
            this.Add(new Shortcut(KeymapKey.Backtick, KeymapAction.CashShop));
            this.Add(new Shortcut(KeymapKey.Backslash, KeymapAction.SetKey));
            this.Add(new Shortcut(KeymapKey.Z, KeymapAction.PickUp));
            this.Add(new Shortcut(KeymapKey.X, KeymapAction.Sit));
            this.Add(new Shortcut(KeymapKey.C, KeymapAction.Messenger));
            this.Add(new Shortcut(KeymapKey.B, KeymapAction.MonsterBook));
            this.Add(new Shortcut(KeymapKey.M, KeymapAction.MiniMap));
            this.Add(new Shortcut(KeymapKey.LeftAlt, KeymapAction.Jump));
            this.Add(new Shortcut(KeymapKey.Space, KeymapAction.NpcChat));
            this.Add(new Shortcut(KeymapKey.F1, KeymapAction.Cockeyed));
            this.Add(new Shortcut(KeymapKey.F2, KeymapAction.Happy));
            this.Add(new Shortcut(KeymapKey.F3, KeymapAction.Sarcastic));
            this.Add(new Shortcut(KeymapKey.F4, KeymapAction.Crying));
            this.Add(new Shortcut(KeymapKey.F5, KeymapAction.Outraged));
            this.Add(new Shortcut(KeymapKey.F6, KeymapAction.Shocked));
            this.Add(new Shortcut(KeymapKey.F7, KeymapAction.Annoyed));
        }

        protected override KeymapKey GetKeyForItem(Shortcut item)
        {
            return item.Key;
        }
    }
}
