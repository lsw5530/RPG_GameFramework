/****************************************************
    文件：DynamicWnd.cs
	
    
    
	功能：动态UI元素界面
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWnd : WindowRoot {
    public Animation tipsAni;
    public Text txtTips;
    public Transform hpItemRoot;

    public Animation selfDodgeAni;

    private bool isTipsShow = false;
    private Queue<string> tipsQue = new Queue<string>();
    private Dictionary<string, ItemEntityHP> itemDic = new Dictionary<string, ItemEntityHP>();

    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(AddTipEventArgs.EventId, AddTips);
        GameEntry.Event.Subscribe(RmvAllHpItemInfoEventArgs.EventId, RmvAllHpItemInfo);
        GameEntry.Event.Subscribe(SetDodgeEventArgs.EventId, SetDodge);
        GameEntry.Event.Subscribe(SetCriticalEventArgs.EventId, SetCritical);
        GameEntry.Event.Subscribe(SetHurtEventArgs.EventId, SetHurt);
        GameEntry.Event.Subscribe(SetHPValEventArgs.EventId, SetHPVal);
        GameEntry.Event.Subscribe(SetSelfDodgeEventArgs.EventId, SetSelfDodge);
        GameEntry.Event.Subscribe(AddHpItemInfoEventArgs.EventId, AddHpItemInfo);
        GameEntry.Event.Subscribe(RmvHpItemInfoEventArgs.EventId, RmvHpItemInfo);
        SetActive(txtTips, false);
    }

    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);
        GameEntry.Event.Unsubscribe(AddTipEventArgs.EventId, AddTips);
        GameEntry.Event.Unsubscribe(RmvAllHpItemInfoEventArgs.EventId, RmvAllHpItemInfo);
        GameEntry.Event.Unsubscribe(SetDodgeEventArgs.EventId, SetDodge);
        GameEntry.Event.Unsubscribe(SetCriticalEventArgs.EventId, SetCritical);
        GameEntry.Event.Unsubscribe(SetHurtEventArgs.EventId, SetHurt);
        GameEntry.Event.Unsubscribe(SetHPValEventArgs.EventId, SetHPVal);
        GameEntry.Event.Unsubscribe(SetSelfDodgeEventArgs.EventId, SetSelfDodge);
        GameEntry.Event.Unsubscribe(AddHpItemInfoEventArgs.EventId, AddHpItemInfo);
        GameEntry.Event.Unsubscribe(RmvHpItemInfoEventArgs.EventId, RmvHpItemInfo);
    }
    #region Tips相关  
    private void AddTips(object sender, GameEventArgs e) {
        AddTipEventArgs ne = e as AddTipEventArgs;
        lock (tipsQue) {
            tipsQue.Enqueue(ne.Tip);
        }
    }
    private void Update() {
        if (tipsQue.Count > 0 && isTipsShow == false) {
            lock (tipsQue) {
                string tips = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tips);
            }
        }
    }
    private void SetTips(string tips) {
        SetActive(txtTips);
        SetText(txtTips, tips);

        AnimationClip clip = tipsAni.GetClip("TipsShowAni");
        tipsAni.Play();
        //延时关闭激活状态

        StartCoroutine(AniPlayDone(clip.length, () => {
            SetActive(txtTips, false);
            isTipsShow = false;
        }));
    }

    private IEnumerator AniPlayDone(float sec, Action cb) {
        yield return new WaitForSeconds(sec);
        if (cb != null) {
            cb();
        }
    }
    #endregion

    private void AddHpItemInfo(object sender, GameEventArgs e) {
        AddHpItemInfoEventArgs ne = e as AddHpItemInfoEventArgs;
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Name, out item)) {
            return;
        }
        else {
            AddHpItemInfoEventArgs ne1 = ReferencePool.Acquire<AddHpItemInfoEventArgs>();
            ne1.Fill(ne.Name, ne.Tf, ne.Hp);
            GameEntry.Res.LoadPrefab(PathDefine.HPItemPrefab, LoadAssetSucessCallBack, LoadAssetFailCallBack, ne1);
        }
    }

    private void LoadAssetSucessCallBack(string assetName, object asset, float duration, object userData) {
        GameObject go = asset as GameObject;
        AddHpItemInfoEventArgs ne = userData as AddHpItemInfoEventArgs;
        if (!go || ne == null) {
            Debug.LogError("Sprite:" + assetName + "资源不存在");
            return;
        }
       
        go = Instantiate(go);
        go.transform.SetParent(hpItemRoot);
        go.transform.localPosition = new Vector3(-1000, 0, 0);
        ItemEntityHP ieh = go.GetComponent<ItemEntityHP>();
        ieh.InitItemInfo(ne.Tf, ne.Hp);
        itemDic.Add(ne.Name, ieh);
        GameEntry.Resource.UnloadAsset(asset);
        ReferencePool.Release(ne); 
    }

    private void LoadAssetFailCallBack(string assetName, LoadResourceStatus status, string errorMessage, object userData) {
        Debug.LogError("Prefab:" + assetName + " 资源不存在");
    }

    private void RmvHpItemInfo(object sender, GameEventArgs e) {
        RmvHpItemInfoEventArgs ne = e as RmvHpItemInfoEventArgs;
        
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Key, out item)) {
            Destroy(item.gameObject);
            itemDic.Remove(ne.Key);
        }
    }
    private void RmvAllHpItemInfo(object sender, GameEventArgs e) {
        RmvAllHpItemInfoEventArgs ne = e as RmvAllHpItemInfoEventArgs;
        foreach (var item in itemDic) {
            Destroy(item.Value.gameObject);
        }
        itemDic.Clear();
    }

    private void SetDodge(object sender, GameEventArgs e) {
        SetDodgeEventArgs ne = e as SetDodgeEventArgs;
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Name, out item)) {
            item.SetDodge();
        }
    }

    private void SetCritical(object sender, GameEventArgs e) {
        SetCriticalEventArgs ne = e as SetCriticalEventArgs;
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Name, out item)) {
            item.SetCritical(ne.Critical);
        }
    }

    private void SetHurt(object sender, GameEventArgs e) {
        SetHurtEventArgs ne = e as SetHurtEventArgs;
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Name, out item)) {
            item.SetHurt(ne.Hurt);
        } 
    }

    private void SetHPVal(object sender, GameEventArgs e) {
        SetHPValEventArgs ne = e as SetHPValEventArgs;
        ItemEntityHP item = null;
        if (itemDic.TryGetValue(ne.Name, out item)) {
            item.SetHPVal(ne.OldVal, ne.NewVal);
        }
    }

    private void SetSelfDodge(object sender, GameEventArgs e) {
        selfDodgeAni.Stop();
        selfDodgeAni.Play();
    }
}