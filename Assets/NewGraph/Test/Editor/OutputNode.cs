using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NewGraph.Test.Editor
{
    [Serializable, Node]
    public class OutputNode : INode
    {
        [Port, SerializeReference] public InputNode node = null;
    }
}