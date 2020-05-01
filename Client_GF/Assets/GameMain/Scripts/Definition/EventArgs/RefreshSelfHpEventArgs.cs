using GameFramework.Event;
class RefreshSelfHpEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(RefreshSelfHpEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public int NewVal { get; private set; }
    public void Fill(int newVal) {
        this.NewVal = newVal;
    }
    public override void Clear() {
        NewVal = 0;
    }
}
