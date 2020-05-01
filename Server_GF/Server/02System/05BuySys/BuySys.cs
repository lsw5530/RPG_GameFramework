/****************************************************
	文件：BuySys.cs


	
	功能：交易购买系统
*****************************************************/

using GameMessage;
//using KDProtocol;

public class BuySys {
    private static BuySys instance = null;
    public static BuySys Instance {
        get {
            if (instance == null) {
                instance = new BuySys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        KDCommon.Log("BuySys Init Done.");
    }

    public void ReqBuy(MsgPack pack) {
        ReqBuy data = (ReqBuy)pack.msg;
        RspBuy msg = new RspBuy();
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        if (pd.Diamond < data.Cost) {
            msg.error = (int)ErrorCode.LackDiamond;
        }
        else {
            pd.Diamond -= data.Cost;
            PshTaskPrgs pshTaskPrgs = null;
            switch (data.Type) {
                case 0:
                    pd.Power += 100;
                    //任务进度数据更新
                    pshTaskPrgs = TaskSys.Instance.GetTaskPrgs(pd, 4);
                    break;
                case 1:
                    pd.Coin += 1000;
                    //任务进度数据更新
                    pshTaskPrgs = TaskSys.Instance.GetTaskPrgs(pd, 5);
                    break;
            }

            if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
            else {
                RspBuy rspBuy = new RspBuy {
                    Type = data.Type,
                    Dimond = pd.Diamond,
                    Coin = pd.Coin,
                    Power = pd.Power
                };
                msg = rspBuy;

                //并包处理
                //msg.pshTaskPrgs = pshTaskPrgs;
                if(pshTaskPrgs!=null)
                pack.session.SendMsg(pshTaskPrgs);
            }
        }
        pack.session.SendMsg(msg);
    }
}
