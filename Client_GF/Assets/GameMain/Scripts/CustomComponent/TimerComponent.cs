/****************************************************
	文件：TimerSvc.cs
	
	
	
	功能：计时服务
*****************************************************/

using System;
using UnityEngine;
using UnityGameFramework.Runtime;

[DisallowMultipleComponent]
[AddComponentMenu("Game Framework/Timer")]
public class TimerComponent : GameFrameworkComponent {

    private PETimer pt;

    public void InitSvc() {
        pt = new PETimer();

        //设置日志输出
        pt.SetLog((string info) => {
            PECommon.Log(info);
        });

        PECommon.Log("Init TimerSvc...");
    }

    public void Update() {
        if (pt == null) return;
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1) {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public double GetNowTime() {
        return pt.GetMillisecondsTime();
    }

    public void DelTask(int tid) {
        pt.DeleteTimeTask(tid);
    }
    public long GetTimeStamp() {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }
    public void Close() {
        pt.Reset();
    }
}