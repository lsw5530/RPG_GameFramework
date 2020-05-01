using GameFramework.Event;
using GameMessage;

class OnBossHPBarChangeEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(OnBossHPBarChangeEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public int OldVal { get; private set; }
    public int NewVal { get; private set; }
    public int SumVal { get; private set; }
    public OnBossHPBarChangeEventArgs Fill(int oldVal,int newVal,int sunVal) {
        this.OldVal = oldVal;
        this.NewVal = newVal;
        this.SumVal = sunVal;
        return this;
    }
    public override void Clear() {
        this.OldVal = 0;
        this.NewVal = 0;
        this.SumVal = 0;
    }
} 