/****************************************************
	文件：FubenSys.cs


	
	功能：副本战斗业务
*****************************************************/

using GameMessage;
//using KDProtocol;

public class FubenSys {
    private static FubenSys instance = null;
    public static FubenSys Instance {
        get {
            if (instance == null) {
                instance = new FubenSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        KDCommon.Log("FubenSys Init Done.");
    }

    public void ReqFBFight(MsgPack pack) {
        ReqFBFight data =(ReqFBFight) pack.msg;

        SCPacketBase msg = new RspFBFight(); 
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int power = cfgSvc.GetMapCfg(data.Fbid).power;

        if (pd.Fuben < data.Fbid) {
            msg.error = (int)ErrorCode.ClientDataError;
        }
        else if (pd.Power < power) {
            msg.error = (int)ErrorCode.LackPower;
        }
        else {
            pd.Power -= power;
            if (cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                RspFBFight rspFBFight = new RspFBFight {
                    Fbid = data.Fbid,
                    Power = pd.Power
                };
                msg = rspFBFight;
            }
            else {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
        }
        pack.session.SendMsg(msg);
    }

    public void ReqFBFightEnd(MsgPack pack) {
        ReqFBFightEnd data =(ReqFBFightEnd) pack.msg;

        SCPacketBase msg = new RspFBFightEnd(); 
        //校验战斗是否合法
        if (data.Win) {
            if (data.Costtime > 0 && data.Resthp > 0) {
                //根据副本ID获取相应奖励
                MapCfg rd = cfgSvc.GetMapCfg(data.Fbid);
                PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

                //任务进度数据更新
                TaskSys.Instance.CalcTaskPrgs(pd, 2);

                pd.Coin += rd.coin;
                pd.Crystal += rd.crystal;
                KDCommon.CalcExp(pd, rd.exp);

                if (pd.Fuben == data.Fbid) {
                    pd.Fuben += 1;
                }

                if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                    msg.error = (int)ErrorCode.UpdateDBError;
                }
                else {
                    RspFBFightEnd rspFBFight = new RspFBFightEnd {
                        Win = data.Win,
                        Fbid = data.Fbid,
                        Resthp = data.Resthp,
                        Costtime = data.Costtime,

                        Coin = pd.Coin,
                        Lv = pd.Lv,
                        Exp = pd.Exp,
                        Crystal = pd.Crystal,
                        Fuben = pd.Fuben
                    };

                    msg = rspFBFight;
                }
            }

        }
        else {
            msg.error = (int)ErrorCode.ClientDataError;
        }

        pack.session.SendMsg(msg);
    }
}
