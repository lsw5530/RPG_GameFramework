/****************************************************
	文件：BaseData.cs
	
	
	
	功能：配置数据类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData : BaseData<MonsterData> {
    public int mWave;//批次
    public int mIndex;//序号
    public MonsterCfg mCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
    public int mLevel;
}

[System.Serializable]
public class MonsterCfg : BaseData<MonsterCfg> {
    public string mName;
    public int entityId;
    public MonsterType mType;//1:普通怪物，2：boss怪物
    public bool isStop;//怪物是否能被攻击中断当前的状态
    public string resPath;
    public int skillID;
    public float atkDis;
    public BattleProps bps;
}
/// <summary>
/// 技能移动参数
/// </summary>
[System.Serializable]
public class SkillMoveCfg : BaseData<SkillMoveCfg> {
    public int delayTime;
    public int moveTime;
    public float moveDis;
}

[System.Serializable]
public class SkillActionCfg : BaseData<SkillActionCfg> {
    public int delayTime;
    public float radius;//伤害计算范围
    public int angle;//伤害有效角度
}

[System.Serializable]
public class SkillCfg : BaseData<SkillCfg> {
    public string skillName;
    public int cdTime;
    public int skillTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public DamageType dmgType;
    public List<int> skillMoveLst;
    public List<int> skillActionLst;
    public List<int> skillDamageLst;
}

[System.Serializable]
public class StrongCfg : BaseData<StrongCfg> {
    public int pos;
    public int startlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

[System.Serializable]
public class AutoGuideCfg : BaseData<AutoGuideCfg> {
    public int npcID;//触发任务目标NPC索引号
    public string dilogArr;
    public int actID;
    public int coin;
    public int exp;
}
[System.Serializable]
public class MapCfg : BaseData<MapCfg> {
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterLst;

    public int coin;
    public int exp;
    public int crystal;
}

[System.Serializable]
public class TaskRewardCfg : BaseData<TaskRewardCfg> {
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}

[System.Serializable]
public class TaskRewardData : BaseData<TaskRewardData> {
    public int prgs;
    public bool taked;
}

[System.Serializable]
public class BaseData<T> {
    public int ID;
}

[System.Serializable]
public class BattleProps {
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}
