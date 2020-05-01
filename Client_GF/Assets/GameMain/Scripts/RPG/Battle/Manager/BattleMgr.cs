/****************************************************
	文件：BattleMgr.cs
	
	
	
	功能：战场管理器
*****************************************************/

using System;
using System.Collections.Generic;
using GameFramework;
using GameMessage;
using StarForce;
using UnityEngine;

public class BattleMgr {
    public bool TriggerCheck { get; set; } = true;
    public bool IsPauseGame { get; set; } = false;
    public double LastAtkTime { get; set; }
    /// <summary>
    /// 普攻ID
    /// </summary>
    public int ComboIndex { get; set; }

    private StateMgr m_StateMgr;
    private SkillMgr m_SkillMgr;
    private MapMgr m_MapMgr;
    public EntityPlayer EntitySelfPlayer { get; set; }
    private MapCfg m_MapCfg;
    private GameObject m_Player;
    /// <summary>
    /// 普攻连招
    /// </summary>
    private int[] m_ComboArr = new int[] { 111, 112, 113, 114, 115 };
    private Dictionary<string, EntityMonster> monsterDic = new Dictionary<string, EntityMonster>();
    public BattleMgr() {
        m_StateMgr = new StateMgr();
        m_SkillMgr = new SkillMgr();
    }
    public void Init(int mapid, Action cb = null) {
        TriggerCheck = true;
        IsPauseGame = false;
        //初始化各管理器
        m_StateMgr.Init();
        m_SkillMgr.Init();

        //加载战场地图
        m_MapCfg = GameEntry.Res.GetMapCfg(mapid);
        //初始化地图数据
        //GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
        m_MapMgr = GameObject.FindObjectOfType<MapMgr>();
        m_MapMgr.Init(this);
        monsterDic.Clear(); 

        Camera.main.transform.position = m_MapCfg.mainCamPos;
        Camera.main.transform.localEulerAngles = m_MapCfg.mainCamRote;

        LoadPlayer();

        
        //GameEntry.Sound.PlayUISound(Constants.BGHuangYe);
        if (cb != null) {
            cb();
        }
    }
    public void Update() {
        foreach (var item in monsterDic) {
            EntityMonster em = item.Value;
            em.TickAILogic();
        }
        //检测当前批次的怪物是否全部死亡
        if (m_MapMgr != null) {
            if (TriggerCheck && monsterDic.Count == 0) {
                bool isExist = m_MapMgr.SetNextTriggerOn();
                TriggerCheck = false;
                if (!isExist) {
                    //关卡结束，战斗胜利
                    EndBattle(true, EntitySelfPlayer.HP);
                }
            }
        }
    }
    /// <summary>
    /// 包括输赢两种情况
    /// </summary>
    /// <param name="isWin"></param>
    /// <param name="restHP"></param>
    public void EndBattle(bool isWin, int restHP) {
        IsPauseGame = true;
        GameEntry.Sound.StopMusic();
        EndBattleEventArgs eventArgs = ReferencePool.Acquire<EndBattleEventArgs>();
        eventArgs.Fill(isWin, restHP);
        GameEntry.Event.Fire(this, eventArgs);
    }
    private void LoadPlayer() {
        PlayerEntityData playerEntityData = new PlayerEntityData(GameEntry.Entity.GenerateSerialId(), PathDefine.AssissnBattlePlayerId, ActorType.Player) {
            Name = "BattlePlayer",
            Position = m_MapCfg.playerBornPos,
            Rotation = Quaternion.Euler(m_MapCfg.playerBornRote),
            LocalScale = Vector3.one
        };
        GameEntry.Entity.ShowMyPlayer(playerEntityData);
        PlayerData pd = PECommon.PlayerData;
        BattleProps props = new BattleProps {
            hp = pd.Hp,
            ad = pd.Ad,
            ap = pd.Ap,
            addef = pd.Addef,
            apdef = pd.Apdef,
            dodge = pd.Dodge,
            pierce = pd.Pierce,
            critical = pd.Critical
        };

        EntitySelfPlayer = new EntityPlayer {
            BattleMgr = this,
            StateMgr = m_StateMgr,
            SkillMgr = m_SkillMgr
        };
        EntitySelfPlayer.Name = "AssassinBattle";
        EntitySelfPlayer.SetBattleProps(props);
        EntitySelfPlayer.Idle();
    }
    public void OnPlayerLoaded(GameObject playerObj) {
        m_Player = playerObj;
        PlayerController playerCtrl = m_Player.GetComponent<PlayerController>();
        playerCtrl.Init();
        EntitySelfPlayer.SetCtrl(playerCtrl);
        //激活第一批次怪物
        ActiveCurrentBatchMonsters();
    }
    public void OnMonsterLoaded(GameObject m) {
        MonsterController controller = m.GetComponent<MonsterController>();
        MonsterData md = controller.MonsterEntityData.MD;//晚于OnShow执行
        EntityMonster em = new EntityMonster {
            BattleMgr = this,
            StateMgr = m_StateMgr,
            SkillMgr = m_SkillMgr
        };
        //设置初始属性
        em.Md = md;
        em.SetBattleProps(md.mCfg.bps);
        em.Name = "m" + md.mWave + "_" + md.mIndex;

        MonsterController mc = m.GetComponent<MonsterController>();
        mc.Init();
        em.SetCtrl(mc);

        mc.gameObject.SetActive(false);
        monsterDic.Add(m.name, em);
        if (md.mCfg.mType == MonsterType.Normal) {
            GameEntry.UI.AddHpItemInfo(m.name, mc.hpRoot, em.HP);
        }
        else if (md.mCfg.mType == MonsterType.Boss) {
            SetBossHPBarStateEventArgs stateEventArgs = ReferencePool.Acquire<SetBossHPBarStateEventArgs>();
            stateEventArgs.Fill(true);
            GameEntry.Event.Fire(this, stateEventArgs);
        }
    }
    public void LoadMonsterByWaveID(int wave) {
        for (int i = 0; i < m_MapCfg.monsterLst.Count; i++) {
            MonsterData md = m_MapCfg.monsterLst[i];
            if (md.mWave == wave) {
                MonsterEntityData monsterEntityData = new MonsterEntityData(GameEntry.Entity.GenerateSerialId(), md.mCfg.entityId, ActorType.Monster, md) {
                    Name = "m" + md.mWave + "_" + md.mIndex,
                    Position = md.mBornPos,
                    Rotation = Quaternion.Euler(md.mBornRote),
                    LocalScale = Vector3.one
                };
                GameEntry.Entity.ShowMonster(monsterEntityData);
            }
        }
    }
    public void ActiveCurrentBatchMonsters() {
        GameEntry.Timer.AddTimeTask((int tid) => {
            foreach (var item in monsterDic) {
                item.Value.SetActive(true);
                item.Value.Born();
                GameEntry.Timer.AddTimeTask((int id) => {
                    //出生1秒钟后进入Idle
                    item.Value.Idle();
                }, 1000);
            }
        }, 500);
    }

