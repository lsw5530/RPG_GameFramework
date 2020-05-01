///****************************************************
//	文件：ServerSession.cs


//		功能：网络会话连接
//*****************************************************/

//using System;
//using GameMessage;
//using KDNet;
////using KDProtocol;

//public class ServerSession :ClientSocket {
//    public override void OnConnected() {
//        sessionID = ServerRoot.Instance.GetSessionID();
//        LastPingTime = NetSvc.GetTimeStamp();
//        KDCommon.Log("SessionID:" + sessionID + " Client Connect");
//    }

//    public override void OnReciveMsg(GameMsg msg) {
//        KDCommon.Log("SessionID: " + sessionID + "   RcvPack CMD:" + ((CMD)msg.cmd).ToString());
//        NetSvc.Instance.AddMsgQue(this, msg);
//    }

//    public override void OnDisConnected() {
//        LoginSys.Instance.ClearOfflineData(this);
//        KDCommon.Log("SessionID:" + sessionID + " Client Offline"+ "，在线人数：" + CacheSvc.Instance.GetOnlineServerSessions().Count);
//    }

    
//}
