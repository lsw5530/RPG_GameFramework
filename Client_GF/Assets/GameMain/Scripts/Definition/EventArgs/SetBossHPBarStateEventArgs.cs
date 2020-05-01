
using GameFramework.Event;

class SetBossHPBarStateEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(SetBossHPBarStateEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public bool State { get;private set; }
    public SetBossHPBarStateEventArgs Fill(bool state) {
        this.State = state;
        return this;
    }
    public override void Clear() {
        State = false;
    }
}
 