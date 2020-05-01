/****************************************************
	文件：PathDefine.cs
	
	
	
	功能：路径常量宣言
*****************************************************/

using UnityEngine;
using System.Collections;

public class PathDefine {

    #region Configs
    public const string RDNameCfg = "rdname";
    public const string MapCfg = "map";
    public const string MonsterCfg = "monster";

    public const string GuideCfg = "guide";
    public const string StrongCfg = "strong";
    public const string TaskRewardCfg = "taskreward";

    public const string SkillCfg = "skill";
    public const string SkillMoveCfg = "skillmove";
    public const string SkillActionCfg = "skillaction";


    #endregion

    #region Strong
    public const string ItemArrorBG = "btnstrong";
    public const string ItemPlatBG = "charbg3";

    public const string ItemToukui = "toukui";
    public const string ItemBody = "body";
    public const string ItemYaobu = "yaobu";
    public const string ItemHand = "hand";
    public const string ItemLeg = "leg";
    public const string ItemFoot = "foot";

    public const string SpStar1 = "star1";
    public const string SpStar2 = "star2";


    #endregion

    #region TaskReward
    public const string TaskItemPrefab = "ItemTask";
    #endregion


    #region AutoGuide
    public const string TaskHead = "task";
    public const string WiseManHead = "wiseman";
    public const string GeneralHead = "general";
    public const string ArtisanHead = "artisan";
    public const string TraderHead = "trader";

    public const string SelfIcon = "assassin";
    public const string GuideIcon = "npcguide";
    public const string WiseManIcon = "npc0";
    public const string GeneralIcon = "npc1";
    public const string ArtisanIcon = "npc2";
    public const string TraderIcon = "npc3";


    #endregion

    #region Player 
    public const int AssissnCityPlayerId =10000; 
    public const int AssissnBattlePlayerId = 10001;

    public const string HPItemPrefab = "ItemEntityHp";

    #endregion
}
