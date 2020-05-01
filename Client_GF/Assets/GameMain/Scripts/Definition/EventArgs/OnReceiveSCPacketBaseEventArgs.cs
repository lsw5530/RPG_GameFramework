using GameFramework.Event;
using GameMessage;

class OnReceiveSCPacketBaseEventArgs : GameEventArgs {
    /// <summary>
    /// 收到服务器消息事件编号。
    /// </summary>
    public static readonly int EventId = typeof(OnReceiveSCPacketBaseEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public CMD msgID { get; private set; }
    public SCPacketBase scPacketBase { get; private set; }
    public override void Clear() {
        msgID = CMD.None;
    }
    public OnReceiveSCPacketBaseEventArgs Fill(CMD msgID, SCPacketBase scPacketBase) {
        this.msgID = msgID;
        this.scPacketBase = scPacketBase;
        return this;
    }
}
