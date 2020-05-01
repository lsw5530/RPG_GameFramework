/****************************************************
    文件：BuyWnd.cs
	
    
    
	功能：购买交易窗口
*****************************************************/

using GameMessage;
using UnityEngine;
using UnityEngine.UI;

public class BuyWnd : WindowRoot {
    public Text txtInfo;
    public Button btnSure;

    private int buyType;//0：体力 1：金币
     
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        BuyFormData data = userData as BuyFormData;
        buyType = data.BuyType;
        btnSure.interactable = true;
        RefreshUI();
    }
    private void RefreshUI() {
        switch (buyType) {
            case 0:
                //体力
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("100体力", TxtColor.Green) + "?";
                break;
            case 1:
                //金币
                txtInfo.text = "是否花费" + Constants.Color("10钻石", TxtColor.Red) + "购买" + Constants.Color("1000金币", TxtColor.Green) + "?";
                break;
        }
    }

    public void ClickSureBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        //发送网络购买消息 
        CSPacketBase msg = new ReqBuy {
            Type = buyType,
            Cost = 10
        };

        GameEntry.Net.SendMsg(msg);
        btnSure.interactable = false;
    }

    public void ClickCloseBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        Close(true);
    }
}