using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test.XLua
{
    public class CommonPanel : MonoBehaviour
    {
        [SerializeField] private string m_MainSceneName = "Scene_Launch";

        public void GotoMain()
        {
            SceneManager.LoadScene(m_MainSceneName, LoadSceneMode.Single);
        }
    }
}