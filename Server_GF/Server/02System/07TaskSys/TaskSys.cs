/****************************************************
	文件：TaskSys.cs


	
	功能：任务奖励系统
*****************************************************/

using GameMessage;
using Google.Protobuf.Collections;
//using KDProtocol;

public class TaskSys {
    private static TaskSys  instance = null;
    public static TaskSys Instance {
        get {
            if (instance == null) {
                instance = new TaskSys();
            }
            return instance;
        }
    }
    private CacheSvc cacheSvc = null;
    private CfgSvc cfgSvc = null;

    public void Init() {
        cacheSvc = CacheSvc.Instance;
        cfgSvc = CfgSvc.Instance;
        KDCommon.Log("TaskSys Init Done.");
    }

    public void ReqTakeTaskReward(MsgPack pack) {
        ReqTakeTaskReward data =(ReqTakeTaskReward) pack.msg;

        SCPacketBase msg = new RspTakeTaskReward();
        PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.Rid);
        TaskRewardData trd = CalcTaskRewardData(pd, data.Rid);

        if (trd.prgs == trc.count && !trd.taked) {
            pd.Coin += trc.coin;
            KDCommon.CalcExp(pd, trc.exp);
            trd.taked = true;
            //更新任务进度数据
            CalcTaskArr(pd, trd);

            if (!cacheSvc.UpdatePlayerData(pd.Id, pd)) {
                msg.error = (int)ErrorCode.UpdateDBError;
            }
            else {
                RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward {
                    Coin = pd.Coin,
                    Lv = pd.Lv,
                    Exp = pd.Exp,
                };
                rspTakeTaskReward.TaskArr.SetRepeated<string>(pd.TaskArr);
                msg = rspTakeTaskReward;
            }
        }
        else {
            msg.error = (int)ErrorCode.ClientDataError;
        }
        pack.session.SendMsg(msg);
    }

    public TaskRewardData CalcTaskRewardData(PlayerData pd, int rid) {
            Google.Protobuf.Collections.RepeatedField<string> aabb;
        //Google.Protobuf.Collections.RepeatedField<string> aa = pd.TaskArr;

        TaskRewardData trd = null;
        for (int i = 0; i < pd.TaskArr.Count; i++) {
            var a = pd.TaskArr;
            string[] taskinfo = pd.TaskArr[i].Split('|');
            //1|0|0
            if (int.Parse(taskinfo[0]) == rid) {
                trd = new TaskRewardData {
                    ID = int.Parse(taskinfo[0]),
                    prgs = int.Parse(taskinfo[1]),
                    taked = taskinfo[2].Equals("1")
                };
                break;
            }
        }
        return trd;
    }

    public void CalcTaskArr(PlayerData pd, TaskRewardData trd) {
        string result = trd.ID + "|" + trd.prgs + '|' + (trd.taked ? 1 : 0);
        int index = -1;
        for (int i = 0; i < pd.TaskArr.Count; i++) {
            string[] taskinfo = pd.TaskArr[i].Split('|');
            if (int.Parse(taskinfo[0]) == trd.ID) {
                index = i;
                break;
            }
        }
        pd.TaskArr[index] = result;
    }

    public void CalcTaskPrgs(PlayerData pd, int tid) {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count) {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);

            ClientSocket session = cacheSvc.GetOnlineServersession(pd.Id);
            if (session != null) {
                PshTaskPrgs PshTaskPrgs = new PshTaskPrgs();
                PshTaskPrgs.TaskArr.SetRepeated<string>(pd.TaskArr);
                session.SendMsg(PshTaskPrgs);
            }
        }
    }

    public PshTaskPrgs GetTaskPrgs(PlayerData pd, int tid) {
        TaskRewardData trd = CalcTaskRewardData(pd, tid);
        TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

        if (trd.prgs < trc.count) {
            trd.prgs += 1;
            //更新任务进度
            CalcTaskArr(pd, trd);
            PshTaskPrgs pshTaskPrgs = new PshTaskPrgs();
            pshTaskPrgs.TaskArr.SetRepeated<string>(pd.TaskArr);
            return pshTaskPrgs;
        }
        else {
            return null;
        }
    }
}