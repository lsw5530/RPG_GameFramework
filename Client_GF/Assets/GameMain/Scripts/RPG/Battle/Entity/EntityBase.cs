/****************************************************
	文件：EntityBase.cs
	
	
	
	功能：逻辑实体基类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase {
    public AniState CurrentAniState { get; set; }
    public BattleMgr BattleMgr { get; set; }
    public StateMgr StateMgr { get; set; }
    public SkillMgr SkillMgr { get; set; }
    /// <summary>
    /// 技能播放中，值为false
    /// </summary>
    public bool CanControl { get; set; } = true;
    public bool CanRlsSkill { get; set; } = true;
    public Queue<int> ComboQue { get; set; } = new Queue<int>();
    public int NextSkillID { get; set; }
    public SkillCfg CurtSkillCfg { get; set; }

    //技能位移的回调ID
    public List<int> SkMoveCBLst { get; set; } = new List<int>();
    //技能伤害计算回调ID
    public List<int> SkActionCBLst { get; set; } = new List<int>();

    public int SkEndCB { get; set; } = -1;

    public EntityType entityType = EntityType.None;

    public EntityState entityState = EntityState.None;
    public string Name { get; set; }

    public BattleProps Props { get; set; }

    private Controller m_Controller { get; set; }
    private int hp;
    public int HP {
        get {
            return hp;
        }
        set {
            //通知UI层TODO
            PECommon.Log(Name + ": HPchange:" + hp + " to " + value);
            SetHPVal(hp, value);
            hp = value;
        }
    }
    public void Born() {
        StateMgr.ChangeStatus(this, AniState.Born, null);
    }
    public void Move() {
        StateMgr.ChangeStatus(this, AniState.Move, null);
    }
    public void Idle() {
        StateMgr.ChangeStatus(this, AniState.Idle, null);
    }
    public void Attack(int skillID) {
        StateMgr.ChangeStatus(this, AniState.Attack, skillID);
    }
    public void Hit() {
        StateMgr.ChangeStatus(this, AniState.Hit, null);
    }
    public void Die() {
        StateMgr.ChangeStatus(this, AniState.Die, null);
    }

    public virtual void TickAILogic() { }

    public void SetCtrl(Controller ctrl) {
        m_Controller = ctrl;
    }
    public void SetActive(bool active) {
        if (m_Controller != null) {
            if (!active) {
                m_Controller.HideSelf();
            }
            else {
                m_Controller.gameObject.SetActive(true);
            }
        }
    }
    public virtual void SetBattleProps(BattleProps props) {
        HP = props.hp;
        Props = props;
    }
    /// <summary>
    /// 设置静止/跑动动画混合
    /// </summary>
    /// <param name="blend">1为跑</param>
    public virtual void SetBlend(float blend) {
        if (m_Controller != null) {
            m_Controller.SetBlend(blend);
        }
    }
    /// <summary>
    /// 设置角色朝向
    /// 当为zero，停止移动
    /// </summary>
    /// <param name="dir"></param>
    public virtual void SetDir(Vector2 dir) {
        if (m_Controller != null) {
            m_Controller.Dir = dir;
        }
    }
    /// <summary>
    /// 播放动作
    /// </summary>
    /// <param name="act"></param>
    public virtual void SetAction(int act) {
        if (m_Controller != null) {
            m_Controller.SetAction(act);
        }
    }
    public virtual void SetFX(string name, float destroy) {
        if (m_Controller != null) {
            m_Controller.SetFX(name, destroy);
        }
    }
    public virtual void SetSkillMoveState(bool move, float speed = 0f) {
        if (m_Controller != null) {
            m_Controller.SetSkillMoveState(move, speed);
        }
    }
    /// <summary>
    /// 设置攻击角度
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="offset">主角是否处于移动状态</param>
    public virtual void SetAtkRotation(Vector2 dir, bool offset) {
        if (m_Controller != null) {
            if (offset) {//主角移动时设置攻击朝向
                m_Controller.SetAtkRotationCam(dir);
            }
            else {//主角或怪物静止时设置攻击朝向
                m_Controller.SetAtkRotationLocal(dir);
            }
        }
    }

    #region 战斗信息显示
    public virtual void SetDodge() {
        if (m_Controller != null) {
            GameEntry.UI.SetDodge(Name);
        }
    }
    public virtual void SetCritical(int critical) {
        if (m_Controller != null) {
            GameEntry.UI.SetCritical(Name, critical);
        }
    }
    public virtual void SetHurt(int hurt) {
        if (m_Controller != null) {
            GameEntry.UI.SetHurt(Name, hurt);
        }
    }
    public virtual void SetHPVal(int oldval, int newval) {
        if (m_Controller != null) {
            GameEntry.UI.SetHPVal(Name, oldval, newval);
        }
    }
    #endregion

    public virtual void SkillAttack(int skillID) {
        SkillMgr.SkillAttack(this, skillID);
    }

    public virtual Vector2 GetDirInput() {
        return Vector2.zero;
    }

    public virtual Vector3 GetPos() {
        return m_Controller.transform.position;
    }

    public virtual Transform GetTrans() {
        return m_Controller.transform;
    }

    public AnimationClip[] GetAniClips() {
        if (m_Controller != null) {
            return m_Controller.Animator.runtimeAnimatorController.animationClips;
        }
        return null;
    }

    public AudioSource GetAudio() {
        return m_Controller.GetComponent<AudioSource>();
    }
    public CharacterController GetCC() {
        return m_Controller.GetComponent<CharacterController>();
    }

    public virtual bool GetBreakState() {
        return true;
    }

    public virtual Vector2 CalcTargetDir() {
        return Vector2.zero;
    }
    /// <summary>
    /// 技能播放完时调用
    /// </summary>
    public void ExitCurtSkill() {
        CanControl = true;

        if (CurtSkillCfg != null) {
            if (!CurtSkillCfg.isBreak) {
                entityState = EntityState.None;
            }
            //连招数据更新
            if (CurtSkillCfg.isCombo) {
                if (ComboQue.Count > 0) {
                    NextSkillID = ComboQue.Dequeue();
                }
                else {
                    NextSkillID = 0;
                }
            }
            CurtSkillCfg = null;
        }
        SetAction(Constants.ActionDefault);
    }
    /// <summary>
    /// 技能开始释放时，移除回调编号
    /// </summary>
    /// <param name="tid">回调编号</param>
    public void RmvActionCB(int tid) {
        int index = -1;
        for (int i = 0; i < SkActionCBLst.Count; i++) {
            if (SkActionCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            SkActionCBLst.RemoveAt(index);
        }
    }
    /// <summary>
    /// 移动回调正常执行完毕后，移除回调编号
    /// </summary>
    /// <param name="tid">回调编号</param>
    public void RmvMoveCB(int tid) {
        int index = -1;
        for (int i = 0; i < SkMoveCBLst.Count; i++) {
            if (SkMoveCBLst[i] == tid) {
                index = i;
                break;
            }
        }
        if (index != -1) {
            SkMoveCBLst.RemoveAt(index);
        }
    }
    /// <summary>
    /// 在死亡或受伤时，取消技能回调（例如，移动/停止，延时技能，连招）
    /// 当前正在播放的技能不受影响
    /// </summary>
    public void RmvSkillCB() {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);

        for (int i = 0; i < SkMoveCBLst.Count; i++) {
            int tid = SkMoveCBLst[i];
            GameEntry.Timer.DelTask(tid);
        }

        for (int i = 0; i < SkActionCBLst.Count; i++) {
            int tid = SkActionCBLst[i];
            GameEntry.Timer.DelTask(tid);
        }

        //攻击被中断，删除定时回调
        if (SkEndCB != -1) {
            GameEntry.Timer.DelTask(SkEndCB);
            SkEndCB = -1;
        }
        SkMoveCBLst.Clear();
        SkActionCBLst.Clear();


        //清空连招
        if (NextSkillID != 0 || ComboQue.Count > 0) {
            NextSkillID = 0;
            ComboQue.Clear();

            BattleMgr.LastAtkTime = 0;
            BattleMgr.ComboIndex = 0;
        }
    }
}
