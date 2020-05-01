
/****************************************************
	文件：StateMgr.cs
	
	
	
	功能：状态管理器
*****************************************************/

using System.Collections.Generic;

public class StateMgr {
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init() {
        fsm.Clear();
        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Hit, new StateHit());
        fsm.Add(AniState.Die, new StateDie());

        PECommon.Log("Init StateMgr Done.");
    }

    public void ChangeStatus(EntityBase entity, AniState targetState, params object[] args) {
        if (entity.CurrentAniState == targetState) {
            return;
        }

        if (fsm.ContainsKey(targetState)) {
            if (entity.CurrentAniState != AniState.None) {
                fsm[entity.CurrentAniState].Exit(entity, args);
            }
            fsm[targetState].Enter(entity, args);
            fsm[targetState].Process(entity, args);
        }
    }
}
