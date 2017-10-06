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
        private WaitableResult<bool> mBooleanResult;
        private WaitableResult<int> mIntegerResult;

        public NpcScript(Npc npc, Character character)
            : base(ScriptType.Npc, npc.MapleID.ToString(), character, true)
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
            this.Expose("sendOk", new Func<bool>(this.SendOk));
            this.Expose("sendNext", new Func<bool>(this.SendNext));
            this.Expose("sendBackNext", new Func<bool>(this.SendBackNext));
            this.Expose("sendBackOk", new Func<bool>(this.SendBackOk));
        }

        public void SetBooleanResult(bool value)
        {
            mBooleanResult.Set(value);
        }

        public void SetIntegerResult(int value)
        {
            mIntegerResult.Set(value);
        }

        private void AddText(string text)
        {
            mText += text;
        }

        private bool SendOk()
        {
            mBooleanResult = new WaitableResult<bool>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 0, 0))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mBooleanResult.Wait();

            return mBooleanResult.Value;
        }

        private bool SendNext()
        {
            mBooleanResult = new WaitableResult<bool>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 0, 1))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mBooleanResult.Wait();

            return mBooleanResult.Value;
        }

        private bool SendBackOk()
        {
            mBooleanResult = new WaitableResult<bool>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 1, 0))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mBooleanResult.Wait();

            return mBooleanResult.Value;
        }

        private bool SendBackNext()
        {
            mBooleanResult = new WaitableResult<bool>();

            using (Packet oPacket = mNpc.GetDialogPacket(mText, NpcMessageType.Standard, 1, 1))
            {
                mCharacter.Client.Send(oPacket);
            }

            mText = string.Empty;

            mBooleanResult.Wait();

            return mBooleanResult.Value;
        }

        private void AskYesNo()
        {

        }

        private void AskAcceptDecline()
        {

        }

        private void AskChoice()
        {

        }
    }
}
