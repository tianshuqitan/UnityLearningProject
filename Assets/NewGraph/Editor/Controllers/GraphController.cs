using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    public class GraphController
    {
        public event Action<IGraphModelData> OnGraphLoaded;
        public event Action<IGraphModelData> OnBeforeGraphLoaded;

        public IGraphModelData graphData = null;
        public CopyPasteHandler copyPasteHandler = new();
        public GraphView graphView;

        private readonly InspectorControllerBase m_Inspector;
        private readonly ContextMenu m_ContextMenu;
        private readonly EdgeDropMenu m_EdgeDropMenu;
        private readonly Dictionary<Actions, Action<object>> m_InternalActions;
        private readonly Dictionary<object, NodeView> m_DataToViewLookup = new();
        private bool isLoading = false;

        public void ForEachNode(Action<Node> callback)
        {
            graphView.ForEachNodeDo(callback);
        }

        public GraphController(VisualElement uxmlRoot, VisualElement root, Type inspectorType)
        {
            graphView = new GraphView(this, OnGraphAction);
            graphView.viewTransformChanged -= OnViewportChanged;
            graphView.viewTransformChanged += OnViewportChanged;
            
            m_Inspector = Activator.CreateInstance(inspectorType, uxmlRoot) as InspectorControllerBase;
            if (m_Inspector != null)
            {
                m_Inspector.OnShouldLoadGraph = OnShouldLoadGraph;
                m_Inspector.OnAfterGraphCreated = OnAfterGraphCreated;
                m_Inspector.OnHomeClicked = OnHomeClicked;
            }

            Undo.undoRedoPerformed -= Reload;
            Undo.undoRedoPerformed += Reload;

            m_InternalActions = new Dictionary<Actions, Action<object>>()
            {
                { Actions.Paste, OnPaste },
                { Actions.Delete, OnDelete },
                { Actions.Cut, OnCut },
                { Actions.Copy, OnCopy },
                { Actions.Duplicate, OnDuplicate },
                { Actions.EdgeCreate, OnEdgeCreate },
                { Actions.EdgeDelete, OnEdgeDelete },
                { Actions.SelectionChanged, OnSelected },
                { Actions.SelectionCleared, OnDeselected },
                { Actions.EdgeDrop, OnEdgeDrop },
                { Actions.Rename, OnRename },
            };

            m_ContextMenu = ContextMenu.CreateContextMenu(this);
            m_ContextMenu.BuildContextMenu();

            m_EdgeDropMenu = EdgeDropMenu.CreateEdgeDropMenu(this);
        }

        public void OnRename(object obj)
        {
            if (graphView.GetSelectedNodeCount() != 1)
            {
                return;
            }

            var nodeView = graphView.GetFirstSelectedNode();
            if (nodeView == null)
            {
                return;
            }

            foreach (var label in nodeView.editableLabels)
            {
                label.EnableInput(!label.isInEditMode);
                break;
            }
        }

        private void OnEdgeDrop(object edge)
        {
            var baseEdge = (Edge)edge;
            if (baseEdge?.output is not PortView port)
            {
                return;
            }

            m_EdgeDropMenu.port = port;
            m_EdgeDropMenu.BuildContextMenu();
            SearchWindow.Open(m_EdgeDropMenu, graphView);
        }
        
        public void OpenContextMenu()
        {
            graphView.schedule.Execute(() =>
            {
                SearchWindow.Open(m_ContextMenu, graphView);
            });
        }
        
        private void OnHomeClicked()
        {
            graphView.ClearSelection();
            FrameGraph();
        }

        public void FrameGraph(object _ = null)
        {
            graphView.FrameSelected();
        }

        public NodeView GetNodeView(object serializedValue)
        {
            return m_DataToViewLookup.ContainsKey(serializedValue) ? m_DataToViewLookup[serializedValue] : null;
        }

        public void Disable()
        {
            EnsureSerialization();
        }

        public void EnsureSerialization()
        {
            if (graphData?.SerializedGraphData == null)
            {
                return;
            }

            graphData.ForceSerializationUpdate();
            AssetDatabase.SaveAssets();
        }

        private void OnSelected(object data = null)
        {
            Debug.Log("OnSelected");
            m_Inspector.SetInspectorContent(null);
            m_Inspector.SetSelectedNodeInfoActive(active: false);

            var selectedNodesCount = graphView.GetSelectedNodeCount();
            if (selectedNodesCount > 1)
            {
                m_Inspector.SetSelectedNodeInfoActive(selectedNodesCount, graphView.GetSelectedEdgesCount(), true);
            }
            else if (selectedNodesCount > 0)
            {
                var nodeView = graphView.GetFirstSelectedNode();
                if (nodeView != null)
                {
                    m_Inspector.SetInspectorContent(nodeView.GetInspectorContent(), graphData.SerializedGraphData);
                }
            }
        }

        private void OnDeselected(object data = null)
        {
            m_Inspector.SetInspectorContent(null);
            m_Inspector.SetSelectedNodeInfoActive(active: false);
        }

        public void OnCopy(object data = null)
        {
            var nodesToCapture = new List<NodeView>();
            graphView.ForEachSelectedNodeDo((node) =>
            {
                if (node is NodeView scopedNodeView)
                {
                    nodesToCapture.Add(scopedNodeView);
                }
            });
            copyPasteHandler.CaptureSelection(nodesToCapture, graphData);
        }

        public void OnPaste(object data = null)
        {
            copyPasteHandler.Resolve(graphData, (nodes) =>
            {
                Undo.RecordObject(graphData.BaseObject, "Paste Action");
                var viewPosition = graphView.GetMouseViewPosition();
                PositionNodesRelative(viewPosition, nodes);
            }, Reload);
        }

        private void PositionNodesRelative(Vector2 viewPosition, List<NodeModel> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return;
            }

            var bounds = new Bounds(nodes[0].GetPosition(), Vector3.zero);
            foreach (var node in nodes)
            {
                bounds.Encapsulate(node.GetPosition());
            }

            Vector2 refPos = bounds.center;
            var newBasePos = new Vector2(refPos.x + (viewPosition.x - refPos.x),
                refPos.y + (viewPosition.y - refPos.y));

            foreach (var node in nodes)
            {
                var oldPos = node.GetPosition();
                var offset = (refPos - oldPos).normalized * Vector2.Distance(oldPos, refPos);
                var newPos = newBasePos - offset;
                node.SetPosition(newPos.x, newPos.y);
            }
        }

        public void OnDelete(object data = null)
        {
            var isDirty = false;

            graphView.ForEachSelectedEdgeDo((edge) =>
            {
                isDirty = true;
                if (edge.output is PortView outputPort)
                {
                    outputPort.Reset();
                }
            });

            var nodesToRemove = new List<NodeModel>();
            graphView.ForEachSelectedNodeDo((node) =>
            {
                if (node is not NodeView scopedNodeView)
                {
                    return;
                }

                nodesToRemove.Add(scopedNodeView.controller.nodeItem);
                isDirty = true;
            });

            if (nodesToRemove.Count > 0)
            {
                graphView.ForEachPortDo((basePort) =>
                {
                    if (basePort.direction != Direction.Output)
                    {
                        return;
                    }

                    var port = basePort as PortView;
                    if (port != null && (port.boundProperty?.managedReferenceValue == null))
                    {
                        return;
                    }

                    if (nodesToRemove.Any(nodeToRemove =>
                            nodeToRemove.nodeData == port.boundProperty.managedReferenceValue))
                    {
                        port.Reset();
                    }
                });
            }

            if (!isDirty)
            {
                return;
            }

            graphView.Unbind();
            graphData.RemoveNodes(nodesToRemove);
            Reload();
        }

        public void OnCut(object data = null)
        {
            OnCopy();
            OnDelete();
        }

        public void OnDuplicate(object data = null)
        {
            OnCopy();
            OnPaste();
        }

        private void OnEdgeCreate(object data = null)
        {
            var edge = (Edge)data;
            graphView.AddElement(edge);
        }

        private void OnEdgeDelete(object data = null)
        {
            var edge = (Edge)data;
            graphView.RemoveElement(edge);
        }

        private void OnViewportChanged(VisualElement contentContainer)
        {
            if (graphData != null && graphData.BaseObject != null)
            {
                graphData.SetViewport(contentContainer.transform.position, contentContainer.transform.scale);
            }
        }

        public void Reload()
        {
            if (graphData != null)
            {
                Load(graphData);
            }
        }

        private void OnGraphAction(Actions actionType, object data = null)
        {
            if (m_InternalActions.ContainsKey(actionType))
            {
                m_InternalActions[actionType](data);
            }
        }

        public NodeView CreateNewNode(Type nodeType, bool isUtilityNode = false)
        {
            var viewPosition = graphView.GetMouseViewPosition();

            var node = Activator.CreateInstance(nodeType) as INode;
            var nodeItem = graphData.AddNode(node, isUtilityNode);
            nodeItem.SetData(graphData.GetLastAddedNodeProperty(isUtilityNode));

            var nodeController = new NodeController(nodeItem, this, viewPosition);
            var nodeView = nodeController.nodeView;
            nodeView.OnNodeViewSelected = OnSelected;
            graphView.AddElement(nodeView);
            nodeController.Initialize();

            graphView.SetSelected(nodeController.nodeView);

            m_DataToViewLookup.Add(nodeItem.nodeData, nodeController.nodeView);
            return nodeController.nodeView;
        }

        private void OnAfterGraphCreated(IGraphModelData graphData)
        {
            graphView.ClearView();
            this.graphData = graphData;
        }

        private void OnShouldLoadGraph(IGraphModelData graphData)
        {
            m_Inspector.CreateRenameGraphUI(graphData);
            m_Inspector.Clear();
            Load(graphData);
        }

        public void OpenGraphExternal(IGraphModelData baseGraphModel)
        {
            OnShouldLoadGraph(baseGraphModel);
        }

        private void Load(IGraphModelData graphData)
        {
            OnBeforeGraphLoaded?.Invoke(graphData);

            if (isLoading)
            {
                return;
            }

            EnsureSerialization();

            isLoading = true;

            m_Inspector.Clear();
            graphView.ClearView();
            m_DataToViewLookup.Clear();
            m_Inspector.SetSelectedNodeInfoActive(active: false);

            graphView.schedule.Execute(() =>
            {
                this.graphData = graphData;
                if (this.graphData.SerializedGraphData == null)
                {
                    this.graphData.CreateSerializedObject();
                }

                if (this.graphData.ViewportInitiallySet)
                {
                    graphView.UpdateViewTransform(this.graphData.ViewPosition, this.graphData.ViewScale);
                }

                void ForEachNodeProperty(List<NodeModel> nodes, SerializedProperty nodesProperty)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        var node = nodes[i];
                        node.Initialize();

                        var nodeSerializedData = nodesProperty.GetArrayElementAtIndex(i);
                        node.SetData(nodeSerializedData);

                        var nodeController = new NodeController(node, this);
                        var nodeView = nodeController.nodeView;
                        nodeView.OnNodeViewSelected = OnSelected;
                        graphView.AddElement(nodeView);
                        nodeController.Initialize();
                        m_DataToViewLookup.Add(node.nodeData, nodeView);
                    }
                }

                var nodesProperty = this.graphData.GetNodesProperty(false);
                ForEachNodeProperty(this.graphData.Nodes, nodesProperty);

                var utilityNodesProperty = this.graphData.GetNodesProperty(true);
                ForEachNodeProperty(this.graphData.UtilityNodes, utilityNodesProperty);

                graphView.ForEachNodeDo((node) =>
                {
                    if (node is not NodeView nodeView)
                    {
                        return;
                    }

                    foreach (var port in nodeView.OutputPorts)
                    {
                        var value = port.boundProperty.managedReferenceValue;
                        if (value == null || !m_DataToViewLookup.ContainsKey(value))
                        {
                            continue;
                        }

                        var otherView = m_DataToViewLookup[value];
                        if (otherView.controller.nodeItem.nodeData == value)
                        {
                            ConnectPorts(port, otherView.InputPort);
                        }
                    }
                });

                isLoading = false;
                OnGraphLoaded?.Invoke(this.graphData);
            });
        }

        public void ConnectPorts(PortView input, PortView output)
        {
            graphView.ConnectPorts(input, output);
        }

        public void Draw()
        {
            SearchWindow.targetPosition = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            m_Inspector.Draw();
        }
    }
}