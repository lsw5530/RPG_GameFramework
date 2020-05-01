using System.IO;
using UnityEditor;
using UnityEngine;

public class GlobalProtoEditor : EditorWindow {
    const string path = @"./Assets/Editor/Config/GlobalProto.txt";

    private GlobalProto globalProto;

    //[MenuItem("Dark God/全局配置")]
    public static void ShowWindow() {
        GetWindow<GlobalProtoEditor>();
    }
    //[MenuItem("Dark God/web资源服务器")]//ET的资源服务器在这里不管用，不知为什么
    public static void OpenFileServer() {
        ProcessHelper.Run("dotnet", "FileServer.dll", "../FileServer/");
    }
    public void Awake() {
        if (File.Exists(path)) {
            this.globalProto = JsonHelper.FromJson<GlobalProto>(File.ReadAllText(path));
        }
        else {
            this.globalProto = new GlobalProto();
        }
    }

    public void OnGUI() {
        if (globalProto == null) return;
        this.globalProto.AssetBundleServerUrl = EditorGUILayout.TextField("资源路径:", this.globalProto.AssetBundleServerUrl);
        this.globalProto.Address = EditorGUILayout.TextField("服务器地址:", this.globalProto.Address);

        if (GUILayout.Button("保存")) {
            File.WriteAllText(path, JsonHelper.ToJson(this.globalProto));
            AssetDatabase.Refresh();
        }
    }
}
