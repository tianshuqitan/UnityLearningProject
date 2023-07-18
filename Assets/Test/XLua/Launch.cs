using System;
using Test.XLua;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launch : MonoBehaviour
{
    [SerializeField]
    private string[] scenePaths;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        XLuaEnv.Instance.DoString("require 'main'");
    }
    
    private void Update()
    {
        XLuaEnv.Instance.Tick(Time.time);
    }
    
    private void OnDestroy()
    {
        
    }
    
    public void SwitchScene(int index)
    {
        if (index >= scenePaths.Length)
        {
            Debug.LogError("index is out of range");
            return;
        }

        SceneManager.LoadScene(scenePaths[index], LoadSceneMode.Single);
    }
}
