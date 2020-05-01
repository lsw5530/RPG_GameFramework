using GameFramework.Debugger;
using UnityEngine;

public class LuaDebuggerWindow : IDebuggerWindow {
    private Vector2 m_ScrollPosition = Vector2.zero;

    public void Initialize(params object[] args) {

    }

    public void Shutdown() {

    }

    public void OnEnter() {

    }

    public void OnLeave() {

    }

    public void OnUpdate(float elapseSeconds, float realElapseSeconds) {
    }

    public void OnDraw() {
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
        {
            DrawLuaDebugger();
        }
        GUILayout.EndScrollView();
    }

    private void DrawLuaDebugger() {
        if (GUILayout.Button("Reload", GUILayout.Height(30))) {
            //GameEntry.Lua.ReloadMain();
            Debug.LogError("jjjjjjjjjjjjjj");
        }
    }
}
