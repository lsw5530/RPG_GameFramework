using GameFramework.Event;

class RmvHpItemInfoEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(RmvHpItemInfoEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Key { get; private set; }
    public int Hp { get; private set; }
    public RmvHpItemInfoEventArgs Fill(string key) {
        this.Key = key;
        return this;
    }
    public override void Clear() {
        this.Key = string.Empty; 
    }
}