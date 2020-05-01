/****************************************************
    文件：NetSvc.cs
	
    
	功能：网络服务
*****************************************************/

using System;
using System.Net;
using GameFramework;
using GameFramework.Event;
using GameMessage;
using UnityEngine;
using UnityGameFramework.Runtime;
[DisallowMultipleComponent]
[AddComponentMenu("Game Framework/Net")]
public class NetComponent : GameFrameworkComponent, ICustomComponent {
    private static readonly string obj = "lock";
    ET_NetworkChannelHelper m_CustomNetworkChannelHelper;
    GameFramework.Network.INetworkChannel m_NetworkChannel;

    public bool IsConnectedToServer { get; set; }

    [Header("每帧最大发送数量")]
    public int MaxSendCount = 5;

    [Header("每次发包最大的字节")]
    public int MaxSendByteCount = 1024;

    [Header("每帧最大处理包数量")]
    public int MaxReceiveCount = 5;

    [Header("心跳间隔")]
    private int m_PingInterval = 5;

    /// <summary>
    /// 上次心跳时间
    /// </summary>
    private long m_LastPingTime = 0;
    private long m_LastPongTime = 0;
    private int m_ConnectTime = 0;

    public void InitSvc() {
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
        GameEntry.Event.Subscribe(UnityGameFramework.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
        ConnectServer();
        PECommon.Log("Init NetSvc...");
    }
    private void ConnectServer() {
        if (m_CustomNetworkChannelHelper != null) {
            m_CustomNetworkChannelHelper.Shutdown();
        }
        if (m_NetworkChannel != null) {
            m_NetworkChannel.Close();
        }
        m_CustomNetworkChannelHelper = new ET_NetworkChannelHelper();
        m_NetworkChannel = GameEntry.Network.CreateNetworkChannel("tcp" + m_ConnectTime++, GameFramework.Network.ServiceType.TcpWithSyncReceive, m_CustomNetworkChannelHelper);
        IPAddress iPAddress = IPAddress.Parse("192.168.31.32");
        //IPAddress iPAddress = IPAddress.Parse("129.28.170.32");
        m_NetworkChannel.Connect(iPAddress, SrvCfg.srvPort);
        //StartCoroutine(client.CheckNet());
    }
    /// <summary>
    /// 链接服务器的第一个请求
    /// </summary>
    public void SecretRequest() {
        ReqSecret msg = new ReqSecret();
        msg.Secret = ProtoHelper.PublicKey;
        SendMsg(msg);
    }
    private void OnNetworkConnected(object sender, GameEventArgs e) {
        NetworkConnectedEventArgs ne = (NetworkConnectedEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel) {
            return;
        }
        //Debug.LogFormat("Network channel '{0}' connected, local address '{1}', remote address '{2}'.", ne.NetworkChannel.Name, ne.NetworkChannel.Socket.LocalEndPoint.ToString(), ne.NetworkChannel.Socket.RemoteEndPoint.ToString());
        m_LastPingTime = GameEntry.Timer.GetTimeStamp();
        m_LastPongTime = GameEntry.Timer.GetTimeStamp();
        IsConnectedToServer = true;

        //SecretRequest();
    }
    private void OnNetworkClosed(object sender, GameEventArgs e) {
        UnityGameFramework.Runtime.NetworkClosedEventArgs ne = (UnityGameFramework.Runtime.NetworkClosedEventArgs)e;
        if (ne.NetworkChannel != m_NetworkChannel) {
            return;
        }
        IsConnectedToServer = false;
        //procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Login);
        //ChangeState<ProcedureChangeScene>(procedureOwner);
        GameFramework.Procedure.ProcedureBase currentProcedure = GameEntry.Procedure.CurrentProcedure;
    }
    public void SendMsg(CSPacketBase msg) {
        if (IsConnectedToServer && m_NetworkChannel.Connected) {
            m_NetworkChannel.Send(msg);
        }
        else {
            Debug.LogError("m_NetworkChannel.Connected:" + m_NetworkChannel.Connected);
            GameEntry.UI.AddTips("服务器未连接");
            ConnectServer();
        }
    }

    private void Update() {
        if (IsConnectedToServer) {
            long timeNow = GameEntry.Timer.GetTimeStamp();
            if (timeNow > m_LastPingTime + m_PingInterval) {
                //发送心跳
                m_LastPingTime = timeNow;
                ReqHeartbeat proto = new ReqHeartbeat();
                proto.LocalTime = timeNow;
                Debug.Log("send heartbeat");
                SendMsg(proto);
            }
            //如果心跳包过长时间没收到，就关闭连接
            if (timeNow - m_LastPongTime > m_PingInterval * 1.5f) {
                GameEntry.UI.AddTips("服务器断开连接"); 
                Debug.Log("DisConnect To Server" );
                ReloginEventArgs eventArgs = ReferencePool.Acquire<ReloginEventArgs>();
                GameEntry.Event.Fire(this, eventArgs);
                m_NetworkChannel.Close();
                IsConnectedToServer = false;
            }
        }
    }
    public void ProcessMsg(SCPacketBase msg) {
        m_LastPongTime = GameEntry.Timer.GetTimeStamp();
        Debug.Log("receive from server：" + (CMD)msg.Id);
        if (msg.error != (int)ErrorCode.None) {
            switch ((ErrorCode)msg.error) {
                case ErrorCode.ServerDataError:
                    PECommon.Log("服务器数据异常", LogType.Error);
                    GameEntry.UI.AddTips("客户端数据异常");
                    break;
                case ErrorCode.UpdateDBError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameEntry.UI.AddTips("网络不稳定");
                    break;
                case ErrorCode.ClientDataError:
                    PECommon.Log("客户端数据异常", LogType.Error);
                    break;
                case ErrorCode.AcctIsOnline:
                    GameEntry.UI.AddTips("当前账号已经上线");
                    break;
                case ErrorCode.WrongPass:
                    GameEntry.UI.AddTips("密码错误");
                    break;
                case ErrorCode.LackLevel:
                    GameEntry.UI.AddTips("角色等级不够");
                    break;
                case ErrorCode.LackCoin:
                    GameEntry.UI.AddTips("金币数量不够");
                    break;
                case ErrorCode.LackCrystal:
                    GameEntry.UI.AddTips("水晶数量不够");
                    break;
                case ErrorCode.LackDiamond:
                    GameEntry.UI.AddTips("钻石数量不够");
                    break;
                case ErrorCode.LackPower:
                    GameEntry.UI.AddTips("体力值不足");
                    break;
            }
            return;
        }
        OnReceiveSCPacketBaseEventArgs eventArgs = ReferencePool.Acquire<OnReceiveSCPacketBaseEventArgs>();
        eventArgs.Fill((CMD)msg.Id, msg);
        GameEntry.Event.FireNow(this, eventArgs);
    }
    public void Close() {
        if (m_NetworkChannel != null)
            m_NetworkChannel.Close();
    }
}