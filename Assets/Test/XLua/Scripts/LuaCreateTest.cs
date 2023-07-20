using Test.XLua.Components;
using UnityEngine;

namespace Test.XLua.Scripts
{
    public class LuaCreateTest : MonoBehaviour
    {
        public void OnLoadLuaFileClick()
        {
            XLuaEnv.Instance.DoString("package.loaded['02_LuaCallCS.LuaCallCS_Create'] = nil");
            XLuaEnv.Instance.DoString("require('02_LuaCallCS.LuaCallCS_Create')");
        }

        public void OnDestroyLuaGameObject()
        {
            var obj = GameObject.Find("LuaRootGameObject");
            if (obj)
            {
                GameObject.Destroy(obj);
            }
        }

        public void OnTestLuaRootGameObject()
        {
            XLuaEnv.Instance.DoString("package.loaded['02_LuaCallCS.LuaCallCS_Test'] = nil");
            XLuaEnv.Instance.DoString("require('02_LuaCallCS.LuaCallCS_Test')");
        }
        
        public void OnTestLuaRootGameObject2()
        {
            XLuaEnv.Instance.DoString("package.loaded['02_LuaCallCS.LuaCallCS_Test2'] = nil");
            XLuaEnv.Instance.DoString("require('02_LuaCallCS.LuaCallCS_Test2')");
        }
    }
}


