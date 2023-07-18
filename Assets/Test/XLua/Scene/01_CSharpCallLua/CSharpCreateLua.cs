using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Test.XLua.Scene._01_CSharpCallLua
{
    public class CSharpCreateLua : MonoBehaviour
    {
        [SerializeField] private Text m_LuaMemory;
        private List<LuaTable> m_LuaTableList;
        
        private void Start()
        {
            XLuaEnv.Instance.FullGc();
            m_LuaTableList = new List<LuaTable>();
        }
        
        private void Update()
        {
            if (m_LuaMemory != null)
            {
                m_LuaMemory.text = XLuaEnv.Instance.Memory.ToString();
            }
        }

        private void CreateLuaTable(bool record = false)
        {
            for (int i = 0; i < 100; i++)
            {
                var table = XLuaEnv.Instance.NewTable();
                if (record)
                {
                    m_LuaTableList.Add(table);
                }
            }
        }

        #region ButtonEvent
        
        public void OnCreateTableClick()
        {
            CreateLuaTable();
        }
        
        public void OnCreateTableClick2()
        {
            CreateLuaTable(true);
        }
        
        public void OnGCClick()
        {
            XLuaEnv.Instance.FullGc();
        }
        
        public void OnCreateTableClick3()
        {
            LuaTable rootTable = XLuaEnv.Instance.NewTable();
            for (int i = 0; i < 100; i++)
            {
                var table = XLuaEnv.Instance.NewTable();
                rootTable.Set(i, table);
            }
            XLuaEnv.Instance.Global.Set("CSharpLuaTable", rootTable);
        }
        
        public void OnCreateTableClick4()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.clear_global'] = nil");
            XLuaEnv.Instance.DoString("require '01_CSharpCallLua.clear_global'");
        }
        
        #endregion
    }
}