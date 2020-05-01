/****************************************************
	文件：DBMgr.cs


	
	功能：数据库管理类
*****************************************************/

using System;
using MySql.Data.MySqlClient;
//using KDProtocol;
using GameMessage;

public class DBMgr {
    private static DBMgr instance = null;
    public static DBMgr Instance {
        get {
            if (instance == null) {
                instance = new DBMgr();
            }
            return instance;
        }
    }
    private MySqlConnection conn;

    public void Init() {
#if true
        conn = new MySqlConnection("server=localhost;User Id = root;pwd=root;Database=darkgod;Charset = utf8");
#else
        conn = new MySqlConnection("server=localhost;User Id = root;pwd=;Database=darkgod;Charset = utf8");
#endif
        conn.Open();
        KDCommon.Log("DBMgr Init Done.");

        //QueryPlayerData("xxxx", "oooo");
    }


    public PlayerData QueryPlayerData(string acct, string pass) {
        bool isNew = true;
        PlayerData playerData = null;
        MySqlDataReader reader = null;
        try {
            MySqlCommand cmd = new MySqlCommand("select * from account where acct = @acct", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            reader = cmd.ExecuteReader();
            if (reader.Read()) {
                isNew = false;
                string _pass = reader.GetString("pass");
                if (_pass.Equals(pass)) {
                    //密码正确，返回玩家数据
                    playerData = new PlayerData {
                        Id = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        Lv = reader.GetInt32("level"),
                        Exp = reader.GetInt32("exp"),
                        Power = reader.GetInt32("power"),
                        Coin = reader.GetInt32("coin"),
                        Diamond = reader.GetInt32("diamond"),
                        Crystal = reader.GetInt32("crystal"),

                        Hp = reader.GetInt32("hp"),
                        Ad = reader.GetInt32("ad"),
                        Ap = reader.GetInt32("ap"),
                        Addef = reader.GetInt32("addef"),
                        Apdef = reader.GetInt32("apdef"),
                        Dodge = reader.GetInt32("dodge"),
                        Pierce = reader.GetInt32("pierce"),
                        Critical = reader.GetInt32("critical"),

                        Guideid = reader.GetInt32("guideid"),
                        Time = reader.GetInt64("time"),
                        Fuben = reader.GetInt32("fuben"),
                        //TOADD
                    };
                    #region Strong Arr
                    //数据示意：1#2#2#4#3#7#                        
                    string[] strongStrArr = reader.GetString("strong").Split('#');

                    int[] _strongArr = new int[6];
                    for (int i = 0; i < strongStrArr.Length; i++) {
                        if (strongStrArr[i] == "") {
                            continue;
                        }
                        if (int.TryParse(strongStrArr[i], out int starLv)) {
                            _strongArr[i] = starLv;
                        }
                        else {
                            KDCommon.Log("Parse Strong Data Error", LogType.Error);
                        }
                    }
                    playerData.StrongArr.SetRepeated<int>(_strongArr);
                    #endregion

                    #region Task Arr
                    //数据示意：1|1|0#2|1|0#3|1|0#4|1|0#5|1|0#6|1|0#
                    string[] taskStrArr = reader.GetString("task").Split('#');
                    playerData.TaskArr.SetRepeated<string>(new string[6]);
                    for (int i = 0; i < taskStrArr.Length; i++) {
                        if (taskStrArr[i] == "") {
                            continue;
                        }
                        else if (taskStrArr[i].Length >= 5) {
                            playerData.TaskArr[i] = taskStrArr[i];
                        }
                        else {
                            throw new Exception("DataError");
                        }
                    }
                    #endregion

                    //TODO
                }
            }
        }
        catch (Exception e) {
            Debug.LogError("acc:" + acct);
            Debug.LogError("pass:" + pass);
            KDCommon.Log("Query PlayerData By Acct&Pass Error:" + e, LogType.Error);
        }
        finally {
            if (reader != null) {
                reader.Close();
            }
            if (isNew) {
                //不存在账号数据，创建新的默认账号数据，并返回
                playerData = new PlayerData {
                    Id = -1,
                    Name = "",
                    Lv = 1,
                    Exp = 0,
                    Power = 150,
                    Coin = 5000,
                    Diamond = 500,
                    Crystal = 500,

                    Hp = 2000,
                    Ad = 275,
                    Ap = 265,
                    Addef = 67,
                    Apdef = 43,
                    Dodge = 7,
                    Pierce = 5,
                    Critical = 2,

                    Guideid = 1001,
                    //StrongArr = new int[6],
                    Time = TimerSvc.Instance.GetNowTime(),
                    //TaskArr = new string[6],
                    Fuben = 10001,
                    //TOADD
                };
                playerData.StrongArr.SetRepeated<int>(new int[6]);
                playerData.TaskArr.SetRepeated<string>(new string[6]);

                //数据示意：1|0|0#2|0|0#3|0|0#4|0|0#5|0|0#6|0|0#
                //初始化任务奖励数据
                for (int i = 0; i < playerData.TaskArr.Count; i++) {
                    playerData.TaskArr[i] = (i + 1) + "|0|0";
                }


                playerData.Id = InsertNewAcctData(acct, pass, playerData);
            }
        }
        return playerData;
    }

    private int InsertNewAcctData(string acct, string pass, PlayerData pd) {
        int id = -1;
        try {
            MySqlCommand cmd = new MySqlCommand(
                "insert into account set acct=@acct,pass =@pass,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond," +
                "crystal=@crystal,hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical," +
                "guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben", conn);
            cmd.Parameters.AddWithValue("acct", acct);
            cmd.Parameters.AddWithValue("pass", pass);
            cmd.Parameters.AddWithValue("name", pd.Name);
            cmd.Parameters.AddWithValue("level", pd.Lv);
            cmd.Parameters.AddWithValue("exp", pd.Exp);
            cmd.Parameters.AddWithValue("power", pd.Power);
            cmd.Parameters.AddWithValue("coin", pd.Coin);
            cmd.Parameters.AddWithValue("diamond", pd.Diamond);
            cmd.Parameters.AddWithValue("crystal", pd.Crystal);

            cmd.Parameters.AddWithValue("hp", pd.Hp);
            cmd.Parameters.AddWithValue("ad", pd.Ad);
            cmd.Parameters.AddWithValue("ap", pd.Ap);
            cmd.Parameters.AddWithValue("addef", pd.Addef);
            cmd.Parameters.AddWithValue("apdef", pd.Apdef);
            cmd.Parameters.AddWithValue("dodge", pd.Dodge);
            cmd.Parameters.AddWithValue("pierce", pd.Pierce);
            cmd.Parameters.AddWithValue("critical", pd.Critical);

            cmd.Parameters.AddWithValue("guideid", pd.Guideid);

            string strongInfo = "";
            for (int i = 0; i < pd.StrongArr.Count; i++) {
                strongInfo += pd.StrongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.Parameters.AddWithValue("time", pd.Time);

            //1|0|0#1|0|0#1|0|0#1|0|0#1|0|0#
            string taskInfo = "";
            for (int i = 0; i < pd.TaskArr.Count; i++) {
                taskInfo += pd.TaskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            cmd.Parameters.AddWithValue("fuben", pd.Fuben);

            //TOADD
            cmd.ExecuteNonQuery();
            id = (int)cmd.LastInsertedId;
        }
        catch (Exception e) {
            KDCommon.Log("Insert PlayerData Error:" + e, LogType.Error);
        }
        return id;
    }

    public bool QueryNameData(string name) {
        bool exist = false;
        MySqlDataReader reader = null;
        try {
            MySqlCommand cmd = new MySqlCommand("select * from account where name= @name", conn);
            cmd.Parameters.AddWithValue("name", name);
            reader = cmd.ExecuteReader();
            if (reader.Read()) {
                exist = true;
            }
        }
        catch (Exception e) {
            KDCommon.Log("Query Name State Error:" + e, LogType.Error);
        }
        finally {
            if (reader != null) {
                reader.Close();
            }
        }

        return exist;
    }

    public bool UpdatePlayerData(int id, PlayerData playerData) {
        try {
            MySqlCommand cmd = new MySqlCommand(
            "update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
            "hp = @hp, ad = @ad, ap = @ap, addef = @addef, apdef = @apdef, dodge = @dodge, pierce = @pierce, critical = @critical," +
            "guideid=@guideid,strong=@strong,time=@time,task=@task,fuben=@fuben where id =@id", conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("name", playerData.Name);
            cmd.Parameters.AddWithValue("level", playerData.Lv);
            cmd.Parameters.AddWithValue("exp", playerData.Exp);
            cmd.Parameters.AddWithValue("power", playerData.Power);
            cmd.Parameters.AddWithValue("coin", playerData.Coin);
            cmd.Parameters.AddWithValue("diamond", playerData.Diamond);
            cmd.Parameters.AddWithValue("crystal", playerData.Crystal);

            cmd.Parameters.AddWithValue("hp", playerData.Hp);
            cmd.Parameters.AddWithValue("ad", playerData.Ad);
            cmd.Parameters.AddWithValue("ap", playerData.Ap);
            cmd.Parameters.AddWithValue("addef", playerData.Addef);
            cmd.Parameters.AddWithValue("apdef", playerData.Apdef);
            cmd.Parameters.AddWithValue("dodge", playerData.Dodge);
            cmd.Parameters.AddWithValue("pierce", playerData.Pierce);
            cmd.Parameters.AddWithValue("critical", playerData.Critical);

            cmd.Parameters.AddWithValue("guideid", playerData.Guideid);

            string strongInfo = "";
            for (int i = 0; i < playerData.StrongArr.Count; i++) {
                strongInfo += playerData.StrongArr[i];
                strongInfo += "#";
            }
            cmd.Parameters.AddWithValue("strong", strongInfo);
            cmd.Parameters.AddWithValue("time", playerData.Time);

            string taskInfo = "";
            for (int i = 0; i < playerData.TaskArr.Count; i++) {
                taskInfo += playerData.TaskArr[i];
                taskInfo += "#";
            }
            cmd.Parameters.AddWithValue("task", taskInfo);
            cmd.Parameters.AddWithValue("fuben", playerData.Fuben);

            //TOADD Others
            cmd.ExecuteNonQuery();
        }
        catch (Exception e) {
            KDCommon.Log("Update PlayerData Error:" + e, LogType.Error);
            return false;
        }
        return true;
    }
}
