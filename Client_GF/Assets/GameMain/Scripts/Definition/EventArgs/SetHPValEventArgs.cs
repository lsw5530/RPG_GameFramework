using GameFramework.Event;

class SetHPValEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetHPValEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Name { get; private set; }
    public int OldVal { get; private set; }
    public int NewVal { get; private set; }
    public SetHPValEventArgs Fill(string name, int oldVal,int newVal) {
        this.Name = name;
        this.OldVal = oldVal;
        this.NewVal = newVal;
        return this;
    }
    public override void Clear() {
        this.Name = string.Empty;
        this.OldVal = 0;
        this.NewVal = 0;
    }
}
