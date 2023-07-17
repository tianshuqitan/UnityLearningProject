using System;
using UnityEngine;
using XLua;

namespace Test.XLua._01_CSharpCallLua
{
    public class CSharpCallLua : MonoBehaviour
    {
        private LuaEnv m_Env;
        private LuaTable m_Global;
        public TextAsset main;

        private static float lastGCTime = 0;
        private const float GCInterval = 1; // 1 second 
        
        private void Start()
        {
            m_Env = new LuaEnv();
        }
        
        private void Update()
        {
            if (m_Env == null || !(Time.time - lastGCTime > GCInterval))
            {
                return;
            }
            
            m_Env.Tick();
            lastGCTime = Time.time;
        }
        
        private void OnDestroy()
        {
            m_Global?.Dispose();
            m_Global = null;
            
            m_Env?.Dispose();
            m_Env = null;
        }
        
        public void OnLoadClick()
        {
            m_Env?.DoString(main.text, "main");
        }

        public void OnDestroyGlobalClick()
        {
            // 这里一定要设置一下, 否则 lua 端还持有 global
            m_Env?.Global.Set<string, LuaTable>("global", null);
            
            m_Global?.Dispose();
            m_Global = null;
            
            m_Env?.Tick();
            m_Env?.FullGc();// = collectgarbage("collect")
            
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        
        public void OnCreateGlobalClick()
        {
            OnDestroyGlobalClick();
            if (m_Env == null)
            {
                return;
            }
            
            m_Global = m_Env.NewTable();
            m_Global.Set("name", "global");
            m_Global.Set("source", "CSharpCallLua");
            
            // 设置到 Lua.Global
            // m_Env?.Global.Set("global", m_Global);
            m_Env?.Global.Set<string, LuaTable>("global", m_Global);
        }
    }
}

