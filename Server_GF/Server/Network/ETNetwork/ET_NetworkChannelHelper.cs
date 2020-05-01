using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GameFramework.Network;
class ET_NetworkChannelHelper {
    private static readonly Dictionary<int, Type> m_ClientToServerPacketTypes = new Dictionary<int, Type>();
    private static readonly List<byte[]> byteses = new List<byte[]>() { new byte[ETPackets.ET_PacketSizeLength], new byte[1], new byte[2] };
    private static MemoryStream memoryStream = new MemoryStream(1024 * 8);
    /// <summary>
    /// 前2个字节代表消息的长度
    /// 第3个字节代表消息flag，1表示rpc消息，0表示普通消息
    /// 第4和第5个字节表示消息的Id
    /// 这样做是为了和ET服务器的消息包的解析保持一致
    /// </summary>
    public static int PacketHeaderLength {
        get {
            return ETPackets.ET_PacketSizeLength + 3;
        }
    }
    public static void Initialize() {
        // 反射注册包和包处理函数。
        Type packetBaseType = typeof(CSPacketBase);
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();
        for (int i = 0; i < types.Length; i++) {
            if (!types[i].IsClass || types[i].IsAbstract) {
                continue;
            }

            if (types[i].BaseType == packetBaseType) {
                PacketBase packetBase = (PacketBase)Activator.CreateInstance(types[i]);
                Type packetType = GetClientToServerPacketType(packetBase.Id);
                if (packetType != null) {
                    Debug.LogError("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
                    continue;
                }

                m_ClientToServerPacketTypes.Add(packetBase.Id, types[i]);
            }
        }
    }
    private static Type GetClientToServerPacketType(int id) {
        Type type = null;
        if (m_ClientToServerPacketTypes.TryGetValue(id, out type)) {
            return type;
        }

        return null;
    }
    public static bool Serialize<T>(T packet, Stream destination) where T : Packet {
        PacketBase packetImpl = packet as PacketBase;
        if (packetImpl == null) {
            Debug.LogError("Packet is invalid.");
            return false;
        }

        if (packetImpl.PacketType != PacketType.ServerToClient) {
            Debug.LogError("Send packet invalid.");
            return false;
        }
         
        memoryStream.Seek(PacketHeaderLength, SeekOrigin.Begin);
        memoryStream.SetLength(PacketHeaderLength);
        ProtobufHelper.ToStream(packet, memoryStream);

        // 头部消息
        ET_SCPacketHeader packetHeader = new ET_SCPacketHeader();
        packetHeader.Flag = 0;      //客户端发送的消息，默认flag为0,服务器会解析flag字段值
        packetHeader.PacketLength = (int)memoryStream.Length - ETPackets.ET_PacketSizeLength; // 消息总长度需要减去前2字节长度,第3，4，5字节也是消息长度（packetsize）的一部分
        //Debug.LogInfo("发送消息长度：" + packetHeader.PacketLength);
        packetHeader.Id = (ushort)packet.Id;

        memoryStream.Position = 0;
        byteses[0].WriteTo(0, (ushort)packetHeader.PacketLength);
        byteses[1][0] = packetHeader.Flag;
        byteses[2].WriteTo(0, packetHeader.Id);
        int index = 0;
        foreach (var bytes in byteses) {
            Array.Copy(bytes, 0, memoryStream.GetBuffer(), index, bytes.Length);
            index += bytes.Length;
        } 
        memoryStream.WriteTo(destination);

        long len = destination.Length;
        long pos = destination.Position;
        byte[] temp = (destination as MemoryStream).GetBuffer();
        return true;
        //}
    }
    public static IPacketHeader DeserializePacketHeader(Stream source) {

        ET_CSPacketHeader scHeader = new ET_CSPacketHeader();
        MemoryStream memoryStream = source as MemoryStream;
        if (memoryStream != null) {
            int packetSize = BitConverter.ToUInt16(memoryStream.GetBuffer(), 0);
            byte flag = memoryStream.GetBuffer()[ETPackets.ET_MessageFlagIndex];
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), ETPackets.ET_MessageOpcodeIndex);

            //这里需要用服务端发过来的packetSize的值减去消息包中flag和opcode的长度，
            //因为服务端在发送消息时设置的packetSize的值是包含flag和opcode的，而
            //客户端在解析包头的时候已经解析了flag和opcode，因此剩余要解析的数据长度要减去3（flag和opcode的总长度是3个字节）
            scHeader.PacketLength = packetSize - ETPackets.ET_MessageIdentifyLength;
            scHeader.Flag = flag;
            scHeader.Id = opcode;
            return scHeader;
        }

        return null;
    }
    public static Packet DeserializePacket(IPacketHeader packetHeader, Stream source/*, out object customErrorData*/) {
        ET_CSPacketHeader header = packetHeader as ET_CSPacketHeader;
        if (header == null) {
            Debug.LogError("header为空.");
            return null;
        }

        Packet packet = null;
        if (header.IsValid) {
            Type packetType = GetClientToServerPacketType(header.Id);

            if (packetType != null && source is MemoryStream) {
                object instance = Activator.CreateInstance(packetType);
                packet = (Packet)ProtobufHelper.FromStream(instance, (MemoryStream)source);
            }
            else {
                Debug.LogError("Can not deserialize packet for packet id '{0}'.", header.Id.ToString());
            }
        }
        else {
            Debug.LogError("Packet header is invalid.");
        }
        header.Clear();
        return packet;
    }
}
