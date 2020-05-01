/****************************************************
	文件：StrongSys.cs


	
	功能：强化升级系统
*****************************************************/

using GameMessage;
//using KDProtocol;

public class StrongSys {
    private static StrongSys instance = null;
    public static StrongSys Instance {
        get {
            if (instance == null) {
                instance = new StrongSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        KDCommon.Log("StrongSys Init Done.");
    }

    public void ReqStrong(MsgPack pack) {
        ReqStrong data =(ReqStrong) pack.msg;

        RspStrong msg = new RspStrong();
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        int curtStartLv = pd.StrongArr[data.Pos];
        StrongCfg nextSd = CfgSvc.Instance.GetStrongCfg(data.Pos, curtStartLv + 1);

        //条件判断
        if (pd.Lv < nextSd.minlv) {
            msg.error = (int)ErrorCode.LackLevel;
        }
        else if (pd.Coin < nextSd.coin) {
            msg.error = (int)ErrorCode.LackCoin;
        }
        else if (pd.Crystal < nextSd.crystal) {
            msg.error = (int)ErrorCode.LackCrystal;
        }
        else {
            //任务进度数据更新
            TaskSys.Instance.CalcTaskPrgs(pd, 3);

            //资源扣除
            pd.Coin -= nextSd.coin;
            pd.Crystal -= nextSd.crystal;

            pd.StrongArr[data.Pos] += 1;

            //增加属性
            pd.Hp += nextSd.addhp;
            pd.Ad += nextSd.addhurt;
            pd.Ap += nextSd.addhurt;
            pd.Addef += nextSd.adddef;
            pd.Apdef += nextSd.adddef;
        }

        //更新数据库
        if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
            msg.error = (int)ErrorCode.UpdateDBError;
        }
        else {
            msg = new RspStrong {
                Coin = pd.Coin,
                Crystal = pd.Crystal,
                Hp = pd.Hp,
                Ad = pd.Ad,
                Ap = pd.Ap,
                Addef = pd.Addef,
                Apdef = pd.Apdef,
            };
            msg.StrongArr.SetRepeated<int>(pd.StrongArr);
        }
        pack.session.SendMsg(msg);
    }
}
