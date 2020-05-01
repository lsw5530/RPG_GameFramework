using GameFramework.Event;
using UnityEngine;

class AddHpItemInfoEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(AddHpItemInfoEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public string Name { get; private set; }
    public Transform Tf { get; private set; }
    public int Hp { get; private set; }
    public AddHpItemInfoEventArgs Fill(string name,Transform tf,int hp) {
        this.Name = name;
        this.Tf = tf;
        this.Hp = hp;
        return this;
    }
    public override void Clear() {
        this.Name = string.Empty;
        this.Tf = null;
        this.Hp = 0;
    }
}