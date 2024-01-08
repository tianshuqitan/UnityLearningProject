using System;

namespace NewGraph
{
    [Serializable, Node]
    public class TestNode : INode
    {
        [GraphDisplay(DisplayType.BothViews)] public int number1;
        [GraphDisplay(DisplayType.NodeView)] public int number2;
        [GraphDisplay(DisplayType.Inspector)] public int number3;
        [GraphDisplay(DisplayType.NodeView)] public int number4;
        [GraphDisplay(DisplayType.Hide)] public int number5;
    }
}