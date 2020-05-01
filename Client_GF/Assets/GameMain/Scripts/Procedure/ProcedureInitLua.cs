using GameFramework;
using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain {
    /// <summary>
    /// 初始化Lua流程
    /// </summary>
    public class ProcedureInitLua : ProcedureBase {
        public override bool UseNativeDialog {
            get {
                return true;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner) {
            base.OnEnter(procedureOwner);

            GameEntry.Lua.InitSvc();
            procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, (int)SceneId.Login);
            ChangeState<ProcedureChangeScene>(procedureOwner);
        }

    }
}
