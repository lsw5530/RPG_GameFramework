//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
 
using System;
using ProtoBuf;

[Serializable, ProtoContract(Name = @"SCHeartBeat")]
public class SCHeartBeat : SCPacketBase {
    public SCHeartBeat() {
    }

    public override int Id {
        get {
            return 2;
        }
    }

    public override void Clear() {
    }
}
