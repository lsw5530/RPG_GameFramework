/****************************************************
    文件：LoginWnd.cs
	
    
    
	功能：登录注册界面
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

public class LoginWnd : WindowRoot {
    public InputField iptAcct;
    public InputField iptPass;
    public Button btnEnter;
    public Button btnNotice;
    private LoginFormData m_LoginFormData;

    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        m_LoginFormData = userData as LoginFormData;
        if (userData != null) {

        }
            //获取本地存储的账号密码
            if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass")) {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else {
            iptAcct.text = "";
            iptPass.text = "";
        }
    } 

    /// <summary>
    /// 点击进入游戏
    /// </summary>
    public void ClickEnterBtn() {
        //GameEntry.audio.PlayUIAudio(Constants.UILoginBtn);
        PlayUISound(Constants.UILoginBtn);

        string _acct = iptAcct.text;
        string _pass = iptPass.text;
        if (_acct != "" && _pass != "") {
            //更新本地存储的账号密码
            PlayerPrefs.SetString("Acct", _acct);
            PlayerPrefs.SetString("Pass", _pass);
            m_LoginFormData.OnClickEnter(_acct,_pass); 
        }
        else {
            GameEntry.UI.AddTips("账号或密码为空");
        }
    }
    public void ClicKNoticeBtn() {
        PlayUISound(Constants.UIClickBtn);
        m_LoginFormData.OnClickNotice();
    } 
}