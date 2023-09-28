using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnabledTest : MonoBehaviour
{
    public GameObject obj;

    public Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintObjectState()
    {
        Debug.LogFormat("GameObject.activeSelf {0}", obj.activeSelf);
        Debug.LogFormat("GameObject.activeInHierarchy {0}", obj.activeInHierarchy);
    }
    
    public void PrintBehaviourState()
    {
        Debug.LogFormat("Behaviour.enabled {0}", image.enabled);
        Debug.LogFormat("Behaviour.isActiveAndEnabled {0}", image.isActiveAndEnabled);
    }
}
