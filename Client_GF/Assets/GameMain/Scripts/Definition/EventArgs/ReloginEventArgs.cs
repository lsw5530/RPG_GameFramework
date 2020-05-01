using GameFramework.Event;
class ReloginEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(ReloginEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }

    public override void Clear() {
        
    }
}