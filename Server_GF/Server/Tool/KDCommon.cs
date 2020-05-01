/****************************************************
		功能：客户端服务端共用工具类
*****************************************************/

using GameMessage;
public enum LogType {
    Log = 0,
    Warn = 1,
    Error = 2,
    Info = 3
}

public class KDCommon {


    public static void Log(string msg = "", LogType tp = LogType.Log) {
        LogType lv = (LogType)tp;
        switch (lv) {
            case LogType.Log:
                Debug.Log(msg);
                break;
            case LogType.Warn:
                Debug.LogWarn(msg);
                break;
            case LogType.Error:
                Debug.LogError(msg);
                break;
            case LogType.Info:
                Debug.LogInfo(msg);
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

    public const int PowerAddSpace = 5;//分钟
    public const int PowerAddCount = 2;
}
