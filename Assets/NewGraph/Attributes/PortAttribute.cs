using System;
using UnityEditor.Experimental.GraphView;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PortAttribute : PortBaseAttribute
    {
        public PortAttribute(string name = null) : base(name, Port.Capacity.Single, Direction.Output)
        {
        }
    }
}