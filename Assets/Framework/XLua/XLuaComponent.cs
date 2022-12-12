using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Localizations;
using Loxodon.Framework.Services;
using Loxodon.Framework.Views;
using UnityEngine;
using XLua;

namespace Framework.Core
{
    [DisallowMultipleComponent]
    public class XLuaComponent : MonoBehaviour
    {
        // XLua
        private LuaEnv m_luaEnv = null;
        internal static float lastGCTime = 0;
        private const float GC_INTERVAL = 1; //1 second 

        public static string LuaRootPath = "Resources/LuaScripts";

        // Loxodon
        private ApplicationContext context;

        #region Awake/Start/Update/OnDestroy

        private void Awake()
        {
            InitLuaEnv();
            InitLoxodon();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (m_luaEnv != null && Time.time - lastGCTime > GC_INTERVAL)
            {
                m_luaEnv.Tick();
                lastGCTime = Time.time;
            }
        }

        private void OnDestroy()
        {
            if (m_luaEnv != null)
            {
                m_luaEnv.Dispose();
                m_luaEnv = null;
            }
        }

        #endregion

        #region MyRegion

        public void InitLuaEnv()
        {
            m_luaEnv = new LuaEnv();

            // 添加 rapidjson 库
            m_luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);

            m_luaEnv.AddLoader(CustomLuaLoader);

            // 启动脚本
            m_luaEnv.DoString("require 'launcher'");
        }

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

        private void InitLoxodon()
        {
            GlobalWindowManager windowManager = FindObjectOfType<GlobalWindowManager>();
            if (windowManager == null)
                throw new NotFoundException("Not found the GlobalWindowManager.");

            context = Context.GetApplicationContext();
            IServiceContainer container = context.GetContainer();

            /* Initialize the data binding service */
            LuaBindingServiceBundle bundle = new LuaBindingServiceBundle(context.GetContainer());
            bundle.Start();

            /* Initialize the ui view locator and register UIViewLocator */
            container.Register<IUIViewLocator>(new DefaultUIViewLocator());

            /* Initialize the localization service */
            //CultureInfo cultureInfo = Locale.GetCultureInfoByLanguage (SystemLanguage.English);
            CultureInfo cultureInfo = Locale.GetCultureInfo();
            var localization = Localization.Current;
            localization.CultureInfo = cultureInfo;
            localization.AddDataProvider(new DefaultDataProvider("LuaLocalizations", new XmlDocumentParser()));

            /* register Localization */
            container.Register<Localization>(Localization.Current);
        }

        #endregion
    }
}
