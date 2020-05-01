using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameMessage;
using Google.Protobuf;
using Google.Protobuf.Collections;
public static class ProtoHelper {
    public static RepeatedField<T> SetRepeated<T>(this RepeatedField<T> x, RepeatedField<T> y) {
        x.Clear();
        for (int i = 0; i < y.Count; i++) {
            x.Add(y[i]);
        }
        return x;
    }
    public static RepeatedField<int> SetRepeated<T>(this RepeatedField<int> x, int[] y) {
        x.Clear();
        for (int i = 0; i < y.Length; i++) {
            x.Add(y[i]);
        }
        return x;
    }
    public static RepeatedField<string> SetRepeated<T>(this RepeatedField<string> x, string[] y) {
        x.Clear();
        for (int i = 0; i < y.Length; i++) {
            if (y[i] == null) {
                x.Add(String.Empty);
            }
            else {
                x.Add(y[i]);
            }
        }
        return x;
    }
    
    //public static byte[] EncodeName(SCPacketBase msgBase) {
    //    byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(((CMD)msgBase.Id).ToString());
    //    Int16 len = (Int16)nameBytes.Length;
    //    byte[] bytes = new byte[2 + len];
    //    bytes[0] = (byte)(len % 256);
    //    bytes[1] = (byte)(len / 256);
    //    Array.Copy(nameBytes, 0, bytes, 2, len);
    //    return bytes;
    //}
    //public static CMD DecodeName(byte[] bytes, int offset, out int count) {
    //    count = 0;
    //    if (offset + 2 > bytes.Length) return CMD.None;
    //    Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
    //    if (offset + 2 + len > bytes.Length) return CMD.None;
    //    count = 2 + len;
    //    try {
    //        string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
    //        Console.WriteLine("=========");
    //        Console.WriteLine(CMD.ReqSecret);
    //        Console.WriteLine(name);
    //        Console.WriteLine("=========");
    //        return (CMD)System.Enum.Parse(typeof(CMD), name);
    //    }
    //    catch (Exception ex) {
    //        Console.WriteLine("不存在的协议:" + ex.ToString());
    //        return CMD.ReqHeartbeat;//测试
    //    }
    //}
    ///// <summary>
    ///// 服务器端加密
    ///// </summary>
    ///// <param name="msgBase"></param>
    ///// <returns></returns>
    //public static byte[] Encond(SCPacketBase msgBase) {
    //    using (MemoryStream memory = new MemoryStream()) {
    //        //将我们的协议类进行序列化转换成数组
    //        IMessage message = (IMessage)msgBase;
    //        message.WriteTo(memory);
    //        //Serializer.Serialize(memory, msgBase);
    //        byte[] bytes = memory.ToArray();
    //        string secret = ServerSocket.SecretKey;
    //        if (msgBase is RspSecret) {
    //            secret = ServerSocket.PublicKey;
    //        }
    //        bytes = AES.AESEncrypt(bytes, secret);
    //        return bytes;
    //    }
    //}
    //public static CSPacketBase Decode(CMD protocol, byte[] bytes, int offset, int count) {
    //    if (count <= 0) {
    //        MessageParser parser = ProtoDic.GetMessageParser(protocol);
    //        CSPacketBase msg =(CSPacketBase)parser.ParseFrom(new Byte[0]);
    //        return msg;
    //    }

    //    try {
    //        byte[] newBytes = new byte[count];
    //        Array.Copy(bytes, offset, newBytes, 0, count);
    //        string secret = ServerSocket.SecretKey;
    //        if (protocol == CMD.ReqSecret) {
    //            secret = ServerSocket.PublicKey;
    //        }
    //        else {
    //        }
    //        newBytes = AES.AESDecrypt(newBytes, secret);
    //        using (var memory = new MemoryStream(newBytes, 0, newBytes.Length)) {
    //            //Type t = System.Type.GetType(protocol.ToString());
    //            MessageParser parser = ProtoDic.GetMessageParser(protocol);
    //            return (CSPacketBase)parser.ParseFrom(memory);
    //        }
    //    }
    //    catch (Exception ex) {
    //        KDCommon.Log("协议解密出错:" + ex.ToString(), LogType.Error);
    //        return null;
    //    }
    //}
}
