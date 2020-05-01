using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameMessage {
    //public enum PacketType : byte {
    //    /// <summary>
    //    /// 未定义。
    //    /// </summary>
    //    Undefined = 0,

    //    /// <summary>
    //    /// 客户端发往服务器的包。
    //    /// </summary>
    //    ClientToServer,

    //    /// <summary>
    //    /// 服务器发往客户端的包。
    //    /// </summary>
    //    ServerToClient,
    //} 
    //public abstract class BaseEventArgs {
    //    /// <summary>
    //    /// 获取类型编号。
    //    /// </summary>
    //    public abstract int Id {
    //        get;
    //    }
    //}
    //public abstract class Packet : BaseEventArgs {
    //}
    //public abstract class PacketBase : Packet {

    //    public PacketBase() {
    //    }

    //    public abstract PacketType PacketType {
    //        get;
    //    }
    //}
    //public abstract class CSPacketBase : PacketBase {
    //    public override PacketType PacketType {
    //        get {
    //            return PacketType.ClientToServer;
    //        }
    //    }
    //}
    //public abstract class SCPacketBase : PacketBase {
    //    public override PacketType PacketType {
    //        get {
    //            return PacketType.ServerToClient;
    //        }
    //    }
    //}
    //public class CSPacketBase : CSPacketBase {
    //    public override int Id { get; }

    //    public int error { get; set; }

    //    public override void Clear() {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class SCPacketBase : SCPacketBase {
    //    public override int Id { get; }

    //    public int error { get; set; }

    //    public override void Clear() {
    //        throw new NotImplementedException();
    //    }
    //}
    //public interface GameMsg : Google.Protobuf.IMessage {
    //    int cmd { get; }
    //    int error { get; set; }
    //}
    public enum ErrorCode {
        None = 0,//没有错误
        ServerDataError,//服务器数据异常
        UpdateDBError,//更新数据库错误
        ClientDataError,//客户端数据异常

        AcctIsOnline,//账号已经上线
        WrongPass,//密码错误
        NameIsExist,//名字已经存在

        LackLevel,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower,
    }

    public enum CMD {
        None = 0,
        //登录相关 100
        ReqLogin = 101,
        RspLogin = 102,

        ReqRename = 103,
        RspRename = 104,
        ReqHeartbeat = 105,
        RspHeartbeat = 106,
        ReqSecret = 107,
        RspSecret = 108,

        //主城相关 200
        ReqGuide = 201,
        RspGuide = 202,

        ReqStrong = 203,
        RspStrong = 204,

        SndChat = 205,
        PshChat = 206,

        ReqBuy = 207,
        RspBuy = 208,

        PshPower = 209,

        ReqTakeTaskReward = 210,
        RspTakeTaskReward = 211,

        PshTaskPrgs = 212,

        ReqFBFight = 301,
        RspFBFight = 302,

        ReqFBFightEnd = 303,
        RspFBFightEnd = 304,
    }

    public class SrvCfg {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 17666;
    }
}
