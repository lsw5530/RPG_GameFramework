/****************************************************
	文件：StateIdle.cs
	
	
	
	功能：待机状态
*****************************************************/

using UnityEngine;

public class StateIdle : IState {
    public void Enter(EntityBase entity, params object[] args) {
        entity.CurrentAniState = AniState.Idle;
        entity.SetDir(Vector2.zero);
        entity.SkEndCB = -1;
    }

    public void Exit(EntityBase entity, params object[] args) {
    }

    public void Process(EntityBase entity, params object[] args) {
        if (entity.NextSkillID != 0) {
            entity.Attack(entity.NextSkillID);
        }
        else {
            if (entity.entityType == EntityType.Player) {
                entity.CanRlsSkill = true;
            }

            if (entity.GetDirInput() != Vector2.zero) {
                entity.Move();
                entity.SetDir(entity.GetDirInput());
            }
            else {
                entity.SetBlend(Constants.BlendIdle);
            }
        }
    }
}
