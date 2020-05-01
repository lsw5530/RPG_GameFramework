/****************************************************
	文件：StateMove.cs
	
	
	
	功能：移动状态
*****************************************************/

public class StateMove : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.CurrentAniState = AniState.Move;
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        entity.SetBlend(Constants.BlendMove);
    }
}
