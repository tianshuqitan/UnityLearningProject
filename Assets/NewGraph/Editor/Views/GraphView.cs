using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NewGraph
{
    public class GraphView : UnityEditor.Experimental.GraphView.GraphView
    {
        private GraphController m_Controller;
        private readonly Action<Actions, object> OnAction;
        private readonly Dictionary<Actions, Action> internalActions = null;
        public ShortcutHandler shortcutHandler = null;

        public GraphView(GraphController controller, Action<Actions, object> OnAction)
        {
            m_Controller = controller;
            style.flexGrow = 1;
            Insert(0, new GridBackground() { name = "grid_background" });

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            nodeCreationRequest = OnNodeCreationRequest;

            GraphWindow.OnGlobalKeyDown -= OnKeyDown;
            GraphWindow.OnGlobalKeyDown += OnKeyDown;

            UnregisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseDownEvent>(OnMouseDown);

            this.OnAction = OnAction;

            shortcutHandler = new ShortcutHandler();
            internalActions = new Dictionary<Actions, Action>()
            {
                { Actions.Frame, FrameSelected },
            };

            graphViewChanged -= OnGraphViewChanged;
            graphViewChanged += OnGraphViewChanged;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button != 1)
            {
                return;
            }

            var isClickInElement = graphElements.ToList().Any(element =>
                element.ContainsPoint(element.WorldToLocal(Event.current.mousePosition)));
            if (isClickInElement == false)
            {
                m_Controller.OpenContextMenu();
            }
        }

        private void OnKeyDown(Event evt)
        {
            ExecuteShortcutHandler(evt.keyCode, evt.modifiers);
        }

        private void ExecuteShortcutHandler(KeyCode keyCode, EventModifiers modifiers)
        {
            if (panel.GetCapturingElement(PointerId.mousePointerId) != null)
            {
                return;
            }

            var keyAction = shortcutHandler.Execute(keyCode, modifiers);
            if (internalActions.ContainsKey(keyAction))
            {
                internalActions[keyAction]();
            }

            OnActionExecuted(keyAction);
        }

        private Vector2 LocalToViewTransformPosition(Vector2 localMousePosition)
        {
            return new Vector2(
                (localMousePosition.x - contentContainer.transform.position.x) / contentContainer.transform.scale.x,
                (localMousePosition.y - contentContainer.transform.position.y) / contentContainer.transform.scale.y);
        }


        public Vector2 GetMouseViewPosition()
        {
            return LocalToViewTransformPosition(this.WorldToLocal(Event.current.mousePosition));
        }

        protected override void ExecuteDefaultAction(EventBase baseEvent)
        {
            //base.ExecuteDefaultAction(baseEvent);
        }

        public void SetSelected(GraphElement graphElement, bool selected = true, bool clear = true)
        {
            if (clear)
            {
                ClearSelection();
            }

            graphElement.selected = selected;
            OnActionExecuted(Actions.SelectionChanged, graphElement);
        }

        public void ClearView()
        {
            foreach (var node in nodes)
            {
                node.RemoveFromHierarchy();
            }

            foreach (var port in ports)
            {
                port.RemoveFromHierarchy();
            }

            foreach (var edge in edges)
            {
                edge.RemoveFromHierarchy();
            }
        }

        public NodeView GetFirstSelectedNode()
        {
            var selectionList = selection.ToList();
            foreach (var element in selectionList)
            {
                if (element is NodeView node)
                {
                    return node;
                }
            }

            return null;
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            OnActionExecuted(Actions.SelectionCleared);
        }

        private List<T> GetSelected<T>()
        {
            var tmpList = new List<T>();
            selection.ToList().ForEach(selectable =>
            {
                if (selectable is T tmp)
                {
                    tmpList.Add(tmp);
                }
            });
            return tmpList;
        }

        private int GetSelectedCount<T>()
        {
            return GetSelected<T>().Count;
        }

        public int GetSelectedNodeCount()
        {
            return GetSelectedCount<NodeView>();
        }

        public int GetSelectedEdgesCount()
        {
            return GetSelectedCount<Edge>();
        }

        public bool HasSelectedEdges()
        {
            return GetSelectedEdgesCount() > 0 ? true : false;
        }

        public void ForEachPortDo(Action<Port> callback)
        {
            foreach (var port in ports)
            {
                callback(port);
            }
        }

        public void FrameSelected()
        {
            FrameSelection();
        }

        public List<Port> GetPorts()
        {
            return ports.ToList();
        }

        public void ForEachNodeDo(Action<Node> callback)
        {
            foreach (var node in nodes)
            {
                callback(node);
            }
        }

        public void ForEachSelectedNodeDo(Action<Node> callback)
        {
            foreach (var node in GetSelected<Node>())
            {
                callback(node);
            }
        }

        public void ForEachSelectedEdgeDo(Action<Edge> callback)
        {
            foreach (var edge in GetSelected<Edge>())
            {
                callback(edge);
            }
        }

        private void OnActionExecuted(Actions actionType, object data = null)
        {
            OnAction(actionType, data);
        }

        public Edge CreateEdge()
        {
            return new Edge();
        }

        public void ConnectPorts(Port input, Port output)
        {
            AddElement(input.ConnectTo(output));
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
            }

            if (graphViewChange.edgesToCreate != null)
            {
                foreach (var edge in graphViewChange.edgesToCreate)
                {
                }
            }

            if (graphViewChange.movedElements != null)
            {
                foreach (var element in graphViewChange.movedElements)
                {
                    switch (element)
                    {
                        case NodeView nodeView:
                            nodeView.controller.SetPosition(nodeView.GetPosition().x, nodeView.GetPosition().y);
                            break;
                        case Edge edge:
                            break;
                    }
                }
            }

            return graphViewChange;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach((port =>
            {
                if (startPort.node != port.node && startPort.direction != port.direction)
                {
                    compatiblePorts.Add(port);
                }
            }));
            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
        }
        
        private void OnNodeCreationRequest(NodeCreationContext c)
        {
            
        }
    }
}