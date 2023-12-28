using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Test.GraphicView.Runtime
{
    [Serializable]
    public class DialogNodeData
    {
        [Serializable]
        public class Info
        {
            public int number;
        }
        
        [Title("TitleTest"), LabelText("名称")]
        public string name;
        public string guid;
        public string dialogText;
        public Vector2 position;
        public int testNum;
        public bool testBool;
        public string testString;
        public float testFloat;
        public Info info;

        [Button("TestButton")]
        private void TestButton()
        {
            Debug.Log("TestButton");
        }
    }
}