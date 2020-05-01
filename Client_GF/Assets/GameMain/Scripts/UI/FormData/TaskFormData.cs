using GameFramework;
using GameMessage;
using UnityEngine;

public class TaskFormData 
{
    public GameFrameworkAction<int> OnClickTakeTask {
        get;
        set;
    }
    public PlayerData PlayerData { get; set; }
}
