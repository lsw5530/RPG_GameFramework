using GameFramework.Event;
class AddTipEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(AddTipEventArgs).GetHashCode(); 
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Tip { get; private set; }
    public AddTipEventArgs Fill(string tip) {
        this.Tip = tip;
        return this;
    }
    public override void Clear() {
        Tip = string.Empty;
    }
}
