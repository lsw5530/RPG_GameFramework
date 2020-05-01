using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityGameFramework.Runtime;
using StarForce;
/// <summary>
/// 单机模式
/// </summary>
public class ProcedureInitResource : ProcedureBase {
    private bool m_InitResourceComplete = false;

    public override bool UseNativeDialog {
        get { return true; }
    }

    protected override void OnEnter(ProcedureOwner procedureOwner) {
        base.OnEnter(procedureOwner);

        m_InitResourceComplete = false;
        GameEntry.Resource.InitResources(OnResourceInitComplete);
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (!m_InitResourceComplete) {
            return;
        }

        ChangeState<ProcedurePreload>(procedureOwner);
    }

    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown) {
        base.OnLeave(procedureOwner, isShutdown);
    }

    private void OnResourceInitComplete() {
        m_InitResourceComplete = true;
        Log.Info("Init Resources success.");
    }
}
