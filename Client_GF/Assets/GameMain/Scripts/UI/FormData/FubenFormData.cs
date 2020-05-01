
using GameFramework;
using GameMessage;

public class FubenFormData
{
    public GameFrameworkAction OnClickClose {
        get;
        set;
    }
    public GameFrameworkAction<int> OnClickTask {
        get;
        set;
    }
    public PlayerData PlayerData { get; set; }
}
