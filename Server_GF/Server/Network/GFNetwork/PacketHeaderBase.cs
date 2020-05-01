//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Network;

public abstract class PacketHeaderBase : IPacketHeader, IReference {
    public abstract PacketType PacketType {
        get;
    }

    public ushort Id {
        get;
        set;
    }
    /// <summary>
    /// 待解析的消息长度
    /// 发送时=总长-2
    /// 接收时=发送时-3
    /// </summary>
    public int PacketLength {
        get;
        set;
    }

    public bool IsValid {
        get {
            return PacketType != PacketType.Undefined && Id > 0 && PacketLength >= 0;
        }
    }

    public void Clear() {
        Id = 0;
        PacketLength = 0;
    }
}
