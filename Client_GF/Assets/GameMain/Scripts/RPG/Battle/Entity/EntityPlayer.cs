/****************************************************
	文件：EntityPlayer.cs
	
	
	
	功能：玩家逻辑实体
*****************************************************/

using UnityEngine;
using System.Collections.Generic;
using GameFramework.Event;
using System;
using GameFramework;

public class EntityPlayer : EntityBase {
    private Vector2 m_InputDir;
    public EntityPlayer() {
        entityType = EntityType.Player;
        GameEntry.Event.Subscribe(PlayerMoveDirEventArgs.EventId, OnDirInputChange);
    }
    void Destroy() {
        GameEntry.Event.Unsubscribe(PlayerMoveDirEventArgs.EventId, OnDirInputChange);
    }
    private void OnDirInputChange(object sender, GameEventArgs e) {
        PlayerMoveDirEventArgs ne = e as PlayerMoveDirEventArgs;
        m_InputDir = ne.MoveInput;
        Debug.LogError("m_InputDir:" + m_InputDir);
        Debug.LogError("entityState:" + entityState);
    }

    public override Vector2 GetDirInput() {
        Vector2 dir = BattleMgr.InputDir;
        return dir;
    }
    /// <summary>
    /// 查找最近的怪物，计算攻击朝向
    /// </summary>
    /// <returns></returns>
    public override Vector2 CalcTargetDir() {
        EntityMonster monster = FindClosedTarget();
        if (monster != null) {
            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else {
            return Vector2.zero;
        }
    }

    private EntityMonster FindClosedTarget() {
        List<EntityMonster> lst = BattleMgr.GetEntityMonsters();
        if (lst == null || lst.Count == 0) {
            return null;
        }

        Vector3 self = GetPos();
        EntityMonster targetMonster = null;
        float dis = 0;
        for (int i = 0; i < lst.Count; i++) {
            Vector3 target = lst[i].GetPos();
            if (i == 0) {
                dis = Vector3.Distance(self, target);
                targetMonster = lst[0];
            }
            else {
                float calcDis = Vector3.Distance(self, target);
                if (dis > calcDis) {
                    dis = calcDis;
                    targetMonster = lst[i];
                }
            }
        }
        return targetMonster;
    }

    public override void SetHPVal(int oldval, int newval) {
        RefreshSelfHpEventArgs eventArgs = ReferencePool.Acquire<RefreshSelfHpEventArgs>();
        eventArgs.Fill(newval);
        GameEntry.Event.Fire(this,eventArgs); 
    }

    public override void SetDodge() {
        GameEntry.UI.SetSelfDodge();
    }
}
