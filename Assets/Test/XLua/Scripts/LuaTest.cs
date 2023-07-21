using Test.XLua.Components;
using UnityEngine;

namespace Test.XLua.Scripts
{
    public class LuaTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            XLuaEnv.Instance.DoString("package.loaded['03_LuaTest.luaTest'] = nil");
            XLuaEnv.Instance.DoString("require('03_LuaTest.luaTest')");
        }
    }
}


