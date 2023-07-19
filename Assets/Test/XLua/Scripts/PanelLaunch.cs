using UnityEngine;
using UnityEngine.SceneManagement;

namespace Test.XLua.Scripts
{
    public class PanelLaunch : MonoBehaviour
    {
        public void SwitchScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
    }
}