    public List<EntityMonster> GetEntityMonsters() {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        foreach (var item in monsterDic) {
            monsterLst.Add(item.Value);
        }
        return monsterLst;
    }

    public void RmvMonster(string key) {
        EntityMonster entityMonster;
        if (monsterDic.TryGetValue(key, out entityMonster)) {
            monsterDic.Remove(key);
            GameEntry.UI.RmvHpItemInfo(key);
        }
    }
    public Vector2 InputDir { get; private set; }
    #region 技能施放与角色控制
    public void SetSelfPlayerMoveDir(Vector2 dir) {
        //设置玩家移动
        InputDir = dir;
        if (EntitySelfPlayer.CanControl == false) {
            return;
        }

        if (EntitySelfPlayer.CurrentAniState == AniState.Idle || EntitySelfPlayer.CurrentAniState == AniState.Move) {
            if (dir == Vector2.zero) {
                EntitySelfPlayer.Idle();
            }
            else {
                EntitySelfPlayer.Move();
                EntitySelfPlayer.SetDir(dir);
            }
        }
    }
    public void ReqReleaseSkill(int index) {
        switch (index) {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    private void ReleaseNormalAtk() {
        if (EntitySelfPlayer.CurrentAniState == AniState.Attack) {//第二次及以上普通攻击
            //在500ms以内进行第二次点击，存数据
            double nowAtkTime = GameEntry.Timer.GetNowTime();
            if (nowAtkTime - LastAtkTime < Constants.ComboSpace && LastAtkTime != 0) {//满足时间间隔
                if (m_ComboArr[ComboIndex] != m_ComboArr[m_ComboArr.Length - 1]) {//连击未达到5次
                    ComboIndex += 1;
                    EntitySelfPlayer.ComboQue.Enqueue(m_ComboArr[ComboIndex]);
                    LastAtkTime = nowAtkTime;
                }
                else {//连击已满，重新计时/计数
                    LastAtkTime = 0;
                    ComboIndex = 0;
                }
            }
        }
        else if (EntitySelfPlayer.CurrentAniState == AniState.Idle || EntitySelfPlayer.CurrentAniState == AniState.Move) {//开始第一次普通攻击计时，计数
            ComboIndex = 0;
            LastAtkTime = GameEntry.Timer.GetNowTime();
            EntitySelfPlayer.Attack(m_ComboArr[ComboIndex]);
        }
    }
    private void ReleaseSkill1() {
        EntitySelfPlayer.Attack(101);
    }
    private void ReleaseSkill2() {
        EntitySelfPlayer.Attack(102);
    }
    private void ReleaseSkill3() {
        EntitySelfPlayer.Attack(103);
    }

    public bool CanRlsSkill() {
        if (EntitySelfPlayer == null)
            return false;
        return EntitySelfPlayer.CanRlsSkill;
    }

    #endregion
}
