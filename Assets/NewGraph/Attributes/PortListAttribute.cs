using System;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PortListAttribute : PortAttribute
    {
        public PortListAttribute() : base()
        {
        }
    }
}