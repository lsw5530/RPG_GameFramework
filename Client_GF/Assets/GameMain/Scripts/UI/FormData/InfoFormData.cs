using GameFramework;
using UnityEngine;
public class InfoFormData 
{
    public GameFrameworkAction OnClickDown {
        get;
        set;
    }
    public GameFrameworkAction<float> OnDrag {
        get;
        set;
    }
    public GameFrameworkAction OnClickClose {
        get;
        set;
    }
}
