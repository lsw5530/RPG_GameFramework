using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameMessage;

public static class ETPackets {
    public static readonly int ET_PacketSizeLength = 2;

    public static readonly int ET_MessageIdentifyLength = 3;
    public static readonly int ET_MessageFlagIndex = 2;
    public static readonly int ET_MessageOpcodeIndex = 3;

}
namespace GameMessage {
    public partial class ReqLogin : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqLogin;
            }
        }
        public override void Clear() {
            this.error = 0;
            this.Acct = string.Empty;
            this.Pass = string.Empty;
        }
    }
    public partial class RspLogin : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspLogin;
            }
        }
        public override void Clear() {
            this.error = 0;
        }
    }
    public partial class ReqRename : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqRename;
            }
        }
        public override void Clear() {
            this.error = 0;
            this.name_ = string.Empty;
        }
    }
    public partial class RspRename : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspRename;
            }
        }
        public override void Clear() {
            this.error = 0;
            this.Name = string.Empty;
        }
    }
    public partial class ReqHeartbeat : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqHeartbeat;
            }
        }
        public override void Clear() {
            this.error = 0;
            this.LocalTime = 0;
        }
    }
    public partial class RspHeartbeat : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspHeartbeat;
            }
        }
        public override void Clear() {
            this.LocalTime = 0;
            this.ServerTime = 0;
        }
    }
    public partial class ReqSecret : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqSecret;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspSecret : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspSecret;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqGuide : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqGuide;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspGuide : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspGuide;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqStrong : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqStrong;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspStrong : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspStrong;
            }
        }
        public override void Clear() {

        }
    }
    public partial class SndChat : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.SndChat;
            }
        }
        public override void Clear() {

        }
    }
    public partial class PshChat : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.PshChat;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqBuy : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqBuy;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspBuy : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspBuy;
            }
        }
        public override void Clear() {

        }
    }
    public partial class PshPower : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.PshPower;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqTakeTaskReward : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqTakeTaskReward;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspTakeTaskReward : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspTakeTaskReward;
            }
        }
        public override void Clear() {

        }
    }
    public partial class PshTaskPrgs : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.PshTaskPrgs;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqFBFight : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqFBFight;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspFBFight : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspFBFight;
            }
        }
        public override void Clear() {

        }
    }
    public partial class ReqFBFightEnd : CSPacketBase {
        public override int Id {
            get {
                return (int)CMD.ReqFBFightEnd;
            }
        }
        public override void Clear() {

        }
    }
    public partial class RspFBFightEnd : SCPacketBase {
        public override int Id {
            get {
                return (int)CMD.RspFBFightEnd;
            }
        }
        public override void Clear() {

        }
    }
}