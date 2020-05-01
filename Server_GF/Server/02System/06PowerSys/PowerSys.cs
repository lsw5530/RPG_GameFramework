/****************************************************
	


	
	功能：体力恢复系统
*****************************************************/

using System.Collections.Generic;
using GameMessage;
//using KDProtocol;

public class PowerSys {
    private static PowerSys instance = null;
    public static PowerSys Instance {
        get {
            if (instance == null) {
                instance = new PowerSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private TimerSvc timerSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        timerSvc = TimerSvc.Instance;

        TimerSvc.Instance.AddTimeTask(CalcPowerAdd, KDCommon.PowerAddSpace, KDTimeUnit.Minute, 0);
        KDCommon.Log("PowerSys Init Done.");
    }

    private void CalcPowerAdd(int tid) {
        //计算体力增长
        PshPower msg = new PshPower(); 
        //所有在线玩家获得实时的体力增长推送数据
        Dictionary<ClientSocket, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
        if(onlineDic.Count>0)
            KDCommon.Log("All Online Player Calc Power Incress....");

        foreach (var item in onlineDic) {
            PlayerData pd = item.Value;
            ClientSocket session = item.Key;

            int powerMax = KDCommon.GetPowerLimit(pd.Lv);
            if (pd.Power >= powerMax) {
                continue;
            }
            else {
                pd.Power += KDCommon.PowerAddCount;
                pd.Time = timerSvc.GetNowTime();
                if (pd.Power > powerMax) {
                    pd.Power = powerMax;
                }
            }
            if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
            else {
                msg.Power = pd.Power;
                session.SendMsg(msg);
            }
        }
    }
}
