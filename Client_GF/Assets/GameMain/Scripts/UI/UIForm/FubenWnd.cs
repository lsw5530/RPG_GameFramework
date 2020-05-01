/****************************************************
    文件：FubenWnd.cs
	
    
    
	功能：副本选择界面
*****************************************************/

using GameMessage;
using UnityEngine;
using UnityEngine.UI;

public class FubenWnd : WindowRoot {
    public Button[] fbBtnArr;

    public Transform pointerTrans;

    private FubenFormData m_FubenFormData;
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        if (userData != null)
            m_FubenFormData = userData as FubenFormData;
        RefreshUI();
    }
    public void RefreshUI() {
        int fbid = m_FubenFormData.PlayerData.Fuben;
        for (int i = 0; i < fbBtnArr.Length; i++) {
            if (i < fbid % 10000) {
                SetActive(fbBtnArr[i].gameObject);
                if (i == fbid % 10000 - 1) {
                    pointerTrans.SetParent(fbBtnArr[i].transform);
                    pointerTrans.localPosition = new Vector3(25, 100, 0);
                }
            }
            else {
                SetActive(fbBtnArr[i].gameObject, false);
            }
        }
    }

    public void ClickTaskBtn(int fbid) {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        m_FubenFormData.OnClickTask(fbid);
    }

    public void ClickCloseBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        m_FubenFormData.OnClickClose();
    }
}