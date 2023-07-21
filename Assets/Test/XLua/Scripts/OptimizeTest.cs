using Test.XLua.Components;
using UnityEngine;

namespace Test.XLua.Scripts
{
    public class OptimizeTest : MonoBehaviour
    {
        public void CreateVector3()
        {
            XLuaEnv.Instance.DoString("package.loaded['02_LuaCallCS.CreateVector3'] = nil");
            XLuaEnv.Instance.DoString("require('02_LuaCallCS.CreateVector3')");
        }
        
        public void CreateVector3Lua()
        {
            XLuaEnv.Instance.DoString("package.loaded['02_LuaCallCS.CreateVector3_Optimize'] = nil");
            XLuaEnv.Instance.DoString("require('02_LuaCallCS.CreateVector3_Optimize')");
        }
    }
}
