/****************************************************
    文件：PlayerCtrlWnd.cs
	
    
    	功能：玩家控制界面
*****************************************************/

using System;
using GameFramework;
using GameFramework.Event;
using GameMessage;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerCtrlWnd : WindowRoot {
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Transform expPrgTrans;

    private float pointDis;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos = Vector2.zero;

    public Vector2 currentDir;

    public Transform transBossHPBar;
    public Image imgRed;
    public Image imgYellow;
    private float currentPrg = 1f;
    private float targetPrg = 1f;
    #region Skill
    #region SK1
    public Image imgSk1CD;
    public Text txtSk1CD;
    private bool isSk1CD = false;
    private float sk1CDTime;
    private int sk1Num;
    private float sk1FillCount = 0;
    private float sk1NumCount = 0;
    #endregion

    #region SK2
    public Image imgSk2CD;
    public Text txtSk2CD;
    private bool isSk2CD = false;
    private float sk2CDTime;
    private int sk2Num;
    private float sk2FillCount = 0;
    private float sk2NumCount = 0;
    #endregion

    #region SK3
    public Image imgSk3CD;
    public Text txtSk3CD;
    private bool isSk3CD = false;
    private float sk3CDTime;
    private int sk3Num;
    private float sk3FillCount = 0;
    private float sk3NumCount = 0;
    #endregion
    #endregion

    public Text txtSelfHP;
    public Image imgSelfHP;

    private int HPSum;
    private PlayerCtrlFormData m_PlayerCtrlFormData;
 
    protected override void OnOpen(object userData) {
        base.OnOpen(userData);
        if(userData!=null)
        m_PlayerCtrlFormData = userData as PlayerCtrlFormData;

        pointDis = Screen.height * 1.0f / Constants.ScreenStandardHeight * Constants.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);

        HPSum = PECommon.PlayerData.Hp;
        SetText(txtSelfHP, HPSum + "/" + HPSum);
        imgSelfHP.fillAmount = 1;

        SetBossHPBarState();
        RegisterTouchEvts();
        sk1CDTime = GameEntry.Res.GetSkillCfg(101).cdTime / 1000.0f;
        sk2CDTime = GameEntry.Res.GetSkillCfg(102).cdTime / 1000.0f;
        sk3CDTime = GameEntry.Res.GetSkillCfg(103).cdTime / 1000.0f;

        RefreshUI();
        GameEntry.Event.Subscribe(RefreshSelfHpEventArgs.EventId, SetSelfHPBarVal);
        GameEntry.Event.Subscribe(OnBossHPBarChangeEventArgs.EventId, SetBossHPBarVal);
        GameEntry.Event.Subscribe(SetBossHPBarStateEventArgs.EventId, SetBossHPBarState);
        Init();
    }
     
    private void Init() {
        isSk1CD = false;
        SetActive(imgSk1CD, false);
        sk1FillCount = 0;

        isSk2CD = false;
        SetActive(imgSk2CD, false);
        sk2FillCount = 0;

        isSk3CD = false;
        SetActive(imgSk3CD, false);
        sk3FillCount = 0;
    }

    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData); 
        GameEntry.Event.Unsubscribe(RefreshSelfHpEventArgs.EventId, SetSelfHPBarVal);
        GameEntry.Event.Unsubscribe(OnBossHPBarChangeEventArgs.EventId, SetBossHPBarVal);
        GameEntry.Event.Unsubscribe(SetBossHPBarStateEventArgs.EventId, SetBossHPBarState);
    }

    public void RefreshUI() {
        PlayerData pd = PECommon.PlayerData;

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
    }
  
    private void Update() {
        //TEST
        if (Input.GetKeyDown(KeyCode.A)) {
            ClickNormalAtk();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            ClickSkill1Atk();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            ClickSkill2Atk();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            ClickSkill3Atk();
        }

        float delta = Time.deltaTime;
        #region Skill CD
        if (isSk1CD) {
            sk1FillCount += delta;
            if (sk1FillCount >= sk1CDTime) {
                isSk1CD = false;
                SetActive(imgSk1CD, false);
                sk1FillCount = 0;
            }
            else {
                imgSk1CD.fillAmount = 1 - sk1FillCount / sk1CDTime;
            }

            sk1NumCount += delta;
            if (sk1NumCount >= 1) {
                sk1NumCount -= 1;
                sk1Num -= 1;
                SetText(txtSk1CD, sk1Num);
            }
        }

        if (isSk2CD) {
            sk2FillCount += delta;
            if (sk2FillCount >= sk2CDTime) {
                isSk2CD = false;
                SetActive(imgSk2CD, false);
                sk2FillCount = 0;
            }
            else {
                imgSk2CD.fillAmount = 1 - sk2FillCount / sk2CDTime;
            }

            sk2NumCount += delta;
            if (sk2NumCount >= 1) {
                sk2NumCount -= 1;
                sk2Num -= 1;
                SetText(txtSk2CD, sk2Num);
            }
        }

        if (isSk3CD) {
            sk3FillCount += delta;
            if (sk3FillCount >= sk3CDTime) {
                isSk3CD = false;
                SetActive(imgSk3CD, false);
                sk3FillCount = 0;
            }
            else {
                imgSk3CD.fillAmount = 1 - sk3FillCount / sk3CDTime;
            }

            sk3NumCount += delta;
            if (sk3NumCount >= 1) {
                sk3NumCount -= 1;
                sk3Num -= 1;
                SetText(txtSk3CD, sk3Num);
            }
        }
        #endregion

        if (transBossHPBar.gameObject.activeSelf) {
            BlendBossHP();
            imgYellow.fillAmount = currentPrg;
        }
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
            currentDir = Vector2.zero;
            PlayerMoveDirEventArgs eventArgs = ReferencePool.Acquire<PlayerMoveDirEventArgs>();
            m_PlayerCtrlFormData.OnPlayerMove(currentDir);
            eventArgs.Fill(currentDir);
            //GameEntry.Event.FireNow(this,eventArgs);
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
            currentDir = dir.normalized;
            PlayerMoveDirEventArgs eventArgs = ReferencePool.Acquire<PlayerMoveDirEventArgs>();
            m_PlayerCtrlFormData.OnPlayerMove(currentDir);
            eventArgs.Fill(currentDir);
            //GameEntry.Event.FireNow(this, eventArgs);
        });
    }

    public void ClickNormalAtk() {
        m_PlayerCtrlFormData.OnClickSkillAtk(0);
    }

    public void ClickSkill1Atk() {
        if (isSk1CD == false && GetCanRlsSkill()) {
            m_PlayerCtrlFormData.OnClickSkillAtk(1);
            isSk1CD = true;
            SetActive(imgSk1CD);
            imgSk1CD.fillAmount = 1;
            sk1Num = (int)sk1CDTime;
            SetText(txtSk1CD, sk1Num);
        }
    }
    public void ClickSkill2Atk() {
        if (isSk2CD == false && GetCanRlsSkill()) {
            m_PlayerCtrlFormData.OnClickSkillAtk(2);
            isSk2CD = true;
            SetActive(imgSk2CD);
            imgSk2CD.fillAmount = 1;
            sk2Num = (int)sk2CDTime;
            SetText(txtSk2CD, sk2Num);
        }
    }
    public void ClickSkill3Atk() {
        if (isSk3CD == false && GetCanRlsSkill()) {
            m_PlayerCtrlFormData.OnClickSkillAtk(3);
            isSk3CD = true;
            SetActive(imgSk3CD);
            imgSk3CD.fillAmount = 1;
            sk3Num = (int)sk3CDTime;
            SetText(txtSk3CD, sk3Num);
        }
    }

    //Test Reset Data
    public void ClickResetCfgs() {
        GameEntry.Res.ResetSkillCfgs();
    }
    public void ClickHeadBtn() {
        m_PlayerCtrlFormData.OnClickHead();
        //ProcedureBattle.Instance.battleMgr.isPauseGame = true;
        //ProcedureBattle.Instance.SetBattleEndWndState(FBEndType.Pause);
    }


    public void SetSelfHPBarVal(object sender, GameEventArgs e) {
        RefreshSelfHpEventArgs ne = e as RefreshSelfHpEventArgs;
        
        SetText(txtSelfHP, ne.NewVal + "/" + HPSum);
        imgSelfHP.fillAmount = ne.NewVal * 1.0f / HPSum;
    }

    public bool GetCanRlsSkill() {
        return m_PlayerCtrlFormData.OnClickCanRls();
    }

    public void SetBossHPBarState() {
        SetActive(transBossHPBar, false);
        imgRed.fillAmount = 1;
        imgYellow.fillAmount = 1;
    }
    public void SetBossHPBarState(object sender, GameEventArgs e) {
        SetBossHPBarStateEventArgs ne = e as SetBossHPBarStateEventArgs;
        SetActive(transBossHPBar, ne.State);
        imgRed.fillAmount = 1;
        imgYellow.fillAmount = 1;
    }
    public void SetBossHPBarVal(object sender, GameEventArgs e) {
        OnBossHPBarChangeEventArgs ne = e as OnBossHPBarChangeEventArgs;

        currentPrg = ne.OldVal* 1.0f / ne.SumVal;
        targetPrg = ne.NewVal* 1.0f / ne.SumVal;
        imgRed.fillAmount = targetPrg;
    }
    private void BlendBossHP() {
        if (Mathf.Abs(currentPrg - targetPrg) < Constants.AccelerHPSpeed * Time.deltaTime) {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg) {
            currentPrg -= Constants.AccelerHPSpeed * Time.deltaTime;
        }
        else {
            currentPrg += Constants.AccelerHPSpeed * Time.deltaTime;
        }
    } 
}