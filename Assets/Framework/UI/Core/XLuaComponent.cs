using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace XLuaFramework.Core
{
    [DisallowMultipleComponent]
    public class XLuaComponent : MonoBehaviour
    {

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            XLuaEnv.Instance.InitLuaEnv();
        }

        // Update is called once per frame
        void Update()
        {
            XLuaEnv.Instance.Tick(Time.time);
        }

        private void OnDestroy()
        {
            XLuaEnv.Instance.Dispose();
        }
    }
}
