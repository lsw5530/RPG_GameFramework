using GameFramework;
using UnityEngine;

public class MainCityFormData 
{ 
    /// <summary>
    /// strong按钮回调。
    /// </summary>
    public GameFrameworkAction OnClickStrong {
        get;
        set;
    }
    /// <summary>
    /// 副本按钮回调。
    /// </summary>
    public GameFrameworkAction OnClickFuBen {
        get;
        set;
    }
    /// <summary>
    /// 任务按钮回调。
    /// </summary>
    public GameFrameworkAction OnClickTask {
        get;
        set;
    }
    /// <summary>
    /// 购买power按钮回调。
    /// </summary>
    public GameFrameworkAction<int> OnClickBuy {
        get;
        set;
    } 
    /// <summary>
    /// guide按钮回调。
    /// </summary>
    public GameFrameworkAction<AutoGuideCfg>  OnClickGuide {
        get;
        set;
    }
    /// <summary>
    /// head按钮回调。
    /// </summary>
    public GameFrameworkAction OnClickHead {
        get;
        set;
    }
    /// <summary>
    /// 聊天按钮回调。
    /// </summary>
    public GameFrameworkAction  OnClickChat {
        get;
        set;
    }
    /// <summary>
    /// 摇杆回调。
    /// </summary>
    public GameFrameworkAction<Vector2> OnMoveDir {
        get;
        set;
    }
}
