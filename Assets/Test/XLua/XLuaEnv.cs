using System.IO;
using UnityEngine;
using XLua;

namespace Test.XLua
{
    public class XLuaEnv
    {
        #region Singleton

        private static XLuaEnv instance;

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

        public static XLuaEnv Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XLuaEnv();
                    instance.InitLuaEnv();
                }

                return instance;
            }
        }

        #endregion

        #region Property

        private bool m_Initialized = false;
        private LuaEnv m_LuaEnv = null;
        
        private const float m_GCInterval = 2; //1 second
        private float m_LastGCTime = 0;
        
        [SerializeField] private const string m_LuaRootPath = "Test/XLua/LuaScripts";

        #endregion

        #region Public

        #endregion
        
        public LuaTable Global => m_LuaEnv?.Global;

        public int Memory => m_LuaEnv?.Memroy ?? 0;
        
        private void InitLuaEnv()
        {
            if (m_Initialized)
            {
                return;
            }

            m_Initialized = true;

            m_LuaEnv = new LuaEnv();
            m_LuaEnv.AddLoader(CustomLuaLoader);
        }

        #region LuaEnv

        public void Tick(float time)
        {
            if (m_LuaEnv == null || (time - m_LastGCTime) < m_GCInterval)
            {
                return;
            }
            
            m_LuaEnv.Tick();
            m_LastGCTime = time;
        }

        public object[] DoString(byte[] chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return m_LuaEnv?.DoString(chunk, chunkName, env);
        }

        public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return m_LuaEnv?.DoString(chunk, chunkName, env);
        }

        public LuaTable NewTable()
        {
            return m_LuaEnv?.NewTable();
        }

        public void FullGc()
        {
            m_LuaEnv?.FullGc();
        }
        
        #endregion
        
        
        
        private void Dispose()
        {
            m_LuaEnv?.Dispose();
            m_LuaEnv = null;
        }
        
        
        
        #region Other

        private static byte[] CustomLuaLoader(ref string filePath)
        {
            filePath = filePath.Replace('.', '/') + ".lua.txt";
#if UNITY_EDITOR
            filePath = $"{Application.dataPath}/{m_LuaRootPath}/{filePath}";
            // Debug.LogFormat("CustomLuaLoader {0}", filePath);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                Debug.LogErrorFormat("{0} not find!!!", filePath);
                return null;
            }
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