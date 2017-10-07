using Destiny.Maple.Characters;
using Destiny.Maple.Life;
using Destiny.Network;
using Destiny.Threading;
using System;

namespace Destiny.Maple.Scripting
{
    public sealed class NpcScript : ScriptBase
    {
        private Npc mNpc;
        private string mText;
        private WaitableResult<int> mResult;

        public NpcScript(Npc npc, Character character)
            : base(ScriptType.Npc, npc.MapleID.ToString(), character, true) // TODO: Use actual npc script instead of ID.
        {
            mNpc = npc;

            this.Expose("answer_no", 0);
            this.Expose("answer_yes", 1);

            this.Expose("answer_decline", 0);
            this.Expose("answer_accept", 1);

            this.Expose("quiz_npc", 0);
            this.Expose("quiz_mob", 1);
            this.Expose("quiz_item", 2);

            this.Expose("addText", new Action<string>(this.AddText));
            this.Expose("sendOk", new Func<int>(this.SendOk));
            this.Expose("sendNext", new Func<int>(this.SendNext));
            this.Expose("sendBackNext", new Func<int>(this.SendBackNext));
            this.Expose("sendBackOk", new Func<int>(this.SendBackOk));
            this.Expose("askYesNo", new Func<int>(this.AskYesNo));
            this.Expose("askAcceptDecline", new Func<int>(this.AskAcceptDecline));
        }

        public void SetResult(int value)
        {
            mResult.Set(value);
        }

        private void AddText(string text)
        {
            mText += text;
        }

        private int SendOk()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 0, 0))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private int SendNext()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 0, 1))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private int SendBackOk()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 1, 0))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private int SendBackNext()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 1, 1))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private int AskYesNo()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.YesNo))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private int AskAcceptDecline()
        {
            mResult = new WaitableResult<int>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.AcceptDecline))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mResult.Wait();

            return mResult.Value;
        }

        private void AskChoice()
        {

        }
    }
}
