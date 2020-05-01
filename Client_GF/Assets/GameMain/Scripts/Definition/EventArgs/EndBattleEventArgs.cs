using GameFramework.Event;

class EndBattleEventArgs : GameEventArgs {
    /// <summary>
    /// 收到结束战斗消息事件编号。
    /// </summary>
    public static readonly int EventId = typeof(EndBattleEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public bool IsWin { get; private set; }
    public int RestHP { get; private set; }
    public void Fill(bool isWin, int restHP) {
        this.IsWin = isWin;
        this.RestHP = restHP;
    }
    public override void Clear() {
        IsWin = false;
        RestHP = 0;
    }
}