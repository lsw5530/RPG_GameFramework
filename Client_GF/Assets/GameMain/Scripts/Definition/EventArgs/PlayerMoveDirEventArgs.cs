using GameFramework.Event;
using UnityEngine;
/// <summary>
/// 战斗时主角移动方向
/// </summary>
class PlayerMoveDirEventArgs : GameEventArgs {
    public static readonly int EventId = typeof(PlayerMoveDirEventArgs).GetHashCode();
    public override int Id {
        get {
            return EventId;
        }
    }
    public Vector2 MoveInput { get; private set; }
    public void Fill(Vector2 moveInput) {
        this.MoveInput = moveInput;
    }
    public override void Clear() {
        MoveInput = Vector2.zero;
    }
}