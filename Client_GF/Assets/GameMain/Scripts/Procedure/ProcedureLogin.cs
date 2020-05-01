//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using GameFramework.Event;
using GameMessage;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureLogin : ProcedureBase {
    private bool m_StartGame = false;
    private bool m_LoginSuccess = false;
    private bool m_GetPlayerSuccess = false;
    private ProcedureOwner m_procedureOwner;
    private LoginFormData m_LoginFormData = new LoginFormData();
    public override bool UseNativeDialog {
        get {
            return false;
        }
    }

    public void StartGame() {
        m_StartGame = true;
    }

    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspLogin);
        GameEntry.Event.Subscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspRename);

        m_procedureOwner = procedureOwner;
        m_StartGame = false;
        m_GetPlayerSuccess = false;
        m_LoginFormData.OnClickEnter = OnClickEnter;
        m_LoginFormData.OnClickNotice = OnClickNotice;
        GameEntry.UI.OpenUIForm(UIFormId.LoginForm, m_LoginFormData);
        GameEntry.UI.OpenUIForm(UIFormId.DynamicForm);
    }

    private void OnClickNotice() {
        GameEntry.UI.AddTips("功能正在开发中...");
    }

    private void OnClickEnter(string acc, string pass) {
        //发送网络消息，请求登录
        CSPacketBase msg = new ReqLogin {
            Acct = acc,
            Pass = pass
        };
        GameEntry.Net.SendMsg(msg);
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        base.OnLeave(procedureOwner, isShutdown);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspLogin);
        GameEntry.Event.Unsubscribe(OnReceiveSCPacketBaseEventArgs.EventId, RspRename);

        GameEntry.UI.CloseUIForm(UIFormId.LoginForm);
    }
    private void RspLogin(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspLogin) return;
        RspLogin rspLogin = (RspLogin)ne.scPacketBase;
        if(rspLogin.PlayerData==null) {
            Debug.LogError("kongxiaoxi");
            return;
        }
        PECommon.SetPlayerData(rspLogin);
        if (rspLogin.PlayerData.Name == "") {
            GameEntry.UI.OpenUIForm(UIFormId.CreateForm);
        }
        else {
            m_GetPlayerSuccess = true;

        }
    } 
    private void RspRename(object sender, GameEventArgs e) {
        OnReceiveSCPacketBaseEventArgs ne = e as OnReceiveSCPacketBaseEventArgs;
        if (ne.msgID != CMD.RspRename) return;
        RspRename rspRename = (RspRename)ne.scPacketBase;
        PECommon.SetPlayerName(rspRename.Name);

        //跳转场景进入主城
        m_procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.SceneMainCity);
        ChangeState<ProcedureChangeScene>(m_procedureOwner);
        //关闭创建界面
        GameEntry.UI.CloseUIForm(UIFormId.CreateForm);
    }
    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (m_GetPlayerSuccess) {
            procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.SceneMainCity);
            procedureOwner.SetData<VarInt>(Constant.ProcedureData.GameMode, (int)GameMode.Survival);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }
    }

}
