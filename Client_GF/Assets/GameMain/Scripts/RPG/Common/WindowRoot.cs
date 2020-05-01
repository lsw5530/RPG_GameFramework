/****************************************************
    文件：WindowRoot.cs
	
    
    
	功能：UI界面基类
*****************************************************/

using System;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowRoot : UGuiForm {
    public bool GetWndState() {
        return gameObject.activeSelf;
    }

    #region Tool Functions

    protected void SetActive(GameObject go, bool isActive = true) {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true) {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true) {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true) {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true) {
        txt.transform.gameObject.SetActive(state);
    }

    protected void SetText(Text txt, string context = "") {
        txt.text = context;
    }
    protected void SetText(Transform trans, int num = 0) {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans, string context = "") {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt, int num = 0) {
        SetText(txt, num.ToString());
    }

    protected void SetSprite(Image img, string path) {
        GameEntry.Res.LoadSprite(path, LoadSpriteSuccessCallback, LoadSpriteFailureCallback, img);
    }
    private void LoadSpriteSuccessCallback(string assetName, object asset, float duration, object userData) {
        Image img = userData as Image; 
        Sprite sp = asset as Sprite;
        if (!sp||!img) {
            Debug.LogError("Sprite:" + assetName + "资源不存在");
        }else {
            img.sprite = sp;
            GameEntry.Resource.UnloadAsset(asset);
        }
    }
    private static void LoadSpriteFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData) {
        Debug.LogError("Sprite:" + assetName + " 资源不存在");
    }
    protected T GetOrAddComponect<T>(GameObject go) where T : Component {
        T t = go.GetComponent<T>();
        if (t == null) {
            t = go.AddComponent<T>();
        }
        return t;
    }

    protected Transform GetTrans(Transform trans, string name) {
        if (trans != null) {
            return trans.Find(name);
        }
        else {
            return transform.Find(name);
        }
    }
    #endregion

    #region Click Evts
    protected void OnClick(GameObject go, Action<object> cb, object args) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }

    protected void OnClickDown(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickDown = cb;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickUp = cb;
    }

    protected void OnDrag(GameObject go, Action<PointerEventData> cb) {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onDrag = cb;
    }
    #endregion
}