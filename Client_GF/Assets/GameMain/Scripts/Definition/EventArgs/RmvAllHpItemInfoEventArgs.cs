
using GameFramework.Event;

class RmvAllHpItemInfoEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(RmvAllHpItemInfoEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public override void Clear() {
    }
}

