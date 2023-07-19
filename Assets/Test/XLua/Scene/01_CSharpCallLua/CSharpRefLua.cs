using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Test.XLua.Scene._01_CSharpCallLua
{
    public class CSharpRefLua : MonoBehaviour
    {
        private LuaTable m_LuaTable;
        public Text m_LoadMark;

        private void Start()
        {
            LuaTable tmp_ref = XLuaEnv.Instance.Global.Get<LuaTable>("global_reference");
            if (tmp_ref != null)
            {
                m_LoadMark.text = "global_reference = " + tmp_ref.Get<string>("name");
            }
            else
            {
                m_LoadMark.text = "global_reference = nil";
            }
        }
        
        private void OnDestroy()
        {
            XLuaEnv.Instance.FullGc();
        }

        public void OnLoadLuaFileClick()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.reference_test'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSharpCallLua.reference_test')");
        }

        public void OnLocalRefTableClick()
        {
            LuaTable tmp_ref = XLuaEnv.Instance.Global.Get<LuaTable>("global_reference");
            if (tmp_ref == null)
            {
                Debug.LogFormat("global_reference is nil");
            }
            else
            {
                Debug.LogFormat("global_reference.name = {0}", tmp_ref?.Get<string>("name"));
            }
        }

        public void OnMonoRefTableClick()
        {
            m_LuaTable = XLuaEnv.Instance.Global.Get<LuaTable>("global_reference");
            if (m_LuaTable == null)
            {
                Debug.LogFormat("global_reference is nil");
            }
            else
            {
                var name = "";
                m_LuaTable?.Get("name", out name);
                Debug.LogFormat("global_reference.name = {0}", name);
            }
        }
        
        public void OnClearLuaTableClick()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.clear_global_reference'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSharpCallLua.clear_global_reference')");
        }

        public void PrintCSharpReference()
        {
            if (m_LuaTable == null)
            {
                Debug.Log("LuaTable is null");
            }
            else
            {
                var name = "";
                m_LuaTable?.Get("name", out name);
                Debug.LogFormat("LuaTable.name = {0}", name);
            }
        }

        public void OnCSharpLuaTableDispose()
        {
            if (m_LuaTable == null)
            {
                return;
            }
            
            m_LuaTable.Dispose();
            m_LuaTable = null;
        }
        
        public void PrintLuaTable()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.print_global_reference'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSharpCallLua.print_global_reference')");
        }
    }
}
