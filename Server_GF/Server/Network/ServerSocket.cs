using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using GameFramework.Network;
using GameMessage;


public class ServerSocket : Singleton<ServerSocket> {
    //公钥
    public static string PublicKey = "OceanSever";
    //密钥，后续可以随时间进行变化
    public static string SecretKey = "Ocean_Up&&NB!!";

#if DEBUG
    //private string m_IpStr = "127.0.0.1";
#else
        //对应阿里云或腾讯云的 本地ip地址（不是公共ip地址）
        private string m_IpStr = "172.45.756.54";
#endif
    //private const int m_Port = 8011;

    //public static long m_PingInterval = 30;

    //服务器监听socket
    private static Socket m_ListenSocket;

    //临时保存所有socket的集合
    private static List<Socket> m_CheckReadList = new List<Socket>();

    //所有客户端的一个字典
    public static Dictionary<Socket, ClientSocket> m_ClientDic = new Dictionary<Socket, ClientSocket>();

    public static List<ClientSocket> m_TempList = new List<ClientSocket>();

    public void StartAsServer(string ipstr, int port) {
        IPAddress ip = IPAddress.Parse(ipstr);
        IPEndPoint ipEndPoint = new IPEndPoint(ip, port);
        m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_ListenSocket.Bind(ipEndPoint);
        m_ListenSocket.Listen(50000);
        ET_NetworkChannelHelper.Initialize();
        Debug.Log("服务器启动监听{0}成功", m_ListenSocket.LocalEndPoint.ToString());

        //while (true) {
        //    //处理找出所有socket
        //    ResetCheckRead();

        //    try {
        //        //最后等待时间单位是微妙
        //        Socket.Select(m_CheckReadList, null, null, 1000);
        //    }
        //    catch (Exception e) {
        //        Debug.LogError(e);
        //    }

        //    for (int i = m_CheckReadList.Count - 1; i >= 0; i--) {
        //        Socket s = m_CheckReadList[i];
        //        if (s == m_ListenSocket) {
        //            //说明有客户端链接到服务器了，所以服务器socket可读
        //            ReadListen(s);
        //        }
        //        else {
        //            //说明链接的客户端可读，证明有信息传上来了
        //            ReadClient(s);
        //        }
        //    }

        //    //检测是否心跳包超时的计算


        //    long timeNow = GetTimeStamp();
        //    m_TempList.Clear();

        //    foreach (ClientSocket clientSocket in m_TempList) {
        //        CloseClient(clientSocket);
        //    }
        //    m_TempList.Clear();
        //}
    }
    public void MsgSecret(ClientSocket c, CSPacketBase msgBase) {
        RspSecret msgSecret = new RspSecret();
        msgSecret.Secret = ServerSocket.SecretKey;
        Send(c, msgSecret);
    }
    public void Update() {
        //检查是否有读取的socket

        //处理找出所有socket
        ResetCheckRead();

        try {
            //最后等待时间单位是微妙
            Socket.Select(m_CheckReadList, null, null, 1000);
        }
        catch (Exception e) {
            Debug.LogError(e);
        }

        for (int i = m_CheckReadList.Count - 1; i >= 0; i--) {
            Socket s = m_CheckReadList[i];
            if (s == m_ListenSocket) {
                //说明有客户端链接到服务器了，所以服务器socket可读
                ReadListen(s);
            }
            else {
                //说明链接的客户端可读，证明有信息传上来了
                ReadClient(s);
            }
        }

        //检测是否心跳包超时的计算


        long timeNow = GetTimeStamp();
        m_TempList.Clear();

        foreach (ClientSocket clientSocket in m_TempList) {
            CloseClient(clientSocket);
        }
        m_TempList.Clear();
    }
    public void ResetCheckRead() {
        m_CheckReadList.Clear();
        m_CheckReadList.Add(m_ListenSocket);
        foreach (Socket s in m_ClientDic.Keys) {
            m_CheckReadList.Add(s);
        }
    }

