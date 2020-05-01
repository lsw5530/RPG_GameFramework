using GameFramework.Event;

class RefreshChatUIEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(RefreshChatUIEventArgs).GetHashCode();

    public override int Id {
        get {
            return EventId;
        }
    }
    public string name { get; private set; }
    public string chat { get; private set; }
    public void Fill(string name,string chat) {
        this.name = name;
        this.chat = chat;
    }
    public override void Clear() {
        this.name = null;
        this.chat = null;
    }
}