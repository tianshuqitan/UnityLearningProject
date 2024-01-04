using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NewGraph
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly GraphViewChange m_GraphViewChange;
        private readonly List<Edge> m_EdgesToCreate;
        private readonly List<GraphElement> m_EdgesToDelete;

        public EdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            
        }

        public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            
            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (var connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (var connection in edge.output.connections)
                {
                    if (connection != edge)
                    {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }
            
            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }
            
            var edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }
            
            foreach (var edge1 in edgesToCreate)
            {
                graphView.AddElement(edge1);
                edge.input.Connect(edge1);
                edge.output.Connect(edge1);
            }
        }
    }
}
