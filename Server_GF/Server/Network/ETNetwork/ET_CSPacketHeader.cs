using System;
    public class ET_CSPacketHeader : PacketHeaderBase {

        public byte Flag;

        public override PacketType PacketType {
            get {
                return PacketType.ClientToServer;
            }
        }
    }

