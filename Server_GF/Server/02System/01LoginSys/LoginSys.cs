/****************************************************
	文件：LoginSys.cs


		功能：登录业务系统
*****************************************************/

using System;
using GameMessage;
//using KDProtocol;

public class LoginSys {
    private static LoginSys instance = null;
    public static LoginSys Instance {
        get {
            if (instance == null) {
                instance = new LoginSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;
        KDCommon.Log("LoginSys Init Done.");
    }

    public void ReqLogin(MsgPack pack) {
        ReqLogin data =(ReqLogin) pack.msg;
        //当前账号是否已经上线
        SCPacketBase msg = new RspLogin();  
        if (cacheSvc.IsAcctOnLine(data.Acct)) {
            //己上线：返回错误信息
            msg.error = (int)ErrorCode.AcctIsOnline;
        }
        else {
            //未上线：
            //账号是否存在 
            PlayerData pd = cacheSvc.GetPlayerData(data.Acct, data.Pass);
            if (pd == null) {
                //存在，密码错误
                msg.error = (int)ErrorCode.WrongPass;
            }
            else {
                //计算离线体力增长
                int power = pd.Power;
                long now = timerSvc.GetNowTime();
                long milliseconds = now - pd.Time;
                int addPower = (int)(milliseconds / (1000 * 60 * KDCommon.PowerAddSpace)) * KDCommon.PowerAddCount;
                if (addPower > 0) {
                    int powerMax = KDCommon.GetPowerLimit(pd.Lv);
                    if (pd.Power < powerMax) {
                        pd.Power += addPower;
                        if (pd.Power > powerMax) {
                            pd.Power = powerMax;
                        }
                    }
                }

                if (power != pd.Power) {
                    cacheSvc.UpdatePlayerData(pd.Id, pd);
                }

                msg = new RspLogin {
                    PlayerData = pd
                };
                //缓存账号数据
                cacheSvc.AcctOnline(data.Acct, pack.session, pd);
            }
        }
        //回应客户端
        pack.session.SendMsg(msg);
    }

    public void ReqRename(MsgPack pack) {
        ReqRename data = (ReqRename)pack.msg;
        SCPacketBase msg = new RspRename();
        if (cacheSvc.IsNameExist(data.Name)) {
            //名字是否已经存在
            //存在：返回错误码
            msg.error = (int)ErrorCode.NameIsExist;
        }
        else {
            //不存在：更新缓存，以及数据库，再返回给客户端
            PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
            playerData.Name = data.Name;
            if (!cacheSvc.UpdatePlayerData(playerData.Id, playerData)) {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg = new RspRename {
                    Name = data.Name
                };
            }
        }
        pack.session.SendMsg(msg);
    }
   
    public void ClearOfflineData(ClientSocket session) {
        //写入下线时间
        PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
        if (pd != null) {
            pd.Time = timerSvc.GetNowTime();
            if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                KDCommon.Log("Update offline time error", LogType.Error);
            }
            cacheSvc.AcctOffLine(session);
        }
    }
}
