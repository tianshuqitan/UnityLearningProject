using System;
using System.Collections.Generic;
using UnityEngine;

namespace NewGraph
{
    [Serializable]
    public class SpecialData
    {
        [Port, SerializeReference] public TestNode anotherOutput = null;
        [GraphDisplay(DisplayType.BothViews)] public bool toggle;

        public int test;
    }

    [Serializable, Node]
    public class AnotherTestNode : INode
    {
        [Port, SerializeReference] public TestNode output = null;

        [SerializeReference] public List<TestNode> dataList;


        public SpecialData specialData;
    }
}