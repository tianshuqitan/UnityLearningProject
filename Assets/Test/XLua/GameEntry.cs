using Test.XLua.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test.XLua
{
    public class GameEntry : MonoBehaviour
    {
        private static GameEntry m_Instance;
        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this;
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    
        private void Update()
        {
            XLuaEnv.Instance.Tick(Time.time);
        }
    }
}



