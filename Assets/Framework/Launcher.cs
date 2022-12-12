using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    [DisallowMultipleComponent]
    public class Launcher : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            if(gameObject.GetComponent<XLuaComponent>() == null)
            {
                gameObject.AddComponent<XLuaComponent>();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnApplicationQuit()
        {
            
        }

        private void OnApplicationPause(bool pause)
        {
            
        }
    }
}
