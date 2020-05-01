/************************************************************
	文件：UnityToSVN.cs
	
	邮箱：1785275942@qq.com
	
	功能：整合SVN命令到Unity编辑器
*************************************************************/

using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class UnityToSVN {
    private const string Menu_Commit = "SVN/Commit Selected";
    private const string Menu_Commit_All = "SVN/Commit All";
    private const string Menu_Update = "SVN/Update Selected";
    private const string Menu_Update_All = "SVN/Update All";
    private const string Menu_Log = "SVN/Log Selected";
    private const string Menu_Log_All = "SVN/Log All";
    private const string Menu_Cleanup = "SVN/Cleanup";

    #region MenuItem
    [MenuItem(Menu_Commit)]
    public static void SVNCommit() {
        string path = GetSelObjPath(true);
        if (path != "") {
            SVNCmd("commit", path);
        }
        else {
            SVNCommitAll();
        }
    }

    [MenuItem(Menu_Commit_All)]
    public static void SVNCommitAll() {
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 7);
        SVNCmd("commit", path);
    }

    [MenuItem(Menu_Update)]
    public static void SVNUpdate() {
        string path = GetSelObjPath(true);
        if (path != "") {
            SVNCmd("update", path);
        }
        else {
            SVNUpdateAll();
        }
    }

    [MenuItem(Menu_Update_All)]
    public static void SVNUpdateAll() {
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 7);
        SVNCmd("update", path);
    }

    [MenuItem(Menu_Log)]
    public static void SVNLog() {
        string path = GetSelObjPath(true);
        if (path != "") {
            SVNCmd("log", GetSelObjPath(true));
        }
        else {
            SVNLogAll();
        }
    }

    [MenuItem(Menu_Log_All)]
    public static void SVNLogAll() {
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 7);
        SVNCmd("log", path);
    }

    [MenuItem(Menu_Cleanup)]
    public static void SVNCleanup() {
        string path = Application.dataPath;
        path = path.Substring(0, path.Length - 7);
        SVNCmd("cleanup", path);
    }
    #endregion

    public static void SVNCmd(string command, string path) {
        string cmd = "/c tortoiseproc.exe /command:{0} /path:\"{1}\" /closeonend 2";
        cmd = string.Format(cmd, command, path);
        ProcessStartInfo proc = new ProcessStartInfo("cmd.exe", cmd);
        proc.WindowStyle = ProcessWindowStyle.Hidden;
        Process.Start(proc);
    }

    private static string GetSelObjPath(bool firstOne = false) {
        string path = string.Empty;
        for (int i = 0; i < Selection.objects.Length; i++) {
            path += ConvertToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i]));
            if (firstOne) break;
            path += "*";
            path += ConvertToFilePath(AssetDatabase.GetAssetPath(Selection.objects[i])) + ".meta";
            path += "*";
        }
        return path;
    }

    public static string ConvertToFilePath(string path) {
        string m_path = Application.dataPath;
        m_path = m_path.Substring(0, m_path.Length - 6);
        m_path += path;
        return m_path;
    }
}