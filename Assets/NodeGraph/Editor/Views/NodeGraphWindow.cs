using System;
using NodeGraph.Runtime;
using UnityEditor;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class NodeGraphWindow : EditorWindow
    {
        [NonSerialized] private static bool s_LoadRequested = false;

        public VisualTreeAsset m_GraphDocument;
        private GraphController m_GraphController;
        private NodeGraphWindowController m_WindowController;
        
        private void CreateGUI()
        {
            VisualElement element = m_GraphDocument.CloneTree();
            rootVisualElement.Add(element);
            element.StretchToParentSize();
            
            m_WindowController = new NodeGraphWindowController(rootVisualElement)
            {
                OnAfterGraphCreated = OnAfterGraphCreated,
                OnShouldLoadGraph = OnShouldLoadGraph
            };

            m_GraphController = new GraphController(rootVisualElement.Q<NodeGraphView>("GraphView"));
            
            rootVisualElement.schedule.Execute((() =>
            {
                if (!s_LoadRequested)
                {
                    LoadNodeGraph();
                }

                s_LoadRequested = false;
            }));
        }

        private void OnGUI()
        {
            m_WindowController?.OnGraphAssetSelectFromPicker();
        }

        private void OnDisable()
        {
            s_LoadRequested = false;
        }

        private void OnDestroy()
        {
        }
        
        private void LoadNodeGraph(IGraphModelData graphModel = null)
        {
            if (graphModel != null)
            {
            }

            s_LoadRequested = true;
        }

        private void OnAfterGraphCreated(IGraphModelData graphModel)
        {
        }

        private void OnShouldLoadGraph(IGraphModelData graphModel)
        {
        }
    }
}