using System.IO;
using UnityEngine;
using XLua;

namespace XLuaFramework.Core
{
    public class XLuaEnv
    {
        #region Singleton

        private static readonly XLuaEnv instance = new XLuaEnv();
        static XLuaEnv()
        {

        }

        private XLuaEnv()
        {

        }

        ~XLuaEnv()
        {
            Dispose();
        }

        public static XLuaEnv Instance { get { return instance; } }

        #endregion

        #region Property

        private bool _initialized = false;

        private LuaEnv m_luaEnv = null;
        internal static float lastGCTime = 0;
        private const float GC_INTERVAL = 1; //1 second 

        public static string LuaRootPath = "Resources/LuaScripts";

        #endregion


        #region Public

        #endregion

        public void InitLuaEnv()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            m_luaEnv = new LuaEnv();

            // 添加 rapidjson 库
            m_luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);

            m_luaEnv.AddLoader(CustomLuaLoader);

            // 启动脚本
            m_luaEnv.DoString("require 'main'");
        }

        public void Tick(float time)
        {
            if (m_luaEnv != null && time - XLuaEnv.lastGCTime > GC_INTERVAL)
            {
                m_luaEnv.Tick();
                XLuaEnv.lastGCTime = time;
            }
        }

        public void Dispose()
        {
            if (m_luaEnv != null)
            {
                m_luaEnv.Dispose();
                m_luaEnv = null;
            }
        }

        #region Other

        public byte[] CustomLuaLoader(ref string filePath)
        {
            filePath = filePath.Replace('.', '/') + ".lua";
#if UNITY_EDITOR
            filePath = string.Format("{0}/{1}/{2}", Application.dataPath, LuaRootPath, filePath);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                return null;
            };
#else
            TextAsset file = (TextAsset)Resources.Load(filepath);
            if (file != null)
            {
                return file.bytes;
            }
            else
            {
                return null;
            }
#endif
        }

        #endregion
    }
}
