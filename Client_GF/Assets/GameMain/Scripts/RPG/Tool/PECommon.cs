/****************************************************
		功能：客户端服务端共用工具类
*****************************************************/

using GameFramework.DataTable;
using GameMessage;
using StarForce;
using UnityEngine; 
public enum LogType {
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}

public class PECommon {
    public const int PowerAddSpace = 5;//分钟
    public const int PowerAddCount = 2;
    public static PlayerData PlayerData { get; private set; } = null;
    public static void SetPlayerData(RspLogin data) {
        PlayerData = data.PlayerData;
    }
    public static void SetPlayerName(string name) {
        PlayerData.Name = name;
    }
    public static void SetPlayerDataByGuide(RspGuide data) {
        PlayerData.Coin = data.Coin;
        PlayerData.Lv = data.Lv;
        PlayerData.Exp = data.Exp;
        PlayerData.Guideid = data.Guideid;
    }

    public static void SetPlayerDataByStrong(RspStrong data) {
        PlayerData.Coin = data.Coin;
        PlayerData.Crystal = data.Crystal;
        PlayerData.Hp = data.Hp;
        PlayerData.Ad = data.Ad;
        PlayerData.Ap = data.Ap;
        PlayerData.Addef = data.Addef;
        PlayerData.Apdef = data.Apdef;

        PlayerData.StrongArr.SetRepeated<int>(data.StrongArr);
    }
    public static void SetPlayerDataByBuy(RspBuy data) {
        PlayerData.Diamond = data.Dimond;
        PlayerData.Coin = data.Coin;
        PlayerData.Power = data.Power;
    }
    public static void SetPlayerDataByPower(PshPower data) {
        PlayerData.Power = data.Power;
    }
    public static void SetPlayerDataByTask(RspTakeTaskReward data) {
        PlayerData.Coin = data.Coin;
        PlayerData.Lv = data.Lv;
        PlayerData.Exp = data.Exp;
        PlayerData.TaskArr.SetRepeated<string>(data.TaskArr);
    }
    public static void SetPlayerDataByTaskPsh(PshTaskPrgs data) {
        PlayerData.TaskArr.SetRepeated<string>(data.TaskArr);
    }
    public static void SetPlayerDataByFBStart(RspFBFight data) {
        PlayerData.Power = data.Power;
    }
    public static void SetPlayerDataByFBEnd(RspFBFightEnd data) {
        PlayerData.Coin = data.Coin;
        PlayerData.Lv = data.Lv;
        PlayerData.Exp = data.Exp;
        PlayerData.Crystal = data.Crystal;
        PlayerData.Fuben = data.Fuben;
    }
    public static void Log(string msg = "", LogType tp = LogType.Log) {
        LogType lv = (LogType)tp;
        switch (lv) {
            case LogType.Log:
                Debug.Log(msg);
                break;
            case LogType.Warn:
                Debug.LogWarning(msg);
                break;
            case LogType.Error:
                Debug.LogError(msg);
                break;
            case LogType.Info:
                Debug.Log(msg);
                break;
            default:
                break;
        }
    }
    public static int GetFightByProps(PlayerData pd) {
        return pd.Lv * 100 + pd.Ad + pd.Ap + pd.Addef + pd.Apdef;
    }

    public static int GetPowerLimit(int lv) {
        return ((lv - 1) / 10) * 150 + 150;
    }

    public static int GetExpUpValByLv(int lv) {
        return 100 * lv * lv;
    }

    public static void CalcExp(PlayerData pd, int addExp) {
        int curtLv = pd.Lv;
        int curtExp = pd.Exp;
        int addRestExp = addExp;
        while (true) {
            int upNeedExp = GetExpUpValByLv(curtLv) - curtExp;
            if (addRestExp >= upNeedExp) {
                curtLv += 1;
                curtExp = 0;
                addRestExp -= upNeedExp;
            }
            else {
                pd.Lv = curtLv;
                pd.Exp = curtExp + addRestExp;
                break;
            }
        }
    }
    public static bool InSecene(SceneId sceneId) {
        IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
        DRScene drScene = dtScene.GetDataRow((int)sceneId);
        string sceneAssetName = AssetUtility.GetSceneAsset(drScene.AssetName);
        return GameEntry.Scene.SceneIsLoaded(sceneAssetName);
    } 
}
