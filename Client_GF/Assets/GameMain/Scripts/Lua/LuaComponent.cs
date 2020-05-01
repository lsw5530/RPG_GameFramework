using System;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;
using XLua;

namespace GameMain {
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Lua")]
    public class LuaComponent : GameFrameworkComponent, ICustomComponent {
        private LuaEnv m_luaEnv = null;
        public LuaEnv LuaEnv {
            get {
                return m_luaEnv;
            }
        }

        [SerializeField]
        private const float GCInterval = 1;


        private float lastGCTime = 0;
        private Action luaStart = null;
        private Action luaUpdate = null;
        private Action luaLateUpdate = null;
        private Action luaFixedUpdate = null;
        private Action luaOnDestroy = null;
        private Action luaOnSceneLoaded = null;

        public void InitSvc() {
            m_luaEnv = new LuaEnv();
            GameEntry.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);

            //初始化自定义加载器
            InitCustomLoader();

            StartLuaMain();
        }

        public void Clear() {
            luaOnDestroy?.Invoke();

            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneSuccess);
            luaStart = null;
            luaUpdate = null;
            luaLateUpdate = null;
            luaFixedUpdate = null;
            luaOnSceneLoaded = null;
            luaOnDestroy = null;

            m_luaEnv?.Dispose();
        }
        private void InitCustomLoader() {
            m_luaEnv.AddLoader((ref string fileName) => {
                fileName = fileName.Replace('.', '/');
                if (GameEntry.Base.EditorResourceMode) {
                    fileName = fileName + ".lua";
                }
                else {
                    fileName = fileName.Substring(fileName.LastIndexOf('/') + 1) + ".lua";
                }
                string data = string.Empty;
                data = GameEntry.Res.LoadAssetSync(fileName);
                if (string.IsNullOrEmpty(data)) {
                    Log.Error("File content is null. file:{0}", fileName);
                    return null;
                }
                return System.Text.Encoding.UTF8.GetBytes(data);
            });
        }
        private void StartLuaMain() {
            m_luaEnv.DoString("require 'Common/GameMain'");
            LuaTable main = m_luaEnv.Global.Get<LuaTable>("GameMain");
            luaStart = main.Get<Action>("Entry");
            luaUpdate = main.Get<Action>("Update");
            luaLateUpdate = main.Get<Action>("LateUpdate");
            luaFixedUpdate = main.Get<Action>("FixedUpdate");
            luaOnDestroy = main.Get<Action>("Exit");
            luaOnSceneLoaded = main.Get<Action>("OnSceneLoaded");

            luaStart?.Invoke();
        }

        /// <summary>
        /// 重载Lua
        /// </summary>
        public void ReloadMain() {
            m_luaEnv?.DoString("package.loaded['Common/GameMain.lua'] = nil");
            StartLuaMain();
            //GameEntry.FairyGui.ReloadLuaForm();
        }

        private void OnLoadSceneSuccess(object sender, GameEventArgs e) {
            luaOnSceneLoaded?.Invoke();
        }

        void Update() {
            luaUpdate?.Invoke();

            if (Time.time - lastGCTime > GCInterval) {
                m_luaEnv?.Tick();
                lastGCTime = Time.time;
            }
        }

        void LateUpdate() {
            luaLateUpdate?.Invoke();
        }

        void FixedUpdate() {
            luaFixedUpdate?.Invoke();
        }


        public void Close() {
            luaOnDestroy?.Invoke();

            GameEntry.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnLoadSceneSuccess);
            luaStart = null;
            luaUpdate = null;
            luaLateUpdate = null;
            luaFixedUpdate = null;
            luaOnSceneLoaded = null;
            luaOnDestroy = null;

            m_luaEnv?.Dispose();
        }

    }
}
