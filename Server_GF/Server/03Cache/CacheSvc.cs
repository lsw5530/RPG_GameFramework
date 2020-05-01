/****************************************************
	文件：CacheSvc.cs


	#if 	功能：缓存层
*****************************************************/

using System;
using System.Collections.Generic;
using GameMessage;
//using KDProtocol;

public class CacheSvc {
    private static CacheSvc instance = null;
    public static int m_PingInterval = 30;
    public static CacheSvc Instance {
        get {
            if (instance == null) {
                instance = new CacheSvc();
            }
            return instance;
        }
    }
    private DBMgr dbMgr;

    private Dictionary<string, ClientSocket> onLineAcctDic = new Dictionary<string, ClientSocket>();
    private Dictionary<ClientSocket, PlayerData> onLineSessionDic = new Dictionary<ClientSocket, PlayerData>();

    private static List<ClientSocket> timeOutSessions = new List<ClientSocket>();
    public void Init() {
        dbMgr = DBMgr.Instance;
        KDCommon.Log("CacheSvc Init Done.");
        TimerSvc.Instance.AddTimeTask(CheckOnline, 0, KDTimeUnit.Second, 0);
    }
    private void CheckOnline(int n) {
        long timeNow =NetSvc.GetTimeStamp();
        foreach (var item in onLineSessionDic) {
            if(timeNow- item.Key.LastPingTime> m_PingInterval*1.5f) {
                timeOutSessions.Add(item.Key);
            }
        }
        foreach (var item in timeOutSessions) {
            NetSvc.Instance.CloseClient(item);
            Console.WriteLine("当前在线人数：" + onLineSessionDic.Count);
        }
        timeOutSessions.Clear();
    }
    public bool IsAcctOnLine(string acct) {
        return onLineAcctDic.ContainsKey(acct);
    }

    /// <summary>
    /// 根据账号密码返回对应账号数据，密码错误返回null，账号不存在则默认创建新账号
    /// </summary>
    public PlayerData GetPlayerData(string acct, string pass) {
        return dbMgr.QueryPlayerData(acct, pass);
    }

    /// <summary>
    /// 账号上线，缓存数据
    /// </summary>
    public void AcctOnline(string acct, ClientSocket session, PlayerData playerData) {
        onLineAcctDic.Add(acct, session);
        onLineSessionDic.Add(session, playerData);
    }

    public bool IsNameExist(string name) {
        return dbMgr.QueryNameData(name);
    }

    public List<ClientSocket> GetOnlineServerSessions() {
        List<ClientSocket> lst = new List<ClientSocket>();
        foreach (var item in onLineSessionDic) {
            lst.Add(item.Key);
        }
        return lst;
    }

    public PlayerData GetPlayerDataBySession(ClientSocket session) {
        if (onLineSessionDic.TryGetValue(session, out PlayerData playerData)) {
            return playerData;
        }
        else {
            return null;
        }
    }

    public Dictionary<ClientSocket, PlayerData> GetOnlineCache() {
        return onLineSessionDic;
    }

    public ClientSocket GetOnlineServersession(int ID) {
        ClientSocket session = null;
        foreach (var item in onLineSessionDic) {
            if (item.Value.Id == ID) {
                session = item.Key;
                break;
            }
        }
        return session;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData) {
        return dbMgr.UpdatePlayerData(id, playerData);
    }

    public void AcctOffLine(ClientSocket session) {
        foreach (var item in onLineAcctDic) {
            if (item.Value == session) {
                onLineAcctDic.Remove(item.Key);
                break;
            }
        }

        bool succ = onLineSessionDic.Remove(session);
        KDCommon.Log("Offline Result: SessionID:" + session.sessionID + "  " + succ);
    }
}
