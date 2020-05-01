using System;

    public class ET_SCPacketHeader : PacketHeaderBase {

        public override PacketType PacketType {
            get {
                return PacketType.ServerToClient;
            }
        }

        public byte Flag;

    }


