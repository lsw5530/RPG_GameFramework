using GameFramework.Event;

class OnRefreshUIFormEventArgs : GameEventArgs {
    /// <summary>
    /// 收到UI刷新消息事件编号。
    /// </summary>
    public static readonly int EventId = typeof(OnRefreshUIFormEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public UIFormId formId { get; private set; }
    public OnRefreshUIFormEventArgs Fill(UIFormId formId) {
        this.formId = formId;
        return this;
    }

    public override void Clear() {
        formId = UIFormId.Undefined;
    }
}
