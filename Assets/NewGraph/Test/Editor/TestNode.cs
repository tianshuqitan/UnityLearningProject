using System;

namespace NewGraph
{
    [Serializable, Node]
    public class TestNode : INode
    {
        [GraphDisplay(DisplayType.BothViews)] public int number1;
        [GraphDisplay(DisplayType.NodeView)] public int number2;
    }
}