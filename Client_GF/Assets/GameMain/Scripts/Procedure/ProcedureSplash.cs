//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Resource;
using StarForce;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

public class ProcedureSplash : ProcedureBase {
    public override bool UseNativeDialog {
        get {
            return true;
        }
    }

    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds) {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        // TODO: 增加一个 Splash 动画，这里先跳过
        // 编辑器模式下，直接进入预加载流程；
        //如果是在线更新，则检查版本；如果是单机模式则初始化资源
        if (GameEntry.Base.EditorResourceMode) {
            ChangeState<ProcedurePreload>(procedureOwner);
        }
        else {
            if (GameEntry.Resource.ResourceMode == ResourceMode.Updatable) {
                ChangeState<ProcedureCheckVersion>(procedureOwner);
            }
            else if (GameEntry.Resource.ResourceMode == ResourceMode.Package) {
                ChangeState<ProcedureInitResource>(procedureOwner);
            }
        }
    }
}
