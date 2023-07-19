using Test.XLua.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test.XLua.Scripts
{
    public class PanelCommon : MonoBehaviour
    {
        [SerializeField] private string m_MainSceneName = "Launch";

        public void GotoMain()
        {
            SceneManager.LoadScene(m_MainSceneName, LoadSceneMode.Single);
        }
        
        public void LuaGC()
        {
            XLuaEnv.Instance.FullGc();
        }
    }
}