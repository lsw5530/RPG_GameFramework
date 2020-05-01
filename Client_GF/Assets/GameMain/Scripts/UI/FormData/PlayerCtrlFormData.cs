using GameFramework;
using UnityEngine;
public class PlayerCtrlFormData
{
    /// <summary>
    /// 释放普攻/技能
    /// </summary>
    public GameFrameworkAction<int> OnClickSkillAtk {
        get;
        set;
    }
    public GameFrameworkAction OnClickHead {
        get;
        set;
    }
    /// <summary>
    /// 查询是否可以释放技能
    /// 当受伤/正在释放技能时，为false
    /// </summary>
    public GameFrameworkFunc<bool> OnClickCanRls {
        get;
        set;
    }
    public GameFrameworkAction<Vector2> OnPlayerMove { get; set; }
}
