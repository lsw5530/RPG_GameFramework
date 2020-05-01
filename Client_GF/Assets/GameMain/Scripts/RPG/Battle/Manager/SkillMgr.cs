/****************************************************
	文件：SkillMgr.cs
	
	
	
	功能：技能管理器
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public class SkillMgr {
    public void Init() {
    }

    public void SkillAttack(EntityBase entity, int skillID) {
        entity.SkMoveCBLst.Clear();
        entity.SkActionCBLst.Clear();

        AttackDamage(entity, skillID);
        AttackEffect(entity, skillID);
    }
    /// <summary>
    /// 技能伤害
    /// </summary>
    /// <param name="entity">施放者（主角）</param>
    /// <param name="skillID">节能功能ID</param>
    public void AttackDamage(EntityBase entity, int skillID) {
        SkillCfg skillData = GameEntry.Res.GetSkillCfg(skillID);
        List<int> actonLst = skillData.skillActionLst;
        int sum = 0;
        for (int i = 0; i < actonLst.Count; i++) {
            SkillActionCfg skillAction = GameEntry.Res.GetSkillActionCfg(actonLst[i]);
            sum += skillAction.delayTime;
            int index = i;
            if (sum > 0) {
                int actid = GameEntry.Timer.AddTimeTask((int tid) => {
                    if (entity != null) {
                        SkillAction(entity, skillData, index);
                        entity.RmvActionCB(tid);
                    }
                }, sum);
                entity.SkActionCBLst.Add(actid);
            }
            else {
                //瞬时技能
                SkillAction(entity, skillData, index);
            }
        }
    }
    /// <summary>
    /// 瞬时技能
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="skillCfg"></param>
    /// <param name="index"></param>
    public void SkillAction(EntityBase caster, SkillCfg skillCfg, int index) {
        SkillActionCfg skillActionCfg = GameEntry.Res.GetSkillActionCfg(skillCfg.skillActionLst[index]);

        int damage = skillCfg.skillDamageLst[index];
        if (caster.entityType == EntityType.Monster) {
            EntityPlayer target = caster.BattleMgr.EntitySelfPlayer;
            if (target == null) {
                return;
            }
            //判断距离，判断角度
            if (InRange(caster.GetPos(), target.GetPos(), skillActionCfg.radius)
                && InAngle(caster.GetTrans(), target.GetPos(), skillActionCfg.angle)) {
                CalcDamage(caster, target, skillCfg, damage);
            }
        }
        else if (caster.entityType == EntityType.Player) {
            //获取场景里所有的怪物实体，遍历运算
            List<EntityMonster> monsterLst = caster.BattleMgr.GetEntityMonsters();
            for (int i = 0; i < monsterLst.Count; i++) {
                EntityMonster em = monsterLst[i];
                //判断距离，判断角度
                if (InRange(caster.GetPos(), em.GetPos(), skillActionCfg.radius)
                    && InAngle(caster.GetTrans(), em.GetPos(), skillActionCfg.angle)) {
                    CalcDamage(caster, em, skillCfg, damage);
                }
            }
        }
    }

    System.Random rd = new System.Random();
    private void CalcDamage(EntityBase caster, EntityBase target, SkillCfg skillCfg, int damage) {
        int dmgSum = damage;
        if (skillCfg.dmgType == DamageType.AD) {
            //计算闪避
            int dodgeNum = PETools.RDInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge) {
                //UI显示闪避 TODO
                target.SetDodge();
                return;
            }
            //计算属性加成
            dmgSum += caster.Props.ad;

            //计算暴击
            int criticalNum = PETools.RDInt(1, 100, rd);
            if (criticalNum <= caster.Props.critical) {
                float criticalRate = 1 + (PETools.RDInt(1, 100, rd) / 100.0f);
                dmgSum = (int)criticalRate * dmgSum;
                //PECommon.Log("暴击Rate:" + criticalNum + "/" + caster.Props.critical);
                target.SetCritical(dmgSum);
            }

            //计算穿甲
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        else if (skillCfg.dmgType == DamageType.AP) {
            //计算属性加成
            dmgSum += caster.Props.ap;
            //计算魔法抗性
            dmgSum -= target.Props.apdef;
        }
        else { }

        //最终伤害
        if (dmgSum < 0) {
            dmgSum = 0;
            return;
        }
        target.SetHurt(dmgSum);

        if (target.HP < dmgSum) {
            target.HP = 0;
            //目标死亡
            target.Die();
            if (target.entityType == EntityType.Monster) {
                target.BattleMgr.RmvMonster(target.Name);
            }
            else if (target.entityType == EntityType.Player) {
                target.BattleMgr.EndBattle(false, 0);
                target.BattleMgr.EntitySelfPlayer = null;
            }

        }
        else {
            target.HP -= dmgSum;
            if (target.entityState == EntityState.None && target.GetBreakState()) {
                target.Hit();
            }
        }
    }

    private bool InRange(Vector3 from, Vector3 to, float range) {
        float dis = Vector3.Distance(from, to);
        if (dis <= range) {
            return true;
        }
        return false;
    }

    private bool InAngle(Transform trans, Vector3 to, float angle) {
        if (angle == 360) {
            return true;
        }
        else {
            Vector3 start = trans.forward;
            Vector3 dir = (to - trans.position).normalized;

            float ang = Vector3.Angle(start, dir);

            if (ang <= angle / 2) {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillID) {
        SkillCfg skillData = GameEntry.Res.GetSkillCfg(skillID);

        if (!skillData.isCollide) {
            //忽略掉刚体碰撞
            Physics.IgnoreLayerCollision(9, 10);
            GameEntry.Timer.AddTimeTask((int tid) => {
                Physics.IgnoreLayerCollision(9, 10, false);
            }, skillData.skillTime);
        }

        if (entity.entityType == EntityType.Player) {
            if (entity.GetDirInput() == Vector2.zero) {
                Vector2 dir = entity.CalcTargetDir();
                if (dir != Vector2.zero) {
                    entity.SetAtkRotation(dir, false);
                }
            }
            else {
                entity.SetAtkRotation(entity.GetDirInput(), true);
            }
        }

        entity.SetAction(skillData.aniAction);
        entity.SetFX(skillData.fx, skillData.skillTime);

        CalcSkillMove(entity, skillData);

        entity.CanControl = false;
        entity.SetDir(Vector2.zero);

        if (!skillData.isBreak) {
            entity.entityState = EntityState.BatiState;
        }

        entity.SkEndCB = GameEntry.Timer.AddTimeTask((int tid) => {
            entity.Idle();
        }, skillData.skillTime);
    }
    /// <summary>
    /// 设置技能移动，并添加回调编号
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillData"></param>
    private void CalcSkillMove(EntityBase entity, SkillCfg skillData) {
        List<int> skillMoveLst = skillData.skillMoveLst;
        int delay = 0;
        for (int i = 0; i < skillMoveLst.Count; i++) {
            SkillMoveCfg skillMoveCfg = GameEntry.Res.GetSkillMoveCfg(skillData.skillMoveLst[i]);
            float speed = skillMoveCfg.moveDis / (skillMoveCfg.moveTime / 1000f);
            delay += skillMoveCfg.delayTime;
            if (delay > 0) {
                int moveid = GameEntry.Timer.AddTimeTask((int tid) => {
                    entity.SetSkillMoveState(true, speed);
                    entity.RmvMoveCB(tid);//移除移动回调编号
                }, delay);
                entity.SkMoveCBLst.Add(moveid);//添加移动回调编号
            }
            else {
                entity.SetSkillMoveState(true, speed);
            }

            delay += skillMoveCfg.moveTime;
            int stopid = GameEntry.Timer.AddTimeTask((int tid) => {
                entity.SetSkillMoveState(false);
                entity.RmvMoveCB(tid);//移除回调编号
            }, delay);
            entity.SkMoveCBLst.Add(stopid);//添加回调编号
        }
    }
}