using GameFramework.Event;
/// <summary>
/// 加载界面更新进度事件
/// </summary>
public class LoadingFormUpdateProgressEventArgs : GameEventArgs {
    /// <summary>
    /// 加载界面进度更新事件编号
    /// </summary>
    public static readonly int EventId = typeof(LoadingFormUpdateProgressEventArgs).GetHashCode();

    /// <summary>
    /// 获取加载界面进度更新事件编号
    /// </summary>
    public override int Id => EventId;

    /// <summary>
    /// 获取加载进度描述内容
    /// </summary>
    public string Description {
        get;
        private set;
    }

    /// <summary>
    /// 获取加载进度
    /// </summary>
    public float Progress {
        get;
        private set;
    }

    /// <summary>
    /// 获取用户自定义数据
    /// </summary>
    public object UserData {
        get;
        private set;
    }

    /// <summary>
    /// 清理
    /// </summary>
    public override void Clear() {
        Description = default(string);
        Progress = default(float);
        UserData = default(object);
    }

    /// <summary>
    /// 填充加载界面更新事件
    /// </summary>
    public LoadingFormUpdateProgressEventArgs Fill(string description, float progress, object userData) {
        this.Description = description;
        this.Progress = progress;
        this.UserData = userData;

        return this;
    }
}