    void ReadListen(Socket listen) {
        try {
            Socket client = listen.Accept();
            ClientSocket clientSocket = new ClientSocket(this);
            clientSocket.Socket = client;
            clientSocket.LastPingTime = GetTimeStamp();
            m_ClientDic.Add(client, clientSocket);
            clientSocket.OnConnected();
            Debug.Log("一个客户端链接：{0},当前{1}个客户端在线！", client.LocalEndPoint.ToString(), m_ClientDic.Count);
        }
        catch (SocketException ex) {
            Debug.LogError("Accept fali:" + ex.ToString());
        }
    }

    void ReadClient(Socket client) {
        ClientSocket clientSocket = null;
        if (!m_ClientDic.TryGetValue(client, out clientSocket)) {
            return;
        }
        ReceiveState readBuff = clientSocket.ReceiveState;

        //接受信息，根据信息解析协议，根据协议内容处理消息再下发到客户端
        if (readBuff.Stream.Position >= readBuff.Stream.Length) {//如果上一次接收数据刚好收满
            OnReceiveData(clientSocket);
        }

        int count = 0;
        try {
            //count = client.Receive(readBuff.Bytes, readBuff.WriteIdx, readBuff.Remain, 0);
            count = client.Receive(readBuff.Stream.GetBuffer(), (int)readBuff.Stream.Position,
                (int)(readBuff.Stream.Length - readBuff.Stream.Position), SocketFlags.None);
            readBuff.Stream.Position += count;
        }
        catch (SocketException ex) {
            Debug.LogError("Receive fali:" + ex);
            CloseClient(clientSocket);
            return;
        }
        //代表客户端断开链接了
        if (count <= 0) {
            CloseClient(clientSocket);
            return;
        }
        OnReceiveData(clientSocket);
    }
    //解析我们的信息
    void OnReceiveData(ClientSocket clientSocket) {
        ReceiveState readbuff = clientSocket.ReceiveState;
        if (readbuff.Stream.Position < readbuff.Stream.Length) {//未接收满包头或包体
            return;
        }
        readbuff.Stream.Position = 0L;

        bool processSuccess = false;
        if (readbuff.PacketHeader != null) {
            processSuccess = ProcessPacket(clientSocket);
        }
        else {
            processSuccess = ProcessPacketHeader(clientSocket);
        }

        if (processSuccess) {
            //ReceiveAsync(clientSocket);
            return;
        }

    }

    private void ReceiveAsync(ClientSocket clientSocket) {
        Socket socket = clientSocket.Socket;
        if (!socket.Connected) return;
        ReceiveState readBuff = clientSocket.ReceiveState;
        try {
            int count = socket.Receive(readBuff.Stream.GetBuffer(), (int)readBuff.Stream.Position,
                  (int)(readBuff.Stream.Length - readBuff.Stream.Position), SocketFlags.None);
            readBuff.Stream.Position += count;
            if (count <= 0) {
                CloseClient(clientSocket);
                return;
            }
            OnReceiveData(clientSocket);
        }
        catch (Exception exception) {
            Debug.LogError("客户端连接问题：" + exception);
            CloseClient(clientSocket);
            //throw;
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="cs"></param>
    /// <param name="msgBase"></param>
    public void Send(ClientSocket cs, SCPacketBase msgBase) {
        KDCommon.Log("send=" +(CMD) msgBase.Id, LogType.Info);
        if (cs == null || !cs.Socket.Connected) {
            return;
        }
        bool serializeResult = false;
        try {
            cs.SendState.Reset();
            serializeResult = ET_NetworkChannelHelper.Serialize(msgBase, cs.SendState.Stream);

            try {
                if (serializeResult) {
                    cs.SendState.Stream.Position = 0L;
                    SendAsync(cs);
                }
            }
            catch (SocketException ex) {
                Debug.LogError("Socket BeginSend Error：" + ex);
            }
        }
        catch (SocketException ex) {
            Debug.LogError("Socket发送数据失败：" + ex);
        }
        cs.SendState.Stream.Position = 0L;
    }
    private void SendAsync(ClientSocket clientSocket) {
        try {
            int size = (int)(clientSocket.SendState.Stream.Length - clientSocket.SendState.Stream.Position);
            if (size == 0)
                Debug.LogError("size=0,====================");
            Byte[] buffer = clientSocket.SendState.Stream.GetBuffer();
            //clientSocket.Socket.Send(buffer);
            //clientSocket.SendState.Reset();
            clientSocket.Socket.BeginSend(clientSocket.SendState.Stream.GetBuffer(),
                (int)clientSocket.SendState.Stream.Position,
                (int)(clientSocket.SendState.Stream.Length - clientSocket.SendState.Stream.Position),
                SocketFlags.None, null, null);
        }
        catch (Exception exception) {
            Debug.LogError("发送失败：" + exception.ToString());
            throw;
        }
    }
    private void SendCallback(IAsyncResult ar) {
        ClientSocket clientSocket = (ClientSocket)ar.AsyncState;
        Socket socket = clientSocket.Socket;
        if (!socket.Connected) {
            return;
        }

        int bytesSent;
        try {
            bytesSent = socket.EndSend(ar);
        }
        catch (Exception exception) {
            Debug.LogError("发送失败：" + exception.ToString());
            throw;
        }

        clientSocket.SendState.Stream.Position += bytesSent;
        if (clientSocket.SendState.Stream.Position < clientSocket.SendState.Stream.Length) {
            SendAsync(clientSocket);
            Debug.LogError("继续发送");
            return;
        }
        clientSocket.SendState.Reset();
    }

    public void CloseClient(ClientSocket client) {
        if (!m_ClientDic.ContainsKey(client.Socket)) return;
        client.Socket.Close();
        client.OnDisConnected();
        m_ClientDic.Remove(client.Socket);
        Debug.Log("一个客户端断开链接，当前总连接数：{0}", m_ClientDic.Count);
    }


    public static long GetTimeStamp() {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    public bool ProcessPacketHeader(ClientSocket clientSocket) {
        try {
            IPacketHeader packetHeader = ET_NetworkChannelHelper.DeserializePacketHeader(clientSocket.ReceiveState.Stream);

            if (packetHeader == null) {
                string errorMessage = "Packet header is invalid.";
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            clientSocket.ReceiveState.PrepareForPacket(packetHeader);
            if (packetHeader.PacketLength <= 0) { //一般不会满足
                return ProcessPacket(clientSocket);
            }
        }
        catch (Exception exception) {
            Debug.LogError(exception);
            throw;
        }

        return true;
    }
    private bool ProcessPacket(ClientSocket clientSocket) {

        try {
            Packet packet = ET_NetworkChannelHelper.DeserializePacket(clientSocket.ReceiveState.PacketHeader, clientSocket.ReceiveState.Stream);

            if (packet != null) {
                //分发消息 lsw
                CSPacketBase cSPacketBase = (CSPacketBase)packet;
                if (cSPacketBase != null)
                    clientSocket.OnReciveMsg(cSPacketBase);
                else
                    Debug.LogError("解析失败！");

                //C2R_Login r_Login = ((C2R_Login)packet);
                //Debug.LogError(r_Login.Account);
                //Debug.LogError(r_Login.Password);
                ////m_ReceivePacketPool.Fire(this, packet);
            }
            else {
                Debug.LogError("解析包失败");
            }

            clientSocket.ReceiveState.PrepareForPacketHeader(5/*m_NetworkChannelHelper.PacketHeaderLength*/);
        }
        catch (Exception exception) {
            Debug.LogError(exception);

            throw;
        }

        return true;
    }
}
