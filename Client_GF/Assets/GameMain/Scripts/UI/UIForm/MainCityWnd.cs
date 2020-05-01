/****************************************************
    文件：MainCityWnd.cs
	
    
    
	功能：主城UI界面
*****************************************************/

using System;
using GameFramework;
using GameFramework.Event;
using GameMessage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityWnd : WindowRoot {
    #region UIDefine
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;

    public Animation menuAni;
    public Button btnMenu;

    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;

    public Transform expPrgTrans;

    public Button btnGuide;
    #endregion

    private bool menuState = true;
    private float pointDis;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;
    private AutoGuideCfg curtTaskData;

    private MainCityFormData m_MainCityFormData;

    #region MainFunctions
    protected override void OnInit(object userData) {
        base.OnInit(userData);
        FadeTime = 0;
    }
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(OnRefreshUIFormEventArgs.EventId, RefreshUI);
        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);

        RefreshUI(this, null);
        if (userData != null) {
            m_MainCityFormData = userData as MainCityFormData;
            RegisterTouchEvts();
        }
    }
    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);
        GameEntry.Event.Unsubscribe(OnRefreshUIFormEventArgs.EventId, RefreshUI);
    }

    public void RefreshUI(object sender, GameEventArgs e) {
        OnRefreshUIFormEventArgs ne = e as OnRefreshUIFormEventArgs;
        if ((System.Object)sender != this && ne.formId != UIFormId.MainCityForm) return;
        PlayerData pd = PECommon.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, "体力:" + pd.Power + "/" + PECommon.GetPowerLimit(pd.Lv));
        imgPowerPrg.fillAmount = pd.Power * 1.0f / PECommon.GetPowerLimit(pd.Lv);
        SetText(txtLevel, pd.Lv);
        SetText(txtName, pd.Name);


        #region Expprg
        int expPrgVal = (int)(pd.Exp * 1.0f / PECommon.GetExpUpValByLv(pd.Lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        GridLayoutGroup grid = expPrgTrans.GetComponent<GridLayoutGroup>();

        float globalRate = 1.0F * Constants.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float width = (screenWidth - 180) / 10;

        grid.cellSize = new Vector2(width, 7);

        for (int i = 0; i < expPrgTrans.childCount; i++) {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index) {
                img.fillAmount = 1;
            }
            else if (i == index) {
                img.fillAmount = expPrgVal % 10 * 1.0f / 10;
            }
            else {
                img.fillAmount = 0;
            }
        }
        #endregion

        //设置自动任务图标
        curtTaskData = GameEntry.Res.GetAutoGuideCfg(pd.Guideid);
        if (curtTaskData != null) {
            SetGuideBtnIcon(curtTaskData.npcID);
        }
        else {
            SetGuideBtnIcon(-1);
        }

    }

    private void SetGuideBtnIcon(int npcID) {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID) {
            case Constants.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
            case Constants.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Constants.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Constants.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            default:
                spPath = PathDefine.TaskHead;
                break;
        }
        SetSprite(img, spPath);
    }
    #endregion

    #region ClickEvts
    public void ClickFubenBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickFuBen();
    }

    public void ClickTaskBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickTask();
    }
    public void ClickBuyPowerBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickBuy(0);
    }
    public void ClickMKCoinBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickBuy(1);
    }
    public void ClickStrongBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickStrong();
    }
    public void ClickGuideBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        if (curtTaskData != null) {
            m_MainCityFormData.OnClickGuide(curtTaskData);
        }
        else {
            GameEntry.UI.AddTips("更多引导任务，正在开发中...");
        }
    }
    public void ClickMenuBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIExtenBtn);
        menuState = !menuState;
        AnimationClip clip = null;
        if (menuState) {
            clip = menuAni.GetClip("OpenMCMenu");
        }
        else {
            clip = menuAni.GetClip("CloseMCMenu");
        }
        menuAni.Play(clip.name);
    }
    public void ClickHeadBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickHead();
    }
    public void ClickChatBtn() {
        GameEntry.Sound.PlayUISound(Constants.UIOpenPage);
        m_MainCityFormData.OnClickChat();
    }

    public void RegisterTouchEvts() {
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) => {
            startPos = evt.position;
            SetActive(imgDirPoint);
            imgDirBg.transform.position = evt.position;
        });
        OnClickUp(imgTouch.gameObject, (PointerEventData evt) => {
            imgDirBg.transform.position = defaultPos;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            m_MainCityFormData.OnMoveDir(Vector2.zero);
        });
        OnDrag(imgTouch.gameObject, (PointerEventData evt) => {
            Vector2 dir = evt.position - startPos;
            float len = dir.magnitude;
            if (len > pointDis) {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else {
                imgDirPoint.transform.position = evt.position;
            }
            m_MainCityFormData.OnMoveDir(dir.normalized);
        });
    }
    #endregion
}