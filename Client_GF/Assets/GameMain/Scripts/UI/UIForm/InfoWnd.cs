/****************************************************
    文件：InfoWnd.cs
	
    
    
	功能：角色信息展示界面
*****************************************************/

using GameMessage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoWnd : WindowRoot {
    #region UI Define
    public RawImage imgChar;

    public Text txtInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHP;
    public Text txtHurt;
    public Text txtDef;

    public Button btnClose;

    public Button btnDetail;
    public Button btnCloseDetail;
    public Transform transDetail;

    public Text dtxhp;
    public Text dtxad;
    public Text dtxap;
    public Text dtxaddef;
    public Text dtxapdef;
    public Text dtxdodge;
    public Text dtxpierce;
    public Text dtxcritical;
    #endregion

    private Vector2 startPos;
    private InfoFormData m_InfoFormData;

    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        if (userData != null) m_InfoFormData = userData as InfoFormData;
        RegTouchEvts();
        SetActive(transDetail, false);
        RefreshUI();
    }
    private void RegTouchEvts() {
        OnClickDown(imgChar.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            m_InfoFormData.OnClickDown();
        });
        OnDrag(imgChar.gameObject, (PointerEventData evt) => {
            float roate = -(evt.position.x - startPos.x) * 0.4f;
            m_InfoFormData.OnDrag(roate);
        });
    }

    private void RefreshUI() {
        PlayerData pd = PECommon.PlayerData;
        SetText(txtInfo, pd.Name + " LV." + pd.Lv);
        SetText(txtExp, pd.Exp + "/" + PECommon.GetExpUpValByLv(pd.Lv));
        imgExpPrg.fillAmount = pd.Exp * 1.0F / PECommon.GetExpUpValByLv(pd.Lv);
        SetText(txtPower, pd.Power + "/" + PECommon.GetPowerLimit(pd.Lv));
        imgPowerPrg.fillAmount = pd.Power * 1.0F / PECommon.GetPowerLimit(pd.Lv);

        SetText(txtJob, " 职业   暗夜刺客");
        SetText(txtFight, " 战力   " + PECommon.GetFightByProps(pd));
        SetText(txtHP, " 血量   " + pd.Hp);
        SetText(txtHurt, " 伤害   " + (pd.Ad + pd.Ap));
        SetText(txtDef, " 防御   " + (pd.Addef + pd.Apdef));

        //detail TODO
        SetText(dtxhp, pd.Hp);
        SetText(dtxad, pd.Ad);
        SetText(dtxap, pd.Ap);
        SetText(dtxaddef, pd.Addef);
        SetText(dtxapdef, pd.Apdef);
        SetText(dtxdodge, pd.Dodge + "%");
        SetText(dtxpierce, pd.Pierce + "%");
        SetText(dtxcritical, pd.Critical + "%");

    }

    public void ClickCloseBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        m_InfoFormData.OnClickClose();
    }
    public void ClickDetailBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        SetActive(transDetail);
    }

    public void ClickCloseDetailBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIClickBtn);
        SetActive(transDetail, false);
    }
}