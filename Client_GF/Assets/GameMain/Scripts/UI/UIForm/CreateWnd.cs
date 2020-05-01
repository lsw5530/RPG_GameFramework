/****************************************************
    文件：CreateWnd.cs
	
    
    
	功能：角色创建界面
*****************************************************/

using GameMessage;
using UnityEngine;
using UnityEngine.UI;

public class CreateWnd : WindowRoot {
    public InputField iptName;

    public void ClickRandBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        iptName.text = GameEntry.Res.GetRDNameData(false);
        string rdName = GameEntry.Res.GetRDNameData(false);
        iptName.text = rdName;
    }

    public void ClickEnterBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        if (iptName.text != "") {
            //发送名字数据到服务器，登录主城
            CSPacketBase msg = new ReqRename {
                Name = iptName.text
            };
            GameEntry.Net.SendMsg(msg);
        }
        else {
            GameEntry.UI.AddTips("当前名字不符合规范");
        }
    }
}