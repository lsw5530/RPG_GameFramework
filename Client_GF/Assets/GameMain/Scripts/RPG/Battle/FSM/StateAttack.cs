/****************************************************
	文件：StateAttack.cs
	
	
	
	功能：攻击状态
*****************************************************/

public class StateAttack : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.CurrentAniState = AniState.Attack;
        entity.CurtSkillCfg = GameEntry.Res.GetSkillCfg((int)args[0]);
    }

    public void Exit(EntityBase entity, params object[] args) {
        entity.ExitCurtSkill();
    }

    public void Process(EntityBase entity, params object[] args) {
        if (entity.entityType == EntityType.Player) {
            entity.CanRlsSkill = false;
        }

        entity.SkillAttack((int)args[0]);
    }
}
