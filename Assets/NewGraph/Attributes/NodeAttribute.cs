using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public bool createInputPort = true;

        public Color color = default;

        public string nodeName = null;

        public Port.Capacity inputPortCapacity = Port.Capacity.Multi;

        public string categories = "";

        public string inputPortName = null;

        public NodeAttribute(string color = null, string categories = "", string inputPortName = null,
            Port.Capacity inputPortCapacity = Port.Capacity.Multi, string nodeName = null, bool createInputPort = true)
        {
            if (color != null)
            {
                ColorUtility.TryParseHtmlString(color, out this.color);
            }

            if (nodeName != null)
            {
                this.nodeName = nodeName;
            }

            this.inputPortName = inputPortName;
            this.inputPortCapacity = inputPortCapacity;
            this.categories = categories;
            this.createInputPort = createInputPort;
        }
        
        public string GetName(Type nodeType)
        {
            return !string.IsNullOrEmpty(nodeName) ? nodeName : nodeType.Name;
        }
    }
}