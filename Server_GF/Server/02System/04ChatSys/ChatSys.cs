/****************************************************
	文件：ChatSys.cs


	
	功能：聊天业务系统
*****************************************************/

using System.Collections.Generic;
using GameMessage;
//using KDProtocol;

public class ChatSys {
    private static ChatSys instance = null;
    public static ChatSys Instance {
        get {
            if (instance == null) {
                instance = new ChatSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        KDCommon.Log("ChatSys Init Done.");
    }

    public void SndChat(MsgPack pack) {
        SndChat data = (SndChat)pack.msg;
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        //任务进度数据更新
        TaskSys.Instance.CalcTaskPrgs(pd, 6);

        SCPacketBase msg = new PshChat {
            Name = pd.Name,
            Chat = data.Chat
        };

        //广播所有在线客户端
        List<ClientSocket> lst = cacheSvc.GetOnlineServerSessions();
        //byte[] bytes = KDNet.KDTool.PackNetMsg(msg);
        for (int i = 0; i < lst.Count; i++) {
            lst[i].SendMsg(msg);
        }
    }
}
