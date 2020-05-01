/****************************************************
	文件：Controller.cs
	
	
	
	功能：表现实体控制器抽象基类
*****************************************************/

using System.Collections.Generic;
using GameFramework;
using UnityEngine;
public abstract class Controller : StarForce.Entity {
    private Animator m_Animator;
    private CharacterController m_PlayerCtroller;
    public Transform hpRoot;
    /// <summary>
    /// 是否移动取决于目标方向dir
    /// </summary>
    protected bool isMove = false;
    private Vector2 dir = Vector2.zero;
    /// <summary>
    /// 当为zero进入静止状态
    /// 技能释放/受伤/死亡/idle时
    /// </summary>
    public Vector2 Dir {
        get {
            return dir;
        }

        set {
            if (value == Vector2.zero) {
                isMove = false;
            }
            else {
                isMove = true;
            }
            dir = value;
        }
    }
    public Animator Animator { get => m_Animator; set => m_Animator = value; }
    public CharacterController PlayerCtrl { get => m_PlayerCtroller; set => m_PlayerCtroller = value; }

    protected Transform camTrans;

    protected bool skillMove = false;
    protected float skillMoveSpeed = 0f;

     
    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();

    public virtual void Init() {
        m_Animator = GetComponent<Animator>();
        m_PlayerCtroller = GetComponent<CharacterController>();
        m_PlayerCtroller.enabled = true; 
    }

    /// <summary>
    /// 设置静止/跑动动画混合
    /// </summary>
    /// <param name="blend">1为跑</param>
    public virtual void SetBlend(float blend) {
        m_Animator.SetFloat("Blend", blend);
    }
    /// <summary>
    /// 播放动作
    /// </summary>
    /// <param name="act"></param>
    public virtual void SetAction(int act) {
        m_Animator.SetInteger("Action", act);
    }

    public virtual void SetFX(string name, float destroy) {

    }
    /// <summary>
    /// 设置技能移动
    /// </summary>
    /// <param name="move"></param>
    /// <param name="skillSpeed"></param>
    public void SetSkillMoveState(bool move, float skillSpeed = 0f) {
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }
    /// <summary>
    /// 主角或怪物静止时设置攻击朝向
    /// </summary>
    /// <param name="atkDir"></param>
    public virtual void SetAtkRotationLocal(Vector2 atkDir) {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));//Vector2.Signed(v1,v2)，从v2转向v1，需要的水平旋转（y轴）角度
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    /// <summary>
    /// 主角移动时设置攻击朝向
    /// </summary>
    /// <param name="camDir">摇杆输入方向</param>
    public virtual void SetAtkRotationCam(Vector2 camDir) {
        float angle = Vector2.SignedAngle(camDir, new Vector2(0, 1)) + camTrans.eulerAngles.y;//水平旋转加上相机自身的水平旋转
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    protected GameObject FindGo(string objName) {
        GameObject target = null;
        Transform tf = transform.Find(objName);
        if (tf)
            target = tf.gameObject;
        
        return target;
    }
}
