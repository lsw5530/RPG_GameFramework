 
using GameMessage;
using Google.Protobuf.Collections;

public static class ProtoHelper {
    public static string PublicKey = "OceanSever";

    public static string SecretKey { get; private set; }
    public static RepeatedField<T> SetRepeated<T>(this RepeatedField<T> x, RepeatedField<T> y) {
        x.Clear();
        for (int i = 0; i < y.Count; i++) {
            x.Add(y[i]);
        }
        return x;
    }
    public static void SetKey(SCPacketBase key) {
        RspSecret msgSecret = key as RspSecret;
        SecretKey = msgSecret.Secret;
    }
    internal static void SetKey(string empty) {
        SecretKey = empty;
    }
    //public static RepeatedField<T> SetRepeated<T>(this RepeatedField<T> x, T[] y) {
    //    x.Clear();
    //    for (int i = 0; i < y.Length; i++) {
    //        x.Add(y[i]);
    //    }
    //    return x;
    //}
    //public static byte[] EncodeName(CSPacketBase msgBase) {
    //    Debug.LogError("msgBase.Id::" + ((CMD)msgBase.Id).ToString());
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
    //        return (CMD)System.Enum.Parse(typeof(CMD), name);
    //    }
    //    catch (Exception ex) {
    //        Console.WriteLine("不存在的协议:" + ex.ToString());
    //        return CMD.None;
    //    }
    //}

    //public static byte[] Encond(CSPacketBase msgBase) {
    //    string secret = string.IsNullOrEmpty(SecretKey) ? PublicKey : SecretKey;
    //    using (MemoryStream memory = new MemoryStream()) {
    //        //将我们的协议类进行序列化转换成数组
    //        IMessage msg = (IMessage)msgBase;
    //        msg.WriteTo(memory);
    //        byte[] bytes = memory.ToArray();
    //        bytes = AES.AESEncrypt(bytes, secret);
    //        return bytes;
    //    }
    //}
    //public static SCPacketBase Decode(CMD protocol, byte[] bytes, int offset, int count) {
    //    if (count <= 0) {
    //        MessageParser parser = ProtoDic.GetMessageParser(protocol);
    //        SCPacketBase msg = (SCPacketBase)parser.ParseFrom(new Byte[0]);
    //        return msg;
    //    }
    //    string secret = string.IsNullOrEmpty(SecretKey) ? PublicKey : SecretKey;
    //    try {
    //        byte[] newBytes = new byte[count];
    //        Array.Copy(bytes, offset, newBytes, 0, count);
    //        newBytes = AES.AESDecrypt(newBytes, secret);
    //        using (var memory = new MemoryStream(newBytes, 0, newBytes.Length)) {
    //            MessageParser parser = ProtoDic.GetMessageParser(protocol);
    //            return (SCPacketBase)parser.ParseFrom(memory);
    //        }
    //    }
    //    catch (Exception ex) {
    //        PECommon.Log("协议解密出错:" + ex.ToString(), LogType.Error);
    //        return null;
    //    }
    //}


}
