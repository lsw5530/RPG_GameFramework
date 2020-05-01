/****************************************************
	文件：EntityMonster.cs
	
	
	
	功能：怪物逻辑实体
*****************************************************/

using GameFramework;
using UnityEngine;

public class EntityMonster : EntityBase {

    public EntityMonster() {
        entityType = EntityType.Monster;
    }
    public MonsterData Md { get; set; }
    private float m_CheckTime = 2;
    private float m_CheckCountTime = 0;
    /// <summary>
    /// 攻击间隔
    /// </summary>
    private float m_AtkTime = 2;
    /// <summary>
    /// 攻击计时器
    /// </summary>
    private float m_AtkCountTime = 0;
    public override void SetBattleProps(BattleProps props) {
        int level = Md.mLevel;

        BattleProps p = new BattleProps {
            hp = props.hp * level,
            ad = props.ad * level,
            ap = props.ap * level,
            addef = props.addef * level,
            apdef = props.apdef * level,
            dodge = props.dodge * level,
            pierce = props.pierce * level,
            critical = props.critical * level
        };

        Props = p;
        HP = p.hp;
    }

    bool runAI = true;
    public override void TickAILogic() {
        if (!runAI) {
            return;
        }

        if (CurrentAniState == AniState.Idle || CurrentAniState == AniState.Move) {
            if (BattleMgr.IsPauseGame) {
                Idle();
                return;
            }

            float delta = Time.deltaTime;
            m_CheckCountTime += delta;
            if (m_CheckCountTime < m_CheckTime) {
                return;
            }
            else {
                Vector2 dir = CalcTargetDir();
                if (!InAtkRange()) {
                    SetDir(dir);
                    Move();
                }
                else {
                    SetDir(Vector2.zero);//设置静止
                    m_AtkCountTime += m_CheckCountTime;
                    if (m_AtkCountTime > m_AtkTime) {
                        SetAtkRotation(dir,false);
                        Attack(Md.mCfg.skillID);
                        m_AtkCountTime = 0;
                    }
                    else {
                        Idle();
                    }
                }
                m_CheckCountTime = 0;
                m_CheckTime = PETools.RDInt(1, 5) * 1.0f / 10;
            }
        }
    }

    public override Vector2 CalcTargetDir() {
        EntityPlayer entityPlayer = BattleMgr.EntitySelfPlayer;
        if (entityPlayer == null || entityPlayer.CurrentAniState == AniState.Die) {
            runAI = false;
            return Vector2.zero;
        }
        else {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }
    }

    private bool InAtkRange() {
        EntityPlayer entityPlayer = BattleMgr.EntitySelfPlayer;
        if (entityPlayer == null || entityPlayer.CurrentAniState == AniState.Die) {
            runAI = false;
            return false;
        }
        else {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = 0;
            self.y = 0;
            float dis = Vector3.Distance(target, self);
            if (dis <= Md.mCfg.atkDis) {
                return true;
            }
            else {
                return false;
            }
        }
    }

    public override bool GetBreakState() {
        if (Md.mCfg.isStop) {
            if (CurtSkillCfg != null) {
                return CurtSkillCfg.isBreak;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }

    public override void SetHPVal(int oldval, int newval) {
        if (Md.mCfg.mType == MonsterType.Boss) {
            OnBossHPBarChangeEventArgs eventArgs = ReferencePool.Acquire<OnBossHPBarChangeEventArgs>();
            eventArgs.Fill(oldval, newval, Props.hp);
            GameEntry.Event.Fire(this, eventArgs);
        }
        else {
            base.SetHPVal(oldval, newval);
        }
    }
}
