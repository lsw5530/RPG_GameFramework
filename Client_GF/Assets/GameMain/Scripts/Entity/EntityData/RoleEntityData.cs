using StarForce;
using UnityEngine;
[System.Serializable]
public class RoleEntityData : EntityData {
    [SerializeField]
    public ActorType ActorType { get; }
    public RoleEntityData(int entityId, int typeId, ActorType actorType) : base(entityId, typeId) {
        ActorType = actorType;
    }

}
