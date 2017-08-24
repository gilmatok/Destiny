using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Shops;
using System.Threading.Tasks;
using System.Collections.Generic;
using Destiny.Maple.Data;

namespace Destiny.Maple.Life
{
    public class Npc : LifeObject, ISpawnable, IControllable
    {
        public Npc(Datum datum) : base(datum) { }

        public Character Controller { get; set; }

        public Shop Shop { get; set; }
        public int StorageCost { get; set; }

        public Dictionary<Character, TaskCompletionSource<bool>> Responses = new Dictionary<Character, TaskCompletionSource<bool>>();
        public Dictionary<Character, TaskCompletionSource<int>> Choices = new Dictionary<Character, TaskCompletionSource<int>>();
        public Dictionary<Character, int[]> StyleSelectionHelpers = new Dictionary<Character, int[]>();

        public void Move(InPacket iPacket)
        {
            byte action1 = iPacket.ReadByte();
            byte action2 = iPacket.ReadByte();

            Movements movements = null;

            if (iPacket.Remaining > 0)
            {
                movements = Movements.Decode(iPacket);
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.NpcMove))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteByte(action1)
                    .WriteByte(action2);

                if (movements != null)
                {
                    oPacket.WriteBytes(movements.ToByteArray());
                }

                this.Map.Broadcast(oPacket);
            }
        }

        public virtual async Task Converse(Character talker)
        {
            if (this.Shop != null)
            {
                this.Shop.Show(talker);
            }
            else if (this.StorageCost > 0)
            {
                talker.Storage.Show(this);
            }
            else
            {
                Log.Warn("'{0}' attempted to converse with unimplemented NPC {1}.", talker.Name, this.MapleID);

                await this.ShowOkDialog(talker, ". . .");
            }
        }

        public void Handle(Character talker, InPacket iPacket)
        {
            NpcMessageType lastMessageType = (NpcMessageType)iPacket.ReadByte();
            byte action = iPacket.ReadByte();

            // TODO: Validate last message type.

            int selection = -1;

            byte endTalkByte;

            switch (lastMessageType)
            {
                case NpcMessageType.RequestText:
                case NpcMessageType.RequestNumber:
                case NpcMessageType.RequestStyle:
                case NpcMessageType.Choice:
                    endTalkByte = 0;
                    break;

                default:
                    endTalkByte = byte.MaxValue;
                    break;
            }

            if (action != endTalkByte)
            {
                if (iPacket.Remaining >= 4)
                {
                    selection = iPacket.ReadInt();
                }
                else if (iPacket.Remaining > 0)
                {
                    selection = iPacket.ReadByte();
                }

                if (lastMessageType == NpcMessageType.RequestStyle)
                {
                    selection = this.StyleSelectionHelpers[talker][selection];
                }

                if (selection != -1)
                {
                    this.Choices[talker].SetResult(selection);
                }
                else
                {
                    this.Responses[talker].SetResult(action == 1 ? true : false);
                }
            }
            else
            {
                talker.LastNpc = null;
            }
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character character in this.Map.Characters)
                    {
                        if (character.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledNpcs.Count;
                            newController = character;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledNpcs.Add(this);
                }
            }
        }

        public async Task<bool> ShowOkDialog(Character talker, string text)
        {
            this.Responses[talker] = new TaskCompletionSource<bool>();

            this.SendDialog(talker, text, NpcMessageType.Standard, 0, 0);

            return await this.Responses[talker].Task;
        }

        public async Task<bool> ShowNextDialog(Character talker, string text)
        {
            this.Responses[talker] = new TaskCompletionSource<bool>();

            this.SendDialog(talker, text, NpcMessageType.Standard, 0, 1);

            return await this.Responses[talker].Task;
        }

        public async Task<bool> ShowNextPreviousDialog(Character talker, string text)
        {
            this.Responses[talker] = new TaskCompletionSource<bool>();

            this.SendDialog(talker, text, NpcMessageType.Standard, 1, 1);

            return await this.Responses[talker].Task;
        }

        public async Task<bool> ShowPreviousOkDialog(Character talker, string text)
        {
            this.Responses[talker] = new TaskCompletionSource<bool>();

            this.SendDialog(talker, text, NpcMessageType.Standard, 1, 0);

            return await this.Responses[talker].Task;
        }

        public async Task<bool> ShowYesNoDialog(Character talker, string text)
        {
            this.Responses[talker] = new TaskCompletionSource<bool>();

            this.SendDialog(talker, text, NpcMessageType.YesNo);

            return await this.Responses[talker].Task;
        }

        public async Task<int> ShowChoiceDialog(Character talker, string text, params string[] choices)
        {
            this.Choices[talker] = new TaskCompletionSource<int>();

            text += "#b\r\n";

            for (int i = 0; i < choices.Length; i++)
            {
                text += string.Format("#L{0}#{1}#l\r\n", i, choices[i]);
            }

            this.SendDialog(talker, text, NpcMessageType.Choice);

            return await this.Choices[talker].Task;
        }

        public async Task<int> ShowStyleRequestDialog(Character talker, string text, params int[] styleChoices)
        {
            this.Choices[talker] = new TaskCompletionSource<int>();

            List<int> validStyles = new List<int>();

            foreach (int loopStyle in styleChoices)
            {
                if (DataProvider.Styles.Skins.Contains((byte)loopStyle) ||
                    DataProvider.Styles.MaleHairs.Contains(loopStyle) ||
                    DataProvider.Styles.FemaleHairs.Contains(loopStyle) ||
                    DataProvider.Styles.MaleFaces.Contains(loopStyle) ||
                    DataProvider.Styles.FemaleFaces.Contains(loopStyle))
                {
                    validStyles.Add(loopStyle);
                }
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ScriptMessage))
            {
                oPacket
                     .WriteByte(4) // NOTE: Unknown.
                     .WriteInt(this.MapleID)
                     .WriteByte((byte)NpcMessageType.RequestStyle)
                     .WriteByte() // NOTE: Speaker.
                     .WriteMapleString(text)
                     .WriteByte((byte)validStyles.Count);

                foreach (int loopStyle in validStyles)
                {
                    oPacket.WriteInt(loopStyle);
                }

                talker.Client.Send(oPacket);
            }

            this.StyleSelectionHelpers[talker] = validStyles.ToArray();

            return await this.Choices[talker].Task;
        }

        private void SendDialog(Character talker, string text, NpcMessageType messageType, params byte[] footer)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ScriptMessage))
            {
                oPacket
                    .WriteByte(4) // NOTE: Unknown.
                    .WriteInt(this.MapleID)
                    .WriteByte((byte)messageType)
                    .WriteByte() // NOTE: Speaker.
                    .WriteMapleString(text)
                    .WriteBytes(footer);

                talker.Client.Send(oPacket);
            }
        }

        public OutPacket GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            return this.GetInternalPacket(false);
        }

        public OutPacket GetControlRequestPacket()
        {
            return this.GetInternalPacket(true);
        }

        private OutPacket GetInternalPacket(bool requestControl)
        {
            OutPacket oPacket = new OutPacket(requestControl ? ServerOperationCode.NpcChangeController : ServerOperationCode.NpcEnterField);

            if (requestControl)
            {
                oPacket.WriteBool(true);
            }

            oPacket
                .WriteInt(this.ObjectID)
                .WriteInt(this.MapleID)
                .WritePoint(this.Position)
                .WriteBool(!this.FacesLeft)
                .WriteShort(this.Foothold)
                .WriteShort(this.MinimumClickX)
                .WriteShort(this.MaximumClickX)
                .WriteBool(true); // NOTE: Hide.

            return oPacket;
        }

        public OutPacket GetControlCancelPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.NpcChangeController);

            oPacket
                .WriteBool()
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.NpcLeaveField);

            oPacket.WriteInt(this.ObjectID);

            return oPacket;
        }
    }
}
