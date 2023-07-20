using XLua;

namespace Test.XLua.Components
{
    [LuaCallCSharp]
    [ReflectionUse]
    public static class UnityEngineObjectExtenstion
    {
        public static bool IsNull(this UnityEngine.Object o)
        {
            return o == null;
        }

        public static bool IsDestroy(UnityEngine.Object o)
        {
            return o == null;
        }
    }
}

