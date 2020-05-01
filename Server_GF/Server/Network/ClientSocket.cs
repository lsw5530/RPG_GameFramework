using System; 
using System.Net.Sockets; 
using GameMessage;

public class ClientSocket {
    public Socket Socket { get; set; }
    public bool IsDestroyed { get; private set; }
    public long LastPingTime { get; set; } = 0;
    private ServerSocket m_ServerSocket;

    public ReceiveState ReceiveState { get;private set; }
    public SendState SendState { get; private set; }

    public int UserId = 0;
    public int sessionID = 0;
    public ClientSocket(ServerSocket serverSocket) {
        this.m_ServerSocket = serverSocket;
    }

    public void SendMsg(SCPacketBase msg) {
        m_ServerSocket.Send(this, msg);
    }
  
    /// <summary>
    /// Connect network
    /// </summary>
    public virtual void OnConnected() {
        sessionID = ServerRoot.Instance.GetSessionID();
        ReceiveState = new ReceiveState();
        SendState = new SendState();
        SendState.Reset();
        ReceiveState.PrepareForPacketHeader(5);
        LastPingTime = NetSvc.GetTimeStamp();
        KDCommon.Log("SessionID:" + sessionID + " Client Connect");
    }

    /// <summary>
    /// Receive network message
    /// </summary>
    public virtual void OnReciveMsg(CSPacketBase msg) {
        KDCommon.Log("SessionID: " + sessionID + "   ===RcvPack CMD:" + ((CMD)msg.Id).ToString());
        NetSvc.Instance.AddMsgQue(this, msg);
    }

    /// <summary>
    /// Disconnect network
    /// </summary>
    public virtual void OnDisConnected() {
        KDCommon.Log("SessionID:" + sessionID + " Client Offline" + "，在线人数：" + CacheSvc.Instance.GetOnlineServerSessions().Count);
        LoginSys.Instance.ClearOfflineData(this);
        SendState.Dispose();
        ReceiveState.Dispose();
    }
}
