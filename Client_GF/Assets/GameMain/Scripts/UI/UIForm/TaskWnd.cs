/****************************************************
    文件：TaskWnd.cs
	
    
	功能：任务奖励界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameMessage;
using GameFramework.Event;
using GameFramework.Resource;

public class TaskWnd : WindowRoot {
    public Transform scrollTrans;

    private List<TaskRewardData> trdLst = new List<TaskRewardData>();
    private TaskFormData m_TaskFormData;
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(OnRefreshUIFormEventArgs.EventId, RefreshUI);
        m_TaskFormData = userData as TaskFormData;
        RefreshUI(this, null);
    }
    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);
        GameEntry.Event.Unsubscribe(OnRefreshUIFormEventArgs.EventId, RefreshUI);
    }
    public void RefreshUI(object sender, GameEventArgs e) {
        OnRefreshUIFormEventArgs ne = e as OnRefreshUIFormEventArgs;
        if ((System.Object)sender != this && ne.formId != UIFormId.TaskForm) return;
        trdLst.Clear();

        List<TaskRewardData> todoLst = new List<TaskRewardData>();
        List<TaskRewardData> doneLst = new List<TaskRewardData>();

        //1|0|0
        for (int i = 0; i < m_TaskFormData.PlayerData.TaskArr.Count; i++) {
            string[] taskInfo = m_TaskFormData.PlayerData.TaskArr[i].Split('|');
            TaskRewardData trd = new TaskRewardData {
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1")
            };

            if (trd.taked) {
                doneLst.Add(trd);
            }
            else {
                todoLst.Add(trd);
            }
        }

        trdLst.AddRange(todoLst);
        trdLst.AddRange(doneLst);

        for (int i = 0; i < scrollTrans.childCount; i++) {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < trdLst.Count; i++) {
            GameEntry.Res.LoadPrefab(PathDefine.TaskItemPrefab, LoadAssetSucessCallBack, LoadAssetFailCallBack, i);
        }
    }
    private void LoadAssetSucessCallBack(string assetName, object asset, float duration, object userData) {
        GameObject go = asset as GameObject;
        int index = -1;
        if (!go || !int.TryParse(userData.ToString(), out index)) {
            Debug.LogError("Sprite:" + assetName + "资源不存在");
        }
        else {
            go = Instantiate(go);
            go.transform.SetParent(scrollTrans);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.name = "taskItem_" + index;

            TaskRewardData trd = trdLst[index];
            TaskRewardCfg trf = GameEntry.Res.GetTaskRewardCfg(trd.ID);

            SetText(GetTrans(go.transform, "txtName"), trf.taskName);
            SetText(GetTrans(go.transform, "txtPrg"), trd.prgs + "/" + trf.count);
            SetText(GetTrans(go.transform, "txtExp"), "奖励：    经验" + trf.exp);
            SetText(GetTrans(go.transform, "txtCoin"), "金币" + trf.coin);
            Image imgPrg = GetTrans(go.transform, "prgBar/prgVal").GetComponent<Image>();
            float prgVal = trd.prgs * 1.0f / trf.count;
            imgPrg.fillAmount = prgVal;

            Button btnTake = GetTrans(go.transform, "btnTake").GetComponent<Button>();
            btnTake.onClick.AddListener(() => {
                ClickTakeBtn(go.name);
            });

            Transform transComp = GetTrans(go.transform, "imgComp");
            if (trd.taked) {
                btnTake.interactable = false;
                SetActive(transComp);
            }
            else {
                SetActive(transComp, false);
                if (trd.prgs == trf.count) {
                    btnTake.interactable = true;
                }
                else {
                    btnTake.interactable = false;
                }
            }
            GameEntry.Resource.UnloadAsset(asset);
        }
    }

    private void LoadAssetFailCallBack(string assetName, LoadResourceStatus status, string errorMessage, object userData) {
        Debug.LogError("Prefab:" + assetName + " 资源不存在");
    }
    private void ClickTakeBtn(string name) {
        string[] nameArr = name.Split('_');
        int index = int.Parse(nameArr[1]);
        int taskId = trdLst[index].ID;
        m_TaskFormData.OnClickTakeTask(taskId);
        TaskRewardCfg trc = GameEntry.Res.GetTaskRewardCfg(trdLst[index].ID);
        int coin = trc.coin;
        int exp = trc.exp;
        GameEntry.UI.AddTips(Constants.Color("获得奖励：", TxtColor.Blue) + Constants.Color(" 金币 +" + coin + " 经验 +" + exp, TxtColor.Green));
    }

    public void ClickCloseBtn() {
        Close(true);
    }
}