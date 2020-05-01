/****************************************************
	文件：NetSvc.cs


		功能：网络服务
*****************************************************/

using GameFramework.Network;
using GameMessage;
using System;
using System.Collections.Generic;

public class MsgPack {
    public ClientSocket session;
    public CSPacketBase msg;
    public MsgPack(ClientSocket session, CSPacketBase msg) {
        this.session = session;
        this.msg = msg;
    }
}

public class NetSvc {
    private static NetSvc instance = null;
    public static NetSvc Instance {
        get {
            if (instance == null) {
                instance = new NetSvc();
            }
            return instance;
        }
    }
    public static readonly string obj = "lock";
    private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

    public void Init() {
#if true
        ServerSocket.Instance.StartAsServer("192.168.31.32", SrvCfg.srvPort);
#else
        ServerSocket.Instance.StartAsServer("172.30.0.7", SrvCfg.srvPort);
#endif
        KDCommon.Log("NetSvc Init Done.");
    }

    public void AddMsgQue(ClientSocket session, CSPacketBase msg) {
        lock (obj) {
            msgPackQue.Enqueue(new MsgPack(session, msg));
        }
    }

    public void Update() {
        ServerSocket.Instance.Update();
        if (msgPackQue.Count > 0) {
            lock (obj) {
                MsgPack pack = msgPackQue.Dequeue();
                HandOutMsg(pack);
            }
        }
    }

    internal void CloseClient(ClientSocket item) {
        ServerSocket.Instance.CloseClient(item);
    }
    private void HandOutMsg(MsgPack pack) {
        pack.session.LastPingTime = GetTimeStamp();
        switch ((CMD)pack.msg.Id) {
            case CMD.ReqLogin:
                LoginSys.Instance.ReqLogin(pack);
                break;
            case CMD.ReqRename:
                LoginSys.Instance.ReqRename(pack);
                break;
            case CMD.ReqSecret:
                ServerSocket.Instance.MsgSecret(pack.session, pack.msg);
                break;
            case CMD.ReqGuide:
                GuideSys.Instance.ReqGuide(pack);
                break;
            case CMD.ReqStrong:
                StrongSys.Instance.ReqStrong(pack);
                break;
            case CMD.SndChat:
                ChatSys.Instance.SndChat(pack);
                break;
            case CMD.ReqBuy:
                BuySys.Instance.ReqBuy(pack);
                break;
            case CMD.ReqTakeTaskReward:
                TaskSys.Instance.ReqTakeTaskReward(pack);
                break;
            case CMD.ReqFBFight:
                FubenSys.Instance.ReqFBFight(pack);
                break;
            case CMD.ReqFBFightEnd:
                FubenSys.Instance.ReqFBFightEnd(pack);
                break;
            case CMD.ReqHeartbeat:
                ReqHeartbeat(pack);
                break;
            default:
                Debug.LogError("无法解析："+pack);
                break;
        }
    }
    internal void ReqHeartbeat(MsgPack pack) {
        ReqHeartbeat data = (ReqHeartbeat)pack.msg;
        float localTime = data.LocalTime;

        RspHeartbeat proto = new RspHeartbeat(); 
        proto.LocalTime = localTime;
        proto.ServerTime = GetTimeStamp();
        pack.session.SendMsg(proto);
    }
    public static long GetTimeStamp() {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
}
