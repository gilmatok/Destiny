using Destiny.Core.Data;
using Destiny.Core.Network;
using System.Collections;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterBuffs : IEnumerable<Buff>
    {
        public Character Parent { get; private set; }

        private List<Buff> Buffs { get; set; }

        public Buff this[int mapleId]
        {
            get
            {
                foreach (Buff loopBuff in this.Buffs)
                {
                    if (loopBuff.MapleID == mapleId)
                    {
                        return loopBuff;
                    }
                }

                throw new KeyNotFoundException();
            }
        }

        public CharacterBuffs(Character parent)
            : base()
        {
            this.Parent = parent;

            this.Buffs = new List<Buff>();
        }

        public void Load()
        {
            //foreach (Datum datum in new Datums("buffs").Populate("CharacterID = {0}", this.Parent.ID))
            //{
            //    if ((DateTime)datum["End"] > DateTime.Now)
            //    {
            //        this.Add(new Buff(this, datum));
            //    }
            //}
        }

        public void Save()
        {
            this.Delete();

            foreach (Buff loopBuff in this.Buffs)
            {
                loopBuff.Save();
            }
        }

        public void Delete()
        {
            Database.Delete("buffs", "CharacterID = {0}", this.Parent.ID);
        }

        public bool Contains(Buff buff)
        {
            return this.Buffs.Contains(buff);
        }

        public bool Contains(int mapleId)
        {
            foreach (Buff loopBuff in this.Buffs)
            {
                if (loopBuff.MapleID == mapleId)
                {
                    return true;
                }
            }

            return false;
        }

        public void Add(Skill skill, int value)
        {
            this.Add(new Buff(this, skill, value));
        }

        public void Add(Buff buff)
        {
            foreach (Buff loopBuff in this.Buffs)
            {
                if (loopBuff.MapleID == buff.MapleID)
                {
                    this.Remove(loopBuff);

                    break;
                }
            }

            buff.Parent = this;

            this.Buffs.Add(buff);

            if (this.Parent.IsInitialized && buff.Type == 1)
            {
                buff.Apply();
            }
        }

        public void Remove(int mapleId)
        {
            this.Remove(this[mapleId]);
        }

        public void Remove(Buff buff)
        {
            this.Buffs.Remove(buff);

            if (this.Parent.IsInitialized)
            {
                buff.Cancel();
            }
        }

        public void Cancel(InPacket iPacket)
        {
            int mapleID = iPacket.ReadInt();

            switch (mapleID)
            {
                // TODO: Handle special skills.

                default:
                    this.Remove(mapleID);
                    break;
            }
        }

        public IEnumerator<Buff> GetEnumerator()
        {
            return this.Buffs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Buffs).GetEnumerator();
        }
    }
}
