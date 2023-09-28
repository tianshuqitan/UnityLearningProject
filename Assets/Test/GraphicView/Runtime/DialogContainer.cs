using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test.GraphicView.Runtime
{
    [Serializable]
    public class DialogContainer : ScriptableObject
    {
        public List<DialogNodeData> nodeDatas = new List<DialogNodeData>();
        public List<NodeLinkData> linkDatas = new List<NodeLinkData>();
    }
}
