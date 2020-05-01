/****************************************************
    文件：GameRoot.cs
	
    
    
	功能：游戏启动入口
*****************************************************/
using GameMain;
using UnityEngine;

public partial class GameEntry : MonoBehaviour { 
    public static NetComponent Net { get; private set; }
    public static ResComponent Res { get; private set; }
    public static TimerComponent Timer { get; private set; }
    public static LuaComponent Lua { get; private set; }
    private static void InitCustomComponents() {
        BuiltinData = UnityGameFramework.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
        Net = UnityGameFramework.Runtime.GameEntry.GetComponent<NetComponent>();
        Res = UnityGameFramework.Runtime.GameEntry.GetComponent<ResComponent>();
        Timer = UnityGameFramework.Runtime.GameEntry.GetComponent<TimerComponent>();
        Lua = UnityGameFramework.Runtime.GameEntry.GetComponent<LuaComponent>();
        Net.InitSvc();
        Timer.InitSvc();
    }
   
    static void CloseCustomComponents() {
        Net.Close();
        Timer.Close();
    }
}