using System;
using GameFramework.DataTable;
using GameFramework.Event;
using GameMessage;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
/// <summary>
/// 副本状态所在场景可能是主城，也可能是战斗场景
/// </summary>
public class ProcedureFuben : ProcedureBase {
    ProcedureOwner m_ProcedureOwner;
    FubenFormData m_FubenFormData = new FubenFormData();
    public override bool UseNativeDialog {
        get {
            return false;
        }
    }
    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);
        m_ProcedureOwner = procedureOwner;
        m_FubenFormData.PlayerData = PECommon.PlayerData;
        m_FubenFormData.OnClickClose = OnClickClose;
        m_FubenFormData.OnClickTask = OnClickTask;
        GameEntry.UI.OpenUIForm(UIFormId.FubenForm, m_FubenFormData);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspFBFight);
    }
    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        base.OnLeave(procedureOwner, isShutdown);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspFBFight);
        GameEntry.UI.CloseUIForm(UIFormId.FubenForm);
    }
    private void OnClickTask(int fbid) {
        //检查体力是否足够
        int power = GameEntry.Res.GetMapCfg(fbid).power;
        if (power > m_FubenFormData.PlayerData.Power) {
            GameEntry.UI.AddTips("体力值不足");
        }
        else {
            GameEntry.Net.SendMsg(new ReqFBFight {
                Fbid = fbid
            });
        }
    }

    private void OnClickClose() {

        if (PECommon.InSecene(SceneId.SceneMainCity)) {
            ChangeState<ProcedureMainCity>(m_ProcedureOwner);
        }
        else {
            m_ProcedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.SceneMainCity);
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
    }
    //private bool IsInMainCity() {
    //    bool mainCityActivity = false;
    //    IDataTable<DRScene> dtScene = GameEntry.DataTable.GetDataTable<DRScene>();
    //    DRScene drScene = dtScene.GetDataRow((int)SceneId.SceneMainCity);
    //    string sceneAssetName = AssetUtility.GetSceneAsset(drScene.AssetName);
    //    mainCityActivity = GameEntry.Scene.SceneIsLoaded(sceneAssetName);
    //    return mainCityActivity;
    //}
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }
    private void RspFBFight(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspFBFight) return;
        RspFBFight rspFBFight = (RspFBFight)ne.scPacketBase;
        PECommon.SetPlayerDataByFBStart(rspFBFight);
        GameEntry.UI.CloseUIForm(UIFormId.FubenForm);
        if (PECommon.InSecene(SceneId.SceneMainCity)) {
            m_ProcedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.SceneOrge);
            m_ProcedureOwner.SetData<VarInt>(Constant.ProcedureData.FubenId, rspFBFight.Fbid);
            ChangeState<ProcedureChangeScene>(m_ProcedureOwner);
        }
        else {
            ChangeState<ProcedureBattle>(m_ProcedureOwner);
        }

    }

}
