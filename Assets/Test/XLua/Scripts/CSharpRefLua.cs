using Test.XLua.Components;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Test.XLua.Scripts
{
    public class CSharpRefLua : MonoBehaviour
    {
        public PanelCommon panelCommon;
        private LuaTable m_LuaTable;
        public Text m_LoadMark;

        private void Start()
        {
            RefreshGlobalReference();
            if (panelCommon != null)
            {
                panelCommon.OnGCFinish += RefreshGlobalReference;
            }
        }
        
        private void RefreshGlobalReference()
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
            if (panelCommon != null)
            {
                panelCommon.OnGCFinish -= RefreshGlobalReference;
            }
        }
        
        public void OnLoadLuaFileClick()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSCallLua.reference_test'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSCallLua.reference_test')");
            RefreshGlobalReference();
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
            XLuaEnv.Instance.DoString("package.loaded['01_CSCallLua.clear_global_reference'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSCallLua.clear_global_reference')");
            RefreshGlobalReference();
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
            
            RefreshGlobalReference();
        }
        
        public void PrintLuaTable()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSCallLua.print_global_reference'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSCallLua.print_global_reference')");
        }
    }
}
