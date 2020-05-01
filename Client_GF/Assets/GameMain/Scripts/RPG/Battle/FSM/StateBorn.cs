/****************************************************
	文件：StateBorn.cs
	
	
	
	功能：出生状态
*****************************************************/

public class StateBorn : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.CurrentAniState = AniState.Born;
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        //播放出生动画
        entity.SetAction(Constants.ActionBorn);
        GameEntry.Timer.AddTimeTask((int tid) => {
            entity.SetAction(Constants.ActionDefault);
        }, 500);
    }
}
