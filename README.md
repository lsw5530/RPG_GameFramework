# RPG_GameFramework 简介
RPG_GameFramework是本人疫情期间做的练手项目，耗时2周多。Unity版本2019.3.4f1,Game Framework Version: 2020.04.21。
- **项目运行指导：**

1.首先克隆完整项目，大概1.75GB。

2.从DataBase恢复数据库。

3.打开服务器项目，把数据库连接地址及密码修改成你自己的，运行。

4.打开客户端项目，把服务器地址修改成你自己的，运行。

5.也可以提前下载apk在安卓手机测试运行。下载地址：http://129.28.170.32/04apps/ZhanShen_2020.apk

- **项目亮点：**

1.整合Xlua热更模块，Lua代码打入AB包，随同资源一起更新；Lua模块包含Unity中常用的数据结构和函数，比如Vector3，Corotine等，努力减少对C#的调用。

2.一键生成Proto3协议，用于客户端和服务器间通信，协议解析部分参考了ET的部分代码。

3.服务器端项目使用的C#，网络IO使用了Select多路复用，比我以往的项目接入一个连接新开一个线程，性能应该提高了些。今后的改进方向是NIO(Non-blockingI/O,在Java领域,也称为NewI/O)，当前还在学习Netty。

4.客户端完整执行流程：

①ProcedureLaunch-初始化app配置信息，调试窗口等。

②ProcedureSplash-显示Splash动画。

③ProcedureCheckVersion-获取本机设备信息，app版本信息，向CDN发送Post请求，获取更新资源版本地址和文件信息。

④ProcedureUpdateResource-分别向CDN、只读区、可读可写区获取资源列表信息，根据MD5码比对，获取资源更新列表，下载资源数目和大小。

⑤ProcedurePreload-预加载项目运行所必须的字体，配置表等。

⑥ProcedureInitLua-初始化Lua虚拟机，添加自定义Loader，执行GameMain.lua。

⑦ProcedureLogin-进入登录/注册场景。

⑧ProcedureMainCity-进入主城场景，这里包含副本系统入口，战斗系统，任务系统，聊天，锻造等。

⑨ProcedureBattle-进入关卡战斗场景，包含人物控制系统，技能系统等。
