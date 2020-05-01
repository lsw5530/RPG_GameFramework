/****************************************************
	文件：BattleSys.cs
	
	
	功能：战斗业务系统
*****************************************************/

using System;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameMessage;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureBattle : ProcedureBase {
    private BattleMgr m_BattleMgr=new BattleMgr();
    private int m_Fbid;
    private double m_StartTime;
    private  PlayerCtrlFormData m_PlayerCtrlFormData = new PlayerCtrlFormData();
    private BattleEndFormData m_BattleEndFormData = new BattleEndFormData();
    private ProcedureOwner m_ProcedureOwner;
    private Entity m_PlayerEntity;
    public override bool UseNativeDialog {
        get {
            return false;
        }
    }
    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);
        m_ProcedureOwner = procedureOwner;
        AddListener();
        GameEntry.UI.OpenUIForm(UIFormId.PlayerCtrlForm, m_PlayerCtrlFormData);
        Init();
    }
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        base.OnLeave(procedureOwner, isShutdown);
        GameEntry.Entity.HideEntity(m_PlayerEntity);
        RemoveListener();
        GameEntry.UI.CloseUIForm(UIFormId.BattleEndForm);
        GameEntry.UI.CloseUIForm(UIFormId.PlayerCtrlForm);
        GameEntry.UI.RmvAllHpItemInfo();
    }
    private void Init() {
        VarInt fbId = m_ProcedureOwner.GetData<VarInt>(Constant.ProcedureData.FubenId);
        m_Fbid = fbId;
        m_BattleMgr = new BattleMgr();
        m_BattleMgr.Init(fbId, () => {
            m_StartTime = GameEntry.Timer.GetNowTime();
        });
    }
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        if (!GameEntry.Net.IsConnectedToServer) {
            procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Login);
            ChangeState<ProcedureChangeScene>(procedureOwner);
            return;
        }
        m_BattleMgr.Update();
    }
    private void AddListener() {
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspFBFightEnd);
        GameEntry.Event.Subscribe(EndBattleEventArgs.EventId, EndBattle);
        GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        m_BattleEndFormData.OnClickClose = () => { GameEntry.Base.ResetNormalGameSpeed(); };
        m_BattleEndFormData.OnClickExit = OnClickExit;
        m_BattleEndFormData.OnClickSure = EnterFuben;
        m_PlayerCtrlFormData.OnClickSkillAtk = ReqReleaseSkill;
        m_PlayerCtrlFormData.OnClickHead = () => {
            GameEntry.Base.PauseGame();
            SetBattleEndWndState(FBEndType.Pause);
        };
        m_PlayerCtrlFormData.OnClickCanRls = () => { return m_BattleMgr.CanRlsSkill(); };
        m_PlayerCtrlFormData.OnPlayerMove = SetMoveDir;
    }
    private void OnShowEntitySuccess(object sender, GameEventArgs e) {
        ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
        if (ne.EntityLogicType == typeof(PlayerController)) {
            m_PlayerEntity = ne.Entity;
            m_BattleMgr.OnPlayerLoaded(ne.Entity.gameObject);
        }else if(ne.EntityLogicType == typeof(MonsterController)) {
            m_BattleMgr.OnMonsterLoaded(ne.Entity.gameObject);
        }
    }
    protected virtual void OnShowEntityFailure(object sender, GameEventArgs e) {
        ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
        Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
    }
    private void RemoveListener() {
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspFBFightEnd);
        GameEntry.Event.Unsubscribe(EndBattleEventArgs.EventId, EndBattle);
        GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        m_BattleEndFormData.OnClickClose = null;
        m_BattleEndFormData.OnClickExit = null;
        m_BattleEndFormData.OnClickSure = null;
        m_PlayerCtrlFormData.OnClickSkillAtk = null;
        m_PlayerCtrlFormData.OnClickHead = null;
        m_PlayerCtrlFormData.OnClickCanRls = null;
        m_PlayerCtrlFormData.OnPlayerMove = null;
    }
    private void OnClickExit() {//失败后进入主城，取消战斗后进入主城
        m_ProcedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.SceneMainCity);
        ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
    }
    private void EnterFuben() {//胜利后进入副本
        ChangeState<ProcedureFuben>(m_ProcedureOwner);
    }
    /// <summary>
    /// 胜利后请求结算，失败不需要
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EndBattle(object sender, GameEventArgs e) {
        EndBattleEventArgs ne = e as EndBattleEventArgs;
        GameEntry.UI.CloseUIForm(UIFormId.PlayerCtrlForm);
        GameEntry.UI.RmvAllHpItemInfo();

        if (ne.IsWin) {
            double endTime = GameEntry.Timer.GetNowTime();
            //发送结算战斗请求
            //TODO
            CSPacketBase msg = new ReqFBFightEnd {
                Win = ne.IsWin,
                Fbid = m_Fbid,
                Resthp = ne.RestHP,
                Costtime = (int)(endTime - m_StartTime)
            };
            GameEntry.Net.SendMsg(msg);
        }
        else {
            SetBattleEndWndState(FBEndType.Lose);
        }
    }
    private void SetBattleEndWndState(FBEndType endType) {
        m_BattleEndFormData.EndType = endType;
        GameEntry.UI.OpenUIForm(UIFormId.BattleEndForm, m_BattleEndFormData);
    }
    private void RspFBFightEnd(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspFBFightEnd) return;

        RspFBFightEnd data = (RspFBFightEnd)ne.scPacketBase;
        PECommon.SetPlayerDataByFBEnd(data);

        m_BattleEndFormData.Fbid = data.Fbid;
        m_BattleEndFormData.CostTime = data.Costtime;
        m_BattleEndFormData.RestHp = data.Resthp;
        SetBattleEndWndState(FBEndType.Win);
    }
    private void SetMoveDir(Vector2 dir) {
        m_BattleMgr.SetSelfPlayerMoveDir(dir);
    }
    private void ReqReleaseSkill(int index) {
        m_BattleMgr.ReqReleaseSkill(index);
    }
}
