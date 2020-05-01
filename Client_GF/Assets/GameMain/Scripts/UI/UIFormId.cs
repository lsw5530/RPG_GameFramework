//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

/// <summary>
/// 界面编号。
/// </summary>
public enum UIFormId {
    Undefined = 0,

    /// <summary>
    /// 弹出框。
    /// </summary>
    DialogForm = 1,

    /// <summary>
    /// 主菜单。
    /// </summary>
    MenuForm = 100,

    /// <summary>
    /// 设置。
    /// </summary>
    SettingForm = 101,

    /// <summary>
    /// 关于。
    /// </summary>
    AboutForm = 102,

    LoginForm = 200,

    DynamicForm = 210,

    LoadingForm = 220,
    CreateForm = 230,//创建角色
    MainCityForm = 240,//主城界面
    InfoForm = 250,//帮助
    GuideForm = 260,//指引
    StrongForm = 270,//强化
    ChatForm = 280,//聊天
    BuyForm = 290,//商城
    TaskForm = 300,//任务
    FubenForm = 310,//副本
    PlayerCtrlForm = 320,//主角控制
    BattleEndForm = 330,//战斗结算

}
