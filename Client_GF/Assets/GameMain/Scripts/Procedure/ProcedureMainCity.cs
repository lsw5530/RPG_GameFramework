using System;
using GameFramework;
using GameFramework.Event;
using GameMessage;
using StarForce;
using UnityEngine;
using UnityEngine.AI;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

class ProcedureMainCity : ProcedureBase {
    private PlayerController m_PlayerCtrl;
    private PlayerData m_PlayerData;
    private Transform m_CharCamTrans;
    private AutoGuideCfg m_CurtTaskData;
    private Transform[] m_NpcPosTrans;
    private MainCityFormData m_MainCityFormData = new MainCityFormData();
    private NavMeshAgent m_Nav;
    private ProcedureOwner m_ProcedureOwner;
    private InfoFormData m_InfoFormData = new InfoFormData();
    private TaskFormData m_TaskFormData = new TaskFormData();
    private int playerEntityId;
    public override bool UseNativeDialog {
        get {
            return false;
        }
    }
    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);
        m_ProcedureOwner = procedureOwner;
        m_PlayerData = PECommon.PlayerData;
        AddListener();
        //打开主城场景UI
        GameEntry.UI.OpenUIForm(UIFormId.MainCityForm, m_MainCityFormData);
        Init();
    }
    MapCfg m_MapData;
    private void Init() {
        m_MapData = GameEntry.Res.GetMapCfg(Constants.MainCityMapID);
        // 加载游戏主角
        LoadPlayer(); 
        MainCityMap mcm = UnityEngine.Object.FindObjectOfType<MainCityMap>();
        m_NpcPosTrans = mcm.NpcPosTrans;
        //设置人物展示相机
        if (m_CharCamTrans != null) {
            m_CharCamTrans.gameObject.SetActive(false);
        }
    }
    private void AddListener() {
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspTakeTaskReward);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspBuy);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshPower);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshChat);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspGuide);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshTaskPrgs);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspStrong);
        GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        m_MainCityFormData.OnClickStrong = OpenStrongWnd;
        m_MainCityFormData.OnClickFuBen = EnterFuben;
        m_MainCityFormData.OnClickTask = OpenTaskRewardWnd;
        m_MainCityFormData.OnClickBuy = OpenBuyWnd;
        m_MainCityFormData.OnClickGuide = RunTask;
        m_MainCityFormData.OnClickHead = OpenInfoWnd;
        m_MainCityFormData.OnClickChat = OpenChatWnd;
        m_MainCityFormData.OnMoveDir = SetMoveDir;
    }
    
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        base.OnLeave(procedureOwner, isShutdown);
        RemoveListener();
        GameEntry.Entity.HideEntity(m_PlayerCtrl);
        GameEntry.UI.CloseUIForm(UIFormId.MainCityForm);
    } 
    private void RemoveListener() {
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspTakeTaskReward);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspBuy);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshPower);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshChat);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspGuide);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, PshTaskPrgs);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspStrong);
        GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        m_MainCityFormData.OnClickStrong = null;
        m_MainCityFormData.OnClickFuBen = null;
        m_MainCityFormData.OnClickTask = null;
        m_MainCityFormData.OnClickBuy = null;
        m_MainCityFormData.OnClickGuide = null;
        m_MainCityFormData.OnClickHead = null;
        m_MainCityFormData.OnClickChat = null;
        m_MainCityFormData.OnMoveDir = null;
    }
    private void RspTakeTaskReward(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne == null) return;
        if (ne.msgID != CMD.RspTakeTaskReward) return;
        RspTakeTaskReward data = (RspTakeTaskReward)ne.scPacketBase;
        PECommon.SetPlayerDataByTask(data);

        OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
        eventArgs.Fill(UIFormId.TaskForm);
        GameEntry.Event.FireNow(this, eventArgs);
        eventArgs.Fill(UIFormId.MainCityForm);
        GameEntry.Event.FireNow(this, eventArgs);
    }
    private void LoadPlayer() {
        playerEntityId = GameEntry.Entity.GenerateSerialId();
        PlayerEntityData playerEntityData = new PlayerEntityData(GameEntry.Entity.GenerateSerialId(), PathDefine.AssissnCityPlayerId, ActorType.Player) {
            Name = "MainCityPlayer",
            Position = m_MapData.playerBornPos,
            Rotation = Quaternion.Euler(m_MapData.playerBornRote),
            LocalScale = new Vector3(1.5F, 1.5F, 1.5F)
        };
        GameEntry.Entity.ShowMyPlayer(playerEntityData);
    }
    protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e) {
        ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
        if (ne.EntityLogicType == typeof(PlayerController)) {
            m_PlayerCtrl = (PlayerController)ne.Entity.Logic;
            //相机初始化
            Camera.main.transform.position = m_MapData.mainCamPos;
            Camera.main.transform.localEulerAngles = m_MapData.mainCamRote;

            m_PlayerCtrl.Init();
            m_Nav = m_PlayerCtrl.GetComponent<NavMeshAgent>();
        }
    }
    protected virtual void OnShowEntityFailure(object sender, GameEventArgs e) {
        ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
        Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
    }
    private void SetMoveDir(Vector2 dir) {
        StopNavTask();

        if (dir == Vector2.zero) {
            m_PlayerCtrl.SetBlend(Constants.BlendIdle);
        }
        else {
            m_PlayerCtrl.SetBlend(Constants.BlendMove);
        }
        m_PlayerCtrl.Dir = dir;
    }
    #region  Enter FubenSys
    private void EnterFuben() {
        StopNavTask();
        ChangeState<ProcedureFuben>(m_ProcedureOwner);
    }
    #endregion

    #region Task Wnd
    private void OpenTaskRewardWnd() {
        StopNavTask();
        m_TaskFormData.PlayerData = m_PlayerData;
        m_TaskFormData.OnClickTakeTask = OnClickTakeTask;
        GameEntry.UI.OpenUIForm(UIFormId.TaskForm, m_TaskFormData);
    }

    private void OnClickTakeTask(int taskId) {
        CSPacketBase msg = new ReqTakeTaskReward {
            Rid = taskId
        };
        GameEntry.Net.SendMsg(msg);
    }

    public void PshTaskPrgs(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.PshTaskPrgs) return;
        PshTaskPrgs data = (PshTaskPrgs)ne.scPacketBase;
        PECommon.SetPlayerDataByTaskPsh(data);
        OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
        eventArgs.Fill(UIFormId.TaskForm);
        GameEntry.Event.FireNow(this, eventArgs);
    }
    #endregion

    #region Buy Wnd
    private void OpenBuyWnd(int type) {
        StopNavTask();
        BuyFormData data = new BuyFormData();
        data.BuyType = type;
        GameEntry.UI.OpenUIForm(UIFormId.BuyForm, data);
    }
    private void RspBuy(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspBuy) return;
        RspBuy rspBuy = (RspBuy)ne.scPacketBase;
        PECommon.SetPlayerDataByBuy(rspBuy);
        GameEntry.UI.AddTips("购买成功");

        OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
        eventArgs.Fill(UIFormId.MainCityForm);
        GameEntry.Event.Fire(this, eventArgs);
        GameEntry.UI.CloseUIForm(UIFormId.BuyForm);
    }

    private void PshPower(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.PshPower) return;

        PshPower data = ne.scPacketBase as PshPower;
        PECommon.SetPlayerDataByPower(data);
        UGuiForm mainCityForm = GameEntry.UI.GetUIForm(UIFormId.MainCityForm);
        if (!mainCityForm) return;
        if (mainCityForm.Visible) {
            OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
            eventArgs.Fill(UIFormId.MainCityForm);
            GameEntry.Event.FireNow(this, eventArgs);
        }
    }
    #endregion

    #region Chat Wnd
    private void OpenChatWnd() {
        StopNavTask();
        GameEntry.UI.OpenUIForm(UIFormId.ChatForm);
    }
    private void PshChat(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.PshChat) return;

        PshChat pshChat = (PshChat)ne.scPacketBase;

        RefreshChatUIEventArgs eventArgs = ReferencePool.Acquire<RefreshChatUIEventArgs>();
        eventArgs.Fill(pshChat.Name, pshChat.Chat);
        GameEntry.Event.Fire(this, eventArgs);
    }

    #endregion

    #region Strong Wnd
    private void OpenStrongWnd() {
        StopNavTask();
        GameEntry.UI.OpenUIForm(UIFormId.StrongForm);
    }

    private void RspStrong(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspStrong) return;
        int zhanliPre = PECommon.GetFightByProps(PECommon.PlayerData);
        PECommon.SetPlayerDataByStrong((RspStrong)ne.scPacketBase);
        int zhanliNow = PECommon.GetFightByProps(PECommon.PlayerData);
        GameEntry.UI.AddTips(Constants.Color("战力提升 " + (zhanliNow - zhanliPre), TxtColor.Blue));

        OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
        eventArgs.Fill(UIFormId.StrongForm);
        GameEntry.Event.FireNow(this, eventArgs);
        eventArgs.Fill(UIFormId.MainCityForm);
        GameEntry.Event.FireNow(this, eventArgs);
    }
    #endregion

    #region Info Wnd
    private void OpenInfoWnd() {
        StopNavTask();
        if (m_CharCamTrans == null) {
            m_CharCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        //设置人物展示相机相对位置
        m_CharCamTrans.localPosition = m_PlayerCtrl.transform.position + m_PlayerCtrl.transform.forward * 3.8f + new Vector3(0, 1.2f, 0);
        m_CharCamTrans.localEulerAngles = new Vector3(0, 180 + m_PlayerCtrl.transform.localEulerAngles.y, 0);
        m_CharCamTrans.localScale = Vector3.one;
        m_CharCamTrans.gameObject.SetActive(true);
        m_InfoFormData = new InfoFormData();
        m_InfoFormData.OnClickDown = SetStartRoate;
        m_InfoFormData.OnDrag = SetPlayerRoate;
        m_InfoFormData.OnClickClose = CloseInfoWnd;
        GameEntry.UI.OpenUIForm(UIFormId.InfoForm, m_InfoFormData);
    }
    private void CloseInfoWnd() {
        if (m_CharCamTrans != null) {
            m_CharCamTrans.gameObject.SetActive(false);
            GameEntry.UI.CloseUIForm(UIFormId.InfoForm);
        }
    }
    private float startRoate = 0;
    private void SetStartRoate() {
        startRoate = m_PlayerCtrl.transform.localEulerAngles.y;
    }

    private void SetPlayerRoate(float roate) {
        m_PlayerCtrl.transform.localEulerAngles = new Vector3(0, startRoate + roate, 0);
    }
    #endregion

    #region Guide Wnd
    private bool isNavGuide = false;
    private void RunTask(AutoGuideCfg agc) {
        if (agc != null) {
            m_CurtTaskData = agc;
        }
        //解析任务数据
        m_Nav.enabled = true;
        if (m_CurtTaskData.npcID != -1) {
            float dis = Vector3.Distance(m_PlayerCtrl.transform.position, m_NpcPosTrans[agc.npcID].position);
            if (dis < 0.5f) {
                isNavGuide = false;
                m_Nav.isStopped = true;
                m_PlayerCtrl.SetBlend(Constants.BlendIdle);
                m_Nav.enabled = false;

                OpenGuideWnd();
            }
            else {
                isNavGuide = true;
                m_Nav.enabled = true;
                m_Nav.speed = Constants.PlayerMoveSpeed;
                m_Nav.SetDestination(m_NpcPosTrans[agc.npcID].position);
                m_PlayerCtrl.SetBlend(Constants.BlendMove);
            }
        }
        else {
            OpenGuideWnd();
        }
    }
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
       if (!GameEntry.Net.IsConnectedToServer) {
            procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Login);
            ChangeState<ProcedureChangeScene>(procedureOwner);
            return;
        }
        if (isNavGuide) {
            IsArriveNavPos();
            m_PlayerCtrl.SetCam();
        }
    }
    private void IsArriveNavPos() {
        float dis = Vector3.Distance(m_PlayerCtrl.transform.position, m_NpcPosTrans[m_CurtTaskData.npcID].position);
        if (dis < 0.5f) {
            isNavGuide = false;
            m_Nav.isStopped = true;
            m_PlayerCtrl.SetBlend(Constants.BlendIdle);
            m_Nav.enabled = false;

            OpenGuideWnd();
        }
    }
    private void StopNavTask() {
        if (isNavGuide) {
            isNavGuide = false;

            m_Nav.isStopped = true;
            m_Nav.enabled = false;
            m_PlayerCtrl.SetBlend(Constants.BlendIdle);
        }
    }
    private void OpenGuideWnd() {
        GuideFormData guideFormData = new GuideFormData();
        guideFormData.curtTaskData = m_CurtTaskData;
        GameEntry.UI.OpenUIForm(UIFormId.GuideForm, guideFormData);
    }
    private void RspGuide(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspGuide) return;
        RspGuide data = (RspGuide)ne.scPacketBase;
        GameEntry.UI.AddTips(Constants.Color("任务奖励 金币+" + m_CurtTaskData.coin + "  经验+" + m_CurtTaskData.exp, TxtColor.Blue));
        switch (m_CurtTaskData.actID) {
            case 0:
                //与智者对话
                break;
            case 1:
                EnterFuben();
                break;
            case 2:
                //进入强化界面
                OpenStrongWnd();
                break;
            case 3:
                //进入体力购买
                OpenBuyWnd(0);
                break;
            case 4:
                //进入金币铸造
                OpenBuyWnd(1);
                break;
            case 5:
                //进入世界聊天
                OpenChatWnd();
                break;
        }
        PECommon.SetPlayerDataByGuide(data);
        OnRefreshUIFormEventArgs eventArgs = ReferencePool.Acquire<OnRefreshUIFormEventArgs>();
        eventArgs.Fill(UIFormId.MainCityForm);
        GameEntry.Event.Fire(this, eventArgs);
    }
    #endregion
}
