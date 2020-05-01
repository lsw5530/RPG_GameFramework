//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Reflection;
//using GameMessage;


//public class ServerSocket111 : Singleton<ServerSocket> {
//    //公钥
//    public static string PublicKey = "OceanSever";
//    //密钥，后续可以随时间进行变化
//    public static string SecretKey = "Ocean_Up&&NB!!";

//#if DEBUG
//    //private string m_IpStr = "127.0.0.1";
//#else
//        //对应阿里云或腾讯云的 本地ip地址（不是公共ip地址）
//        private string m_IpStr = "172.45.756.54";
//#endif
//    //private const int m_Port = 8011;

//    //public static long m_PingInterval = 30;

//    //服务器监听socket
//    private static Socket m_ListenSocket;

//    //临时保存所有socket的集合
//    private static List<Socket> m_CheckReadList = new List<Socket>();

//    //所有客户端的一个字典
//    public static Dictionary<Socket, ClientSocket> m_ClientDic = new Dictionary<Socket, ClientSocket>();

//    public static List<ClientSocket> m_TempList = new List<ClientSocket>();

//    public void StartAsServer(string ipstr, int port) {
//        IPAddress ip = IPAddress.Parse(ipstr);
//        IPEndPoint ipEndPoint = new IPEndPoint(ip, port);
//        m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//        m_ListenSocket.Bind(ipEndPoint);
//        m_ListenSocket.Listen(50000);

//        Debug.Log("服务器启动监听{0}成功", m_ListenSocket.LocalEndPoint.ToString());
//    }
//    public void MsgSecret(ClientSocket c, GameMsg msgBase) {
//        MsgSecret msgSecret = (MsgSecret)msgBase;
//        msgSecret.Secret = ServerSocket.SecretKey;
//        ServerSocket.Send(c, msgSecret);
//    }
//    public void Update() {
//        //检查是否有读取的socket

//        //处理找出所有socket
//        ResetCheckRead();

//        try {
//            //最后等待时间单位是微妙
//            Socket.Select(m_CheckReadList, null, null, 1000);
//        }
//        catch (Exception e) {
//            Debug.LogError(e);
//        }

//        for (int i = m_CheckReadList.Count - 1; i >= 0; i--) {
//            Socket s = m_CheckReadList[i];
//            if (s == m_ListenSocket) {
//                //说明有客户端链接到服务器了，所以服务器socket可读
//                ReadListen(s);
//            }
//            else {
//                //说明链接的客户端可读，证明有信息传上来了
//                ReadClient(s);
//            }
//        }

//        //检测是否心跳包超时的计算


//        long timeNow = GetTimeStamp();
//        m_TempList.Clear();

//        foreach (ClientSocket clientSocket in m_TempList) {
//            CloseClient(clientSocket);
//        }
//        m_TempList.Clear();
//    }
//    public void ResetCheckRead() {
//        m_CheckReadList.Clear();
//        m_CheckReadList.Add(m_ListenSocket);
//        foreach (Socket s in m_ClientDic.Keys) {
//            m_CheckReadList.Add(s);
//        }
//    }

//    void ReadListen(Socket listen) {
//        try {
//            Socket client = listen.Accept();
//            ClientSocket clientSocket = new ClientSocket();
//            clientSocket.Socket = client;
//            clientSocket.LastPingTime = GetTimeStamp();
//            m_ClientDic.Add(client, clientSocket);
//            clientSocket.OnConnected();
//            Debug.Log("一个客户端链接：{0},当前{1}个客户端在线！", client.LocalEndPoint.ToString(), m_ClientDic.Count);
//        }
//        catch (SocketException ex) {
//            Debug.LogError("Accept fali:" + ex.ToString());
//        }
//    }

//    void ReadClient(Socket client) {
//        ClientSocket clientSocket = m_ClientDic[client];
//        ByteArray readBuff = clientSocket.ReadBuff;
//        //接受信息，根据信息解析协议，根据协议内容处理消息再下发到客户端
//        int count = 0;
//        //如果上一次接收数据刚好占满了1024的数组，
//        if (readBuff.Remain <= 0) {
//            //数据移动到index =0 位置。
//            OnReceiveData(clientSocket);
//            readBuff.CheckAndMoveBytes();
//            //保证到如果数据长度大于默认长度，扩充数据长度，保证信息的正常接收
//            while (readBuff.Remain <= 0) {
//                int expandSize = readBuff.Length < ByteArray.DEFAULT_SIZE ? ByteArray.DEFAULT_SIZE : readBuff.Length;
//                readBuff.ReSize(expandSize * 2);
//            }
//        }
//        try {
//            count = client.Receive(readBuff.Bytes, readBuff.WriteIdx, readBuff.Remain, 0);
//        }
//        catch (SocketException ex) {
//            Debug.LogError("Receive fali:" + ex);
//            CloseClient(clientSocket);
//            return;
//        }

//        //代表客户端断开链接了
//        if (count <= 0) {
//            CloseClient(clientSocket);
//            return;
//        }

//        readBuff.WriteIdx += count;
//        //解析我们的信息
//        OnReceiveData(clientSocket);
//        readBuff.CheckAndMoveBytes();
//    }

//    /// <summary>
//    /// 接收数据处理
//    /// </summary>
//    /// <param name="clientSocket"></param>
//    void OnReceiveData(ClientSocket clientSocket) {
//        ByteArray readbuff = clientSocket.ReadBuff;
//        //基本消息长度判断
//        if (readbuff.Length <= 4 || readbuff.ReadIdx < 0) {
//            return;
//        }
//        int readIdx = readbuff.ReadIdx;
//        byte[] bytes = readbuff.Bytes;
//        int bodyLength = BitConverter.ToInt32(bytes, readIdx);
//        //判断接收到的信息长度是否小于包体长度+包体头长度，如果小于，代表我们的信息不全，大于代表信息全了（有可能有粘包存在）
//        if (readbuff.Length < bodyLength + 4) {
//            return;
//        }
//        readbuff.ReadIdx += 4;
//        //解析协议名
//        int nameCount = 0;
//        CMD proto = CMD.None;
//        try {
//            proto = ProtoHelper.DecodeName(readbuff.Bytes, readbuff.ReadIdx, out nameCount);
//        }
//        catch (Exception ex) {
//            Debug.LogError("解析协议名出错：" + ex);
//            CloseClient(clientSocket);
//            return;
//        }

//        if (proto == CMD.None) {
//            Debug.LogError("OnReceiveData MsgBase.DecodeName  fail");
//            CloseClient(clientSocket);
//            return;
//        }

//        readbuff.ReadIdx += nameCount;

//        //解析协议体
//        int bodyCount = bodyLength - nameCount;
//        GameMsg msgBase = null;
//        try {
//            msgBase = ProtoHelper.Decode(proto, readbuff.Bytes, readbuff.ReadIdx, bodyCount);
//            if (msgBase == null) {
//                Debug.LogError("{0}协议内容解析错误：" + proto.ToString());
//                CloseClient(clientSocket);
//                return;
//            }
//        }
//        catch (Exception ex) {
//            Debug.LogError("接收数据协议内容解析错误：" + ex);
//            CloseClient(clientSocket);
//            return;
//        }

//        readbuff.ReadIdx += bodyCount;
//        readbuff.CheckAndMoveBytes();

//        //分发消息 lsw
//        clientSocket.OnReciveMsg(msgBase);

//        //继续读取消息
//        if (readbuff.Length > 4) {
//            OnReceiveData(clientSocket);
//        }
//    }


//    /// <summary>
//    /// 发送数据
//    /// </summary>
//    /// <param name="cs"></param>
//    /// <param name="msgBase"></param>
//    public static void Send(ClientSocket cs, GameMsg msgBase) {
//        if (cs == null || !cs.Socket.Connected) {
//            return;
//        }

//        try {
//            //分为三部分，头：总协议长度；名字；协议内容。
//            byte[] nameBytes = ProtoHelper.EncodeName(msgBase);
//            byte[] bodyBytes = ProtoHelper.Encond(msgBase);
//            int len = nameBytes.Length + bodyBytes.Length;
//            byte[] byteHead = BitConverter.GetBytes(len);
//            byte[] sendBytes = new byte[byteHead.Length + len];
//            Array.Copy(byteHead, 0, sendBytes, 0, byteHead.Length);
//            Array.Copy(nameBytes, 0, sendBytes, byteHead.Length, nameBytes.Length);
//            Array.Copy(bodyBytes, 0, sendBytes, byteHead.Length + nameBytes.Length, bodyBytes.Length);
//            try {
//                cs.Socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
//            }
//            catch (SocketException ex) {
//                Debug.LogError("Socket BeginSend Error：" + ex);
//            }
//        }
//        catch (SocketException ex) {
//            Debug.LogError("Socket发送数据失败：" + ex);
//        }
//    }

//    public void CloseClient(ClientSocket client) {
//        client.Socket.Close();
//        client.OnDisConnected();
//        m_ClientDic.Remove(client.Socket);
//        Debug.Log("一个客户端断开链接，当前总连接数：{0}", m_ClientDic.Count);
//    }


//    public static long GetTimeStamp() {
//        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
//        return Convert.ToInt64(ts.TotalSeconds);
//    }
//}
