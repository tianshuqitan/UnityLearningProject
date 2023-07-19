using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Test.XLua.Config
{
    public static class XLuaTestGenConfig
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>()
        {
            typeof(Vector3),
            typeof(GameObject),
        };
    }
}
