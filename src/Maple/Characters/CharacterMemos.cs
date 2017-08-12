using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using System.Collections.Generic;

namespace Destiny.Maple.Characters
{
    public sealed class CharacterMemos : List<Memo>
    {
        public Character Parent { get; private set; }

        public CharacterMemos(Character parent)
        {
            this.Parent = parent;
        }

        public void Load()
        {
            foreach (Datum datum in new Datums("memos").Populate("CharacterID = {0}", this.Parent.ID))
            {
                this.Add(new Memo(datum));
            }
        }

        // NOTE: Memos are inserted straight into the database.
        // Therefore, there is no need for a save method.

        public void Send()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.MemoResult))
            {
                oPacket
                    .WriteByte((byte)MemoResult.Send)
                    .WriteByte((byte)this.Count);

                foreach (Memo memo in this)
                {
                    memo.Encode(oPacket);
                }

                this.Parent.Client.Send(oPacket);
            }
        }
    }
}
