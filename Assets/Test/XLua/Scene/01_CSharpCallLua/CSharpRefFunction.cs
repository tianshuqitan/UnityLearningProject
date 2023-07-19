using System;
using UnityEngine;
using XLua;

namespace Test.XLua.Scene._01_CSharpCallLua
{
    [CSharpCallLua]
    public delegate int Delegate_Test(int a, int b);
    
    public class CSharpRefFunction : MonoBehaviour
    {
        private Delegate_Test addNum;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnLoadLuaFileClick()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.function_ref_test'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSharpCallLua.function_ref_test')");
        }

        public void OnRefLuaFunctionClick()
        {
            LuaTable tmpTable = XLuaEnv.Instance.Global.Get<LuaTable>("global_reference");
            if (tmpTable == null)
            {
                Debug.LogError("global_reference is nil");
                return;
            }
            
            tmpTable.Get("addNum", out addNum);
        }

        public void OnClearLuaFile()
        {
            XLuaEnv.Instance.DoString("package.loaded['01_CSharpCallLua.clear_global_reference'] = nil");
            XLuaEnv.Instance.DoString("require('01_CSharpCallLua.clear_global_reference')");
        }
        
        public void ExecuteAddNum()
        {
            if (addNum == null)
            {
                Debug.Log("Delegate addNum is null");
            }
            else
            {
                Debug.LogFormat("AddNum(3,4) = {0}", addNum(3,4));
            }
        }

        public void OnDelegateEmptyClick()
        {
            addNum = null;
            
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }
    }
}
