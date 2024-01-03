using System;
using NodeGraph.Runtime;

namespace NodeGraph.Editor
{
    public class GraphController
    {
        public event Action<IGraphModelData> OnGraphLoaded;
        public event Action<IGraphModelData> OnBeforeGraphLoaded;
        
        public IGraphModelData graphData = null;
        private NodeGraphView m_GraphView;
        
        private bool isLoading = false;
        
        public GraphController(NodeGraphView graphView)
        {
            m_GraphView = graphView;
        }
    }
}