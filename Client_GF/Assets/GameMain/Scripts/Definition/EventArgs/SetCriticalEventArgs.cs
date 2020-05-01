using GameFramework.Event;
class SetCriticalEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetCriticalEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Name { get; private set; }
    public int Critical { get; private set; }
    public SetCriticalEventArgs Fill(string name, int critical) {
        this.Name = name;
        this.Critical = critical;
        return this;
    }
    public override void Clear() {
        Name = string.Empty;
    }
}