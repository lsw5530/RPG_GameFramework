/****************************************************
    文件：LoadingWnd.cs
	
    
    
	功能：加载进度界面
*****************************************************/

using System;
using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWnd : WindowRoot {
    public Text txtTips;
    public Image imgFG;
    public Image imgPoint;
    public Text txtPrg;
    private float fgWidth;
    protected override void OnInit(object userData) {
        base.OnInit(userData);
        FadeTime = 0;
    }
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(LoadingFormUpdateProgressEventArgs.EventId, OnProgressUpdate);
        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x;

        SetText(txtTips, "这是一条游戏Tips");
        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0;
        imgPoint.transform.localPosition = new Vector3(-545f, 0, 0);
    }
    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);
        GameEntry.Event.Unsubscribe(LoadingFormUpdateProgressEventArgs.EventId, OnProgressUpdate);
    }
    public void OnProgressUpdate(object sender, GameEventArgs e) {
        LoadingFormUpdateProgressEventArgs ne = e as LoadingFormUpdateProgressEventArgs;
        SetText(txtPrg, (int)(ne.Progress * 100) + "%");
        imgFG.fillAmount = ne.Progress;
        float posX = ne.Progress * fgWidth - 545;
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }
}