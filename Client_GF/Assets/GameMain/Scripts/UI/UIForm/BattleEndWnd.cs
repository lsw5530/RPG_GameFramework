/****************************************************
    文件：BattleEndWnd.cs
	
    
    	功能：战斗结算界面
*****************************************************/

using GameFramework.Event;
using UnityEngine;
using UnityEngine.UI;


public class BattleEndWnd : WindowRoot {
    #region UI Define
    public Transform rewardTrans;
    public Button btnClose;
    public Button btnExit;
    public Button btnSure;
    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;
    public Animation ani;
    #endregion

    private FBEndType endType = FBEndType.None;
    private BattleEndFormData m_BattleEndFormData;
    protected override void OnInit(object userData) {
        base.OnInit(userData);
        FadeTime = 0;
    }
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        if (userData != null)
            m_BattleEndFormData = userData as BattleEndFormData;
        endType = m_BattleEndFormData.EndType;
        RefreshUI(this, null);
    }
    private void RefreshUI(object sender, GameEventArgs e) {
        OnRefreshUIFormEventArgs ne = e as OnRefreshUIFormEventArgs;
        if ((System.Object)sender != this && ne.formId != UIFormId.BattleEndForm) return;
        switch (endType) {
            case FBEndType.Pause:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case FBEndType.Win:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);

                MapCfg cfg = GameEntry.Res.GetMapCfg(m_BattleEndFormData.Fbid);
                int min = m_BattleEndFormData.CostTime / 60;
                int sec = m_BattleEndFormData.CostTime % 60;
                int coin = cfg.coin;
                int exp = cfg.exp;
                int crystal = cfg.crystal;
                SetText(txtTime, "通关时间：" + min + ":" + sec);
                SetText(txtRestHP, "剩余血量：" + m_BattleEndFormData.RestHp);
                SetText(txtReward, "关卡奖励：" + Constants.Color(coin + "金币", TxtColor.Green) + Constants.Color(exp + "经验", TxtColor.Yellow) + Constants.Color(crystal + "水晶", TxtColor.Blue));

                GameEntry.Timer.AddTimeTask((int tid) => {
                    SetActive(rewardTrans);
                    ani.Play();
                    GameEntry.Timer.AddTimeTask((int tid1) => {
                        GameEntry.Sound.PlayUISound(Constants.FBItemEnter);
                        GameEntry.Timer.AddTimeTask((int tid2) => {
                            GameEntry.Sound.PlayUISound(Constants.FBItemEnter);
                            GameEntry.Timer.AddTimeTask((int tid3) => {
                                GameEntry.Sound.PlayUISound(Constants.FBItemEnter);
                                GameEntry.Timer.AddTimeTask((int tid5) => {
                                    GameEntry.Sound.PlayUISound(Constants.FBItemEnter);
                                }, 300);
                            }, 270);
                        }, 270);
                    }, 325);
                }, 1000);
                break;
            case FBEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject, false);
                GameEntry.Sound.PlayUISound(Constants.FBLose);
                break;
        }
    }

    public void ClickClose() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        m_BattleEndFormData.OnClickClose();
        Close(true);
    }

    public void ClickExitBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        //进入主城，销毁当前战斗 
        m_BattleEndFormData.OnClickExit();
    }

    public void ClickSureBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        m_BattleEndFormData.OnClickSure();
    }

    //private int fbid;
    //private int costtime;
    //private int resthp;
    //public void SetBattleEndData(int fbid, int costtime, int resthp) {
    //    this.fbid = fbid;
    //    this.costtime = costtime;
    //    this.resthp = resthp;
    //}
}

public enum FBEndType {
    None,
    Pause,
    Win,
    Lose
}
