/****************************************************
	文件：ServerStart.cs


	
	功能：服务器入口
*****************************************************/

using System;
using System.IO;
using System.Threading;
using GameMessage;
using Google.Protobuf;

class ServerStart {
    private static Thread m_MsgThread;

    static void Main(string[] args) {
        //m_MsgThread = new Thread(MsgThread);
        //m_MsgThread.IsBackground = true;
        //m_MsgThread.Start();

        ServerRoot.Instance.Init();
        while (true) {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);
        }
        Debug.LogError("jjj");
    }

    private static void MsgThread() {
        while (true) {
            ServerRoot.Instance.Update();
            Thread.Sleep(20);
        }
    }

    public static T Deserialize<T>(MessageParser _type, byte[] byteData) {
        Stream stream = new MemoryStream(byteData);
        if (stream != null) {
            IMessage t = _type.ParseFrom(stream);
            stream.Close();
            return (T)t;
        }
        stream.Close();
        return default(T);
    }
    public static byte[] Serialize(IMessage _data) {
        MemoryStream stream = new MemoryStream();
        if (stream != null) {
            _data.WriteTo(stream);
            byte[] bytes = stream.ToArray();
            stream.Close();
            return bytes;
        }
        stream.Close();
        return null;
    }
}