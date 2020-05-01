/****************************************************
    文件：ResSvc.cs
	
    
    
	功能：资源加载服务
*****************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using GameFramework;
using GameFramework.Resource;
using StarForce;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
[DisallowMultipleComponent]
[AddComponentMenu("Game Framework/Res")]
public class ResComponent : GameFrameworkComponent, ICustomComponent {
    LoadAssetCallbacks LoadRDNameCfgCallbacks;
    LoadAssetCallbacks LoadMonsterCfgCallbacks;
    LoadAssetCallbacks LoadMapCfgCallbacks;
    LoadAssetCallbacks LoadGuideCfgCallbacks;
    LoadAssetCallbacks LoadStrongCfgCallbacks;
    LoadAssetCallbacks LoadTaskRewardCfgCallbacks;
    LoadAssetCallbacks LoadSkillCfgCallbacks;
    LoadAssetCallbacks LoadSkillMoveCfgCallbacks;
    LoadAssetCallbacks LoadSkillActionCfgCallbacks;
    private LoadBytesCallbacks m_LoadBytesCallbacks;

    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    private LoadAssetCallbacks m_LoadAssetsCallbacks;
    public void InitSvc() {
        string assetName = AssetUtility.GetResCfgsAsset(PathDefine.RDNameCfg);
        LoadRDNameCfgCallbacks = new LoadAssetCallbacks(InitRDNameCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadRDNameCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.MonsterCfg);
        LoadMonsterCfgCallbacks = new LoadAssetCallbacks(InitMonsterCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadMonsterCfgCallbacks);


        assetName = AssetUtility.GetResCfgsAsset(PathDefine.MapCfg);
        LoadMapCfgCallbacks = new LoadAssetCallbacks(InitMapCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadMapCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.GuideCfg);
        LoadGuideCfgCallbacks = new LoadAssetCallbacks(InitGuideCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadGuideCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.StrongCfg);
        LoadStrongCfgCallbacks = new LoadAssetCallbacks(InitStrongCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadStrongCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.TaskRewardCfg);
        LoadTaskRewardCfgCallbacks = new LoadAssetCallbacks(InitTaskRewardCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadTaskRewardCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.SkillCfg);
        LoadSkillCfgCallbacks = new LoadAssetCallbacks(InitSkillCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadSkillCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.SkillMoveCfg);
        LoadSkillMoveCfgCallbacks = new LoadAssetCallbacks(InitSkillMoveCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadSkillMoveCfgCallbacks);

        assetName = AssetUtility.GetResCfgsAsset(PathDefine.SkillActionCfg);
        LoadSkillActionCfgCallbacks = new LoadAssetCallbacks(InitSkillActionCfg);
        GameEntry.Resource.LoadAsset(assetName, LoadSkillActionCfgCallbacks);

        if (!GameEntry.Base.EditorResourceMode)
            LoadLuaAb();
        PECommon.Log("Init ResSvc...");
    }
    private AssetBundle m_LuaAb;
    private AssetBundle m_XmlAb;
    private void LoadXMLABFromPool(string abName) {
        if (GameEntry.Base.EditorResourceMode) return;
        AssetBundle ab = GameEntry.Resource.GetABFromPool(abName);
        if (ab != null) {
            m_XmlAb = ab;
            string str = LoadXmlSync("guide");
        }
        else {
            //Debug.LogError("加载失败：" + abName);
        }
    }
    private string LoadXmlSync(string fileName) {
        TextAsset data = m_XmlAb.LoadAsset(fileName) as TextAsset;
        string str = data.text;
        return str;
    }
    private void LoadLuaAb() {
        m_LoadBytesCallbacks = new LoadBytesCallbacks(OnBytesLoadSuccess, OnBytesLoadFailure);
        GameEntry.Resource.LoadBytes("HotFix", m_LoadBytesCallbacks, null);
    }

    private void OnBytesLoadFailure(string fileUri, string errorMessage, object userData) {
        Debug.LogErrorFormat("Bytes {0} File Load Failed：" + errorMessage, fileUri);
    }

    private void OnBytesLoadSuccess(string fileUri, byte[] bytes, float duration, object userData) {
        AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
        m_LuaAb = ab;
        Debug.LogFormat("OnBytesLoaded,fileUri:"+ fileUri);
    }
    public string LoadAssetSync(string fileName) {
        string str = null;
        if (GameEntry.Base.EditorResourceMode) {
            string assetName = AssetUtility.GetLuaAsset(fileName);
            string path = Application.dataPath.Replace("Assets", "") + assetName;
            if (!File.Exists(path)) {
                Debug.LogError("Lua {0} File Load Failed：" + path);
                return null;
            }
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                str = sr.ReadToEnd();
            }
        }
        else {
            TextAsset data = m_LuaAb.LoadAsset(fileName) as TextAsset;
            if (data != null) {
                str = data.text;
            }
            else {
                Debug.LogError("Lua {0} File Load Failed：" + fileName);
            }
        }
        return str;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            //LoadAssetSync("guide");
        }
    }
    public void Close() {

    }
    public void ResetSkillCfgs() {
    }

    //internal void LoadTextAsset(string path, LoadAssetSuccessCallback sucessCallBack, LoadAssetFailureCallback failCallBack, object userdata = null) {
    //    string assetName = AssetUtility.GetLuaAsset(path);
    //    m_LoadAssetsCallbacks = new LoadAssetCallbacks(sucessCallBack, failCallBack);
    //    GameEntry.Resource.LoadAsset(assetName, typeof(TextAsset), m_LoadAssetsCallbacks, userdata);
    //}


    private Dictionary<string, GameObject> goDic = new Dictionary<string, GameObject>();
    public void LoadPrefab(string path, LoadAssetSuccessCallback sucessCallBack, LoadAssetFailureCallback failCallBack, object userdata) {
        string assetName = AssetUtility.GeUIPrefabAsset(path);
        m_LoadAssetsCallbacks = new LoadAssetCallbacks(sucessCallBack, failCallBack);
        GameEntry.Resource.LoadAsset(assetName, typeof(GameObject), m_LoadAssetsCallbacks, userdata);
    }
    public void LoadSprite(string path, LoadAssetSuccessCallback sucessCallBack, LoadAssetFailureCallback failCallBack, Image image) {
        string assetName = AssetUtility.GeSpritAsset(path);
        m_LoadAssetsCallbacks = new LoadAssetCallbacks(sucessCallBack, failCallBack);
        GameEntry.Resource.LoadAsset(assetName, typeof(Sprite), m_LoadAssetsCallbacks, image);
    }
    public Sprite LoadSprite11(string path, bool cache = false) {
        Sprite sp = null;
        if (!spDic.TryGetValue(path, out sp)) {
            sp = Resources.Load<Sprite>(path);
            if (cache) {
                spDic.Add(path, sp);
            }
        }
        return sp;
    }
    #region InitCfgs
    #region 随机名字
    private List<string> surnameLst = new List<string>();
    private List<string> manLst = new List<string>();
    private List<string> womanLst = new List<string>();
    private void InitRDNameCfg(string assetName, object asset, float duration, object userData) {
        LoadXMLABFromPool("test/xxmlcfgs");
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                //int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "surname":
                            surnameLst.Add(e.InnerText);
                            break;
                        case "man":
                            manLst.Add(e.InnerText);
                            break;
                        case "woman":
                            womanLst.Add(e.InnerText);
                            break;
                    }
                }

            }
            GameEntry.Resource.UnloadAsset(asset);
        }

    }

    public string GetRDNameData(bool man = true) {
        string rdName = surnameLst[PETools.RDInt(0, surnameLst.Count - 1)];
        if (man) {
            rdName += manLst[PETools.RDInt(0, manLst.Count - 1)];
        }
        else {
            rdName += womanLst[PETools.RDInt(0, womanLst.Count - 1)];
        }

        return rdName;
    }
    #endregion

    #region 地图
    private Dictionary<int, MapCfg> mapCfgDataDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg {
                    ID = ID,
                    monsterLst = new List<MonsterData>()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "mapName":
                            mc.mapName = e.InnerText;
                            break;
                        case "sceneName":
                            mc.sceneName = e.InnerText;
                            break;
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "mainCamPos": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "mainCamRote": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornPos": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "playerBornRote": {
                                string[] valArr = e.InnerText.Split(',');
                                mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                            }
                            break;
                        case "monsterLst": {
                                string[] valArr = e.InnerText.Split('#');
                                for (int waveIndex = 0; waveIndex < valArr.Length; waveIndex++) {
                                    if (waveIndex == 0) {
                                        continue;
                                    }
                                    string[] tempArr = valArr[waveIndex].Split('|');
                                    for (int j = 0; j < tempArr.Length; j++) {
                                        if (j == 0) {
                                            continue;
                                        }
                                        string[] arr = tempArr[j].Split(',');
                                        MonsterData md = new MonsterData {
                                            ID = int.Parse(arr[0]),
                                            mWave = waveIndex,
                                            mIndex = j,
                                            mCfg = GetMonsterCfg(int.Parse(arr[0])),
                                            mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                            mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                            mLevel = int.Parse(arr[5])
                                        };
                                        mc.monsterLst.Add(md);
                                    }
                                }
                            }
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;
                    }
                }
                mapCfgDataDic.Add(ID, mc);
            }
            GameEntry.Resource.UnloadAsset(asset);
        }
    }
    public MapCfg GetMapCfg(int id) {
        MapCfg data;
        if (mapCfgDataDic.TryGetValue(id, out data)) {
            return data;
        }
        Debug.LogError("GetMapCfg is null:" + id);
        return null;
    }
    #endregion

    #region 怪物
    private Dictionary<int, MonsterCfg> monsterCfgDataDic = new Dictionary<int, MonsterCfg>();
    private void InitMonsterCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MonsterCfg mc = new MonsterCfg {
                    ID = ID,
                    bps = new BattleProps()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "mName":
                            mc.mName = e.InnerText;
                            break;
                        case "entityId":
                            mc.entityId = int.Parse(e.InnerText);
                            break;
                        case "mType":
                            if (e.InnerText.Equals("1")) {
                                mc.mType = MonsterType.Normal;
                            }
                            else if (e.InnerText.Equals("2")) {
                                mc.mType = MonsterType.Boss;
                            }
                            break;
                        case "isStop":
                            mc.isStop = int.Parse(e.InnerText) == 1;
                            break;
                        case "resPath":
                            mc.resPath = e.InnerText;
                            break;
                        case "skillID":
                            mc.skillID = int.Parse(e.InnerText);
                            break;
                        case "atkDis":
                            mc.atkDis = float.Parse(e.InnerText);
                            break;
                        case "hp":
                            mc.bps.hp = int.Parse(e.InnerText);
                            break;
                        case "ad":
                            mc.bps.ad = int.Parse(e.InnerText);
                            break;
                        case "ap":
                            mc.bps.ap = int.Parse(e.InnerText);
                            break;
                        case "addef":
                            mc.bps.addef = int.Parse(e.InnerText);
                            break;
                        case "apdef":
                            mc.bps.apdef = int.Parse(e.InnerText);
                            break;
                        case "dodge":
                            mc.bps.dodge = int.Parse(e.InnerText);
                            break;
                        case "pierce":
                            mc.bps.pierce = int.Parse(e.InnerText);
                            break;
                        case "critical":
                            mc.bps.critical = int.Parse(e.InnerText);
                            break;
                    }
                }
                monsterCfgDataDic.Add(ID, mc);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public MonsterCfg GetMonsterCfg(int id) {
        MonsterCfg data;
        if (monsterCfgDataDic.TryGetValue(id, out data)) {
            return data;
        }
        return null;
    }
    #endregion

    #region 自动引导配置
    private Dictionary<int, AutoGuideCfg> guideTaskDic = new Dictionary<int, AutoGuideCfg>();
    private void InitGuideCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                AutoGuideCfg mc = new AutoGuideCfg {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "npcID":
                            mc.npcID = int.Parse(e.InnerText);
                            break;
                        case "dilogArr":
                            mc.dilogArr = e.InnerText;
                            break;
                        case "actID":
                            mc.actID = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                guideTaskDic.Add(ID, mc);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public AutoGuideCfg GetAutoGuideCfg(int id) {
        AutoGuideCfg agc = null;
        if (guideTaskDic.TryGetValue(id, out agc)) {
            return agc;
        }
        return null;
    }
    #endregion

    #region 强化升级配置
    private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
    private void InitStrongCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sd = new StrongCfg {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name) {
                        case "pos":
                            sd.pos = val;
                            break;
                        case "starlv":
                            sd.startlv = val;
                            break;
                        case "addhp":
                            sd.addhp = val;
                            break;
                        case "addhurt":
                            sd.addhurt = val;
                            break;
                        case "adddef":
                            sd.adddef = val;
                            break;
                        case "minlv":
                            sd.minlv = val;
                            break;
                        case "coin":
                            sd.coin = val;
                            break;
                        case "crystal":
                            sd.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongDic.TryGetValue(sd.pos, out dic)) {
                    dic.Add(sd.startlv, sd);
                }
                else {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sd.startlv, sd);

                    strongDic.Add(sd.pos, dic);
                }
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public StrongCfg GetStrongCfg(int pos, int starlv) {
        StrongCfg sd = null;
        Dictionary<int, StrongCfg> dic = null;
        if (strongDic.TryGetValue(pos, out dic)) {
            if (dic.ContainsKey(starlv)) {
                sd = dic[starlv];
            }
        }
        return sd;
    }

    public int GetPropAddValPreLv(int pos, int starlv, int type) {
        Dictionary<int, StrongCfg> posDic = null;
        int val = 0;
        if (strongDic.TryGetValue(pos, out posDic)) {
            for (int i = 0; i < starlv; i++) {
                StrongCfg sd;
                if (posDic.TryGetValue(i, out sd)) {
                    switch (type) {
                        case 1://hp
                            val += sd.addhp;
                            break;
                        case 2://hurt
                            val += sd.addhurt;
                            break;
                        case 3://def
                            val += sd.adddef;
                            break;
                    }
                }
            }
        }
        return val;
    }
    #endregion


    #region 自动引导配置
    private Dictionary<int, TaskRewardCfg> taskRewareDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskRewardCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskRewardCfg trc = new TaskRewardCfg {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "taskName":
                            trc.taskName = e.InnerText;
                            break;
                        case "count":
                            trc.count = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            trc.exp = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            trc.coin = int.Parse(e.InnerText);
                            break;
                    }
                }
                taskRewareDic.Add(ID, trc);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public TaskRewardCfg GetTaskRewardCfg(int id) {
        TaskRewardCfg trc = null;
        if (taskRewareDic.TryGetValue(id, out trc)) {
            return trc;
        }
        return null;
    }
    #endregion

    #region 技能配置
    private Dictionary<int, SkillCfg> skillDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillCfg sc = new SkillCfg {
                    ID = ID,
                    skillMoveLst = new List<int>(),
                    skillActionLst = new List<int>(),
                    skillDamageLst = new List<int>()
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "skillName":
                            sc.skillName = e.InnerText;
                            break;
                        case "cdTime":
                            sc.cdTime = int.Parse(e.InnerText);
                            break;
                        case "skillTime":
                            sc.skillTime = int.Parse(e.InnerText);
                            break;
                        case "aniAction":
                            sc.aniAction = int.Parse(e.InnerText);
                            break;
                        case "fx":
                            sc.fx = e.InnerText;
                            break;
                        case "isCombo":
                            sc.isCombo = e.InnerText.Equals("1");
                            break;
                        case "isCollide":
                            sc.isCollide = e.InnerText.Equals("1");
                            break;
                        case "isBreak":
                            sc.isBreak = e.InnerText.Equals("1");
                            break;
                        case "dmgType":
                            if (e.InnerText.Equals("1")) {
                                sc.dmgType = DamageType.AD;
                            }
                            else if (e.InnerText.Equals("2")) {
                                sc.dmgType = DamageType.AP;
                            }
                            else {
                                PECommon.Log("dmgType ERROR");
                            }
                            break;
                        case "skillMoveLst":
                            string[] skMoveArr = e.InnerText.Split('|');
                            for (int j = 0; j < skMoveArr.Length; j++) {
                                if (skMoveArr[j] != "") {
                                    sc.skillMoveLst.Add(int.Parse(skMoveArr[j]));
                                }
                            }
                            break;
                        case "skillActionLst":
                            string[] skActionArr = e.InnerText.Split('|');
                            for (int j = 0; j < skActionArr.Length; j++) {
                                if (skActionArr[j] != "") {
                                    sc.skillActionLst.Add(int.Parse(skActionArr[j]));
                                }
                            }
                            break;
                        case "skillDamageLst":
                            string[] skDamageArr = e.InnerText.Split('|');
                            for (int j = 0; j < skDamageArr.Length; j++) {
                                if (skDamageArr[j] != "") {
                                    sc.skillDamageLst.Add(int.Parse(skDamageArr[j]));
                                }
                            }
                            break;
                    }
                }
                skillDic.Add(ID, sc);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public SkillCfg GetSkillCfg(int id) {
        SkillCfg sc = null;
        if (skillDic.TryGetValue(id, out sc)) {
            return sc;
        }
        return null;
    }
    #endregion

    #region 技能配置
    private Dictionary<int, SkillActionCfg> skillActionDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillActionCfg sac = new SkillActionCfg {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "delayTime":
                            sac.delayTime = int.Parse(e.InnerText);
                            break;
                        case "radius":
                            sac.radius = float.Parse(e.InnerText);
                            break;
                        case "angle":
                            sac.angle = int.Parse(e.InnerText);
                            break;
                    }
                }
                skillActionDic.Add(ID, sac);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public SkillActionCfg GetSkillActionCfg(int id) {
        SkillActionCfg sac = null;
        if (skillActionDic.TryGetValue(id, out sac)) {
            return sac;
        }
        return null;
    }
    #endregion

    #region 技能配置
    private Dictionary<int, SkillMoveCfg> skillMoveDic = new Dictionary<int, SkillMoveCfg>();

    private void InitSkillMoveCfg(string assetName, object asset, float duration, object userData) {
        TextAsset xml = asset as TextAsset;
        if (!xml) {
            PECommon.Log("xml file:" + assetName + " not exist", LogType.Error);
        }
        else {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text);

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++) {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null) {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                SkillMoveCfg smc = new SkillMoveCfg {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes) {
                    switch (e.Name) {
                        case "delayTime":
                            smc.delayTime = int.Parse(e.InnerText);
                            break;
                        case "moveTime":
                            smc.moveTime = int.Parse(e.InnerText);
                            break;
                        case "moveDis":
                            smc.moveDis = float.Parse(e.InnerText);
                            break;
                    }
                }
                skillMoveDic.Add(ID, smc);
            }
        }
        GameEntry.Resource.UnloadAsset(asset);
    }
    public SkillMoveCfg GetSkillMoveCfg(int id) {
        SkillMoveCfg smc = null;
        if (skillMoveDic.TryGetValue(id, out smc)) {
            return smc;
        }
        return null;
    }
    #endregion
    #endregion
}