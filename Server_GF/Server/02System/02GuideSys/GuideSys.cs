/****************************************************
	文件：GuideSys.cs


	
	功能：引导业务系统
*****************************************************/

using GameMessage;
//using KDProtocol;

public class GuideSys {
    private static GuideSys instance = null;
    public static GuideSys Instance {
        get {
            if (instance == null) {
                instance = new GuideSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        KDCommon.Log("GuideSys Init Done.");
    }

    public void ReqGuide(MsgPack pack) {
        ReqGuide data =(ReqGuide) pack.msg;

        SCPacketBase msg = new RspGuide(); 
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        GuideCfg gc = cfgSvc.GetGuideCfg(data.Guideid);

        //更新引导ID
        if (pd.Guideid == data.Guideid) {

            //检测是否为智者点拔任务
            if (pd.Guideid == 1001) {
                TaskSys.Instance.CalcTaskPrgs(pd, 1);
            }
            pd.Guideid += 1;

            //更新玩家数据
            pd.Coin += gc.coin;
            KDCommon.CalcExp(pd, gc.exp);

            if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg = new RspGuide {
                    Guideid = pd.Guideid,
                    Coin = pd.Coin,
                    Lv = pd.Lv,
                    Exp = pd.Exp
                };
            }
        }
        else {
            msg.error = (int)ErrorCode.ServerDataError;
        }
        pack.session.SendMsg(msg);
    }


}
