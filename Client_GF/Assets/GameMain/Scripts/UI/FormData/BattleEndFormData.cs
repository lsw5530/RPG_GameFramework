using GameFramework;
using UnityEngine;
public class BattleEndFormData 
{
    public GameFrameworkAction OnClickClose {
        get;
        set;
    }
    public GameFrameworkAction OnClickExit {
        get;
        set;
    }
    public GameFrameworkAction OnClickSure {
        get;
        set;
    }
    public FBEndType EndType { get; set; }
    public int Fbid { get; set; }
    public int CostTime { get; set; }
    public int RestHp { get; set; }
}
