
using GameFramework.Event;

class SetDodgeEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetDodgeEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Name { get; private set; }
    public SetDodgeEventArgs Fill(string name) {
        this.Name = name;
        return this;
    }
    public override void Clear() {
        Name = string.Empty;
    }
} 