using System.Collections.Generic;

namespace NewGraph
{
    public class NodeDataInfo
    {
        public List<NodeReference> externalReferences = new();
        public List<NodeReference> internalReferences = new();
        public NodeModel baseNodeItem;
    }
}