using UnityEngine;
using XLua;

namespace Test.XLua.Scene._01_CSharpCallLua
{
    public class CSharpRefLua : MonoBehaviour
    {
        private LuaTable m_LuaTable;
        
        private void Start()
        {
            
        }
        
        private void OnDestroy()
        {
            
        }
        
        public void OnLoadClick()
        {
            XLuaEnv.Instance.DoString("require '01_CSharpCallLua.test_ref'");
            
            // 局部变量, 不需要调用 Dispose, 自动销毁, 走析构
            // LuaTable global = m_Env?.Global.Get<LuaTable>("global");
            // Debug.Log(global?.Get<string>("name"));
            
            m_LuaTable = XLuaEnv.Instance.Global.Get<LuaTable>("global");
            Debug.Log(m_LuaTable?.Get<string>("name"));
        }
        
        public void OnDestroyGlobalClick()
        {
            // 这里一定要设置一下, 否则 lua 端还持有 global
            // m_Env?.Global.Set<string, LuaTable>("global", null);
            // m_Env?.DoString("global = nil");
            XLuaEnv.Instance?.DoString("package.loaded['01_CSharpCallLua.clear_global'] = nil");
            XLuaEnv.Instance?.DoString("require '01_CSharpCallLua.clear_global'");
            
            m_LuaTable?.Dispose();
            m_LuaTable = null;
            
            XLuaEnv.Instance?.Tick(Time.time);
            XLuaEnv.Instance?.FullGc();// = collectgarbage("collect")
            
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
        
        public void OnPrintGlobal()
        {
            XLuaEnv.Instance?.DoString("package.loaded['01_CSharpCallLua.print_global'] = nil");
            XLuaEnv.Instance?.DoString("require('01_CSharpCallLua.print_global')");
            
            Debug.Log(m_LuaTable?.Get<string>("name"));
        }
    }
}
