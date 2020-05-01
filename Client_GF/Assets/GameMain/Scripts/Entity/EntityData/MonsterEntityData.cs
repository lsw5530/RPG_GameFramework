using StarForce;
using UnityEngine;
[System.Serializable]
public class MonsterEntityData : EntityData {
    [SerializeField]
    public ActorType ActorType { get; }
    [SerializeField]
    public MonsterData MD { get; }
    public MonsterEntityData(int entityId, int typeId, ActorType actorType,MonsterData md) : base(entityId, typeId) {
        ActorType = actorType;
        MD = md;
    }

}
