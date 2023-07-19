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
    }
}


