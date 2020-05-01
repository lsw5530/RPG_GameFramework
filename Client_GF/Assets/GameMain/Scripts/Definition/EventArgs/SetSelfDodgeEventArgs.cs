using GameFramework.Event;

class SetSelfDodgeEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetSelfDodgeEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }  
    public override void Clear() { 
    }
}
