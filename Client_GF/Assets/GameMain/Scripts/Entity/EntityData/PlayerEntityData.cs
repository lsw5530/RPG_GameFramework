using System;
using UnityEngine;

[Serializable]
public class PlayerEntityData : RoleEntityData {
    public PlayerEntityData(int entityId, int typeId, ActorType actorType) : base(entityId, typeId, actorType) {
        
    } 
}
