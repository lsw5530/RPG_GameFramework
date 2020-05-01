using GameFramework.Event;
class SetHurtEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetHurtEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Name { get; private set; }
    public int Hurt { get; private set; }
    public SetHurtEventArgs Fill(string name, int hurt) {
        this.Name = name;
        this.Hurt = hurt;
        return this;
    }
    public override void Clear() {
        this.Name = string.Empty;
        Hurt = 0;
    }
}