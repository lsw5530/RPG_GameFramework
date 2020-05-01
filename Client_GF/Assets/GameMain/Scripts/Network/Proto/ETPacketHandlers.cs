
using GameFramework.Network;
using GameMessage;
using UnityEngine;

//public partial class ReqLogin_Handler : PacketHandler {
//    public override int Id => throw new System.NotImplementedException();

//    public override void Handle(object sender, Packet packet) {
//        throw new System.NotImplementedException();
//    }
//}
public abstract class ET_PacketHandler : PacketHandlerBase {

    public sealed override void Handle(object sender, Packet packet) {
        if (packet.GetType().BaseType == typeof(SCPacketBase)) {
            SCPacketBase msg = packet as SCPacketBase;
            GameEntry.Net.ProcessMsg(msg);
        }
    }
}
public partial class ReqLogin_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqLogin;
        }
    }
}
public partial class RspLogin_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspLogin;
        }
    } 
}
public partial class ReqRename_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqRename;
        }
    } 
}
public partial class RspRename_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspRename;
        }
    } 
}
public partial class ReqHeartbeat_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqHeartbeat;
        }
    } 
}
public partial class RspHeartbeat_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspHeartbeat;
        }
    } 
}
public partial class ReqSecret_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqSecret;
        }
    } 
}
public partial class RspSecret_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspSecret;
        }
    } 
}
public partial class ReqGuide_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqGuide;
        }
    } 
}
public partial class RspGuide_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspGuide;
        }
    } 
}
public partial class ReqStrong_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqStrong;
        }
    } 
}
public partial class RspStrong_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspStrong;
        }
    } 
}
public partial class SndChat_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.SndChat;
        }
    } 
}
public partial class PshChat_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.PshChat;
        }
    } 
}
public partial class ReqBuy_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqBuy;
        }
    } 
}
public partial class RspBuy_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspBuy;
        }
    } 
}
public partial class PshPower_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.PshPower;
        }
    } 
}
public partial class ReqTakeTaskReward_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqTakeTaskReward;
        }
    } 
}
public partial class RspTakeTaskReward_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspTakeTaskReward;
        }
    } 
}
public partial class PshTaskPrgs_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.PshTaskPrgs;
        }
    } 
}
public partial class ReqFBFight_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqFBFight;
        }
    } 
}
public partial class RspFBFight_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspFBFight;
        }
    } 
}
public partial class ReqFBFightEnd_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.ReqFBFightEnd;
        }
    } 
}
public partial class RspFBFightEnd_Handler : ET_PacketHandler {
    public override int Id {
        get {
            return (int)CMD.RspFBFightEnd;
        }
    } 
}



//public partial class R2C_LoginHandler : PacketHandler {
//    public override int Id {
//        get {
//            return 10002;
//        }
//    }

//    R2C_Login r2c_Login;
//    public override void Handle(object sender, Packet packet) {
//        Debug.Log("Get Message From the Server");
//        R2C_Login loginResult = packet as R2C_Login;
//        if (loginResult != null) {
//            Debug.Log(string.Format("Message Content-> Address:{0}, Key->{1} ", loginResult.Address, loginResult.Key));
//        }
//        r2c_Login = loginResult;
//        //NetworkComponent network = MGame.GameEntry.Network;
//        //INetworkChannelHelper helper = new ET_NetworkChannelHelper();
//        //INetworkChannel nc = network.CreateNetworkChannel("CG_TC", helper);
//        //nc.HeartBeatInterval = 0f;

//        //IPEndPoint ipPoint = NetworkHelper.ToIPEndPoint(loginResult.Address);
//        //nc.Connect(ipPoint.Address, ipPoint.Port,r2c_Login);
//        //GameEntry.Event.Fire(r2c_Login, new MGameEvetArgs(Constant.EventDefine.ConnectGateServer));
//    }

//}