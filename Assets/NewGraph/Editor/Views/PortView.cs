using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    using static GraphSettingsSingleton;

    public class PortView : Port
    {
        private UnityEngine.Color DisabledPortColor => Settings.disabledPortColor;
        private UnityEngine.Color DefaultPortColor => Settings.portColor;
        public SerializedProperty m_BoundProperty;
        private Action m_ConnectionChangedCallback;
        private Func<Type, Type, bool> m_IsValidConnectionCheck;
        private UnityEngine.Color targetColor;

        public List<Type> ConnectableTypes;

        private PortView(Orientation orientation, Direction direction, Capacity capacity, Type type) : base(orientation,
            direction, capacity, type)
        {
            portColor = DefaultPortColor;
        }

        public static PortView Create<TEdge>(PortInfo info, SerializedProperty boundProperty,
            Action changedCallback = null) where TEdge : Edge, new()
        {
            var direction = (Direction)(int)(info.portDisplay.direction);
            var capacity = (Capacity)(int)(info.portDisplay.capacity);
            var listener = new EdgeConnectorListener();
            var port = new PortView(Orientation.Horizontal, direction, capacity, typeof(Node))
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener),
                m_IsValidConnectionCheck = info.portDisplay.isValidConnectionCheck,
                ConnectableTypes = info.connectableTypes,
                m_BoundProperty = boundProperty,
                m_ConnectionChangedCallback = changedCallback,
            };
            port.AddManipulator(port.m_EdgeConnector);

            return port;
        }

        public void SetConnectionChangedCallback(Action callback)
        {
            m_ConnectionChangedCallback = callback;
            m_ConnectionChangedCallback();
        }

        public override void Connect(Edge edge)
        {
            base.Connect(edge);
            if (edge.input == null || edge.output == null)
            {
                return;
            }

            var inputPort = edge.input as PortView;
            var outputPort = edge.output as PortView;

            if (outputPort.m_BoundProperty.managedReferenceValue != inputPort.m_BoundProperty.managedReferenceValue)
            {
                outputPort.m_BoundProperty.managedReferenceId = inputPort.m_BoundProperty.managedReferenceId;
                outputPort.m_BoundProperty.serializedObject.ApplyModifiedProperties();
                m_ConnectionChangedCallback?.Invoke();
            }

            // ColorizeEdgeAndPort(edge);
        }

        public void Reset()
        {
            if (m_BoundProperty == null)
            {
                return;
            }

            m_BoundProperty.managedReferenceValue = null;
            m_BoundProperty.serializedObject.ApplyModifiedProperties();
            m_ConnectionChangedCallback?.Invoke();
        }

        public bool CanConnectTo(Port other, bool ignoreCandidateEdges = true)
        {
            PortView outputPort;
            PortView inputPort;
        
            if (direction == Direction.Output)
            {
                outputPort = this;
                inputPort = (PortView)other;
            }
            else
            {
                outputPort = (PortView)other;
                inputPort = this;
            }
        
            if (!m_IsValidConnectionCheck(inputPort.portType, outputPort.portType))
            {
                return false;
            }
        
            return true;
        }

        private void ColorizeEdgeAndPort(Edge edge)
        {
            var nodeType = (edge.input as PortView)?.portType;
            targetColor = NodeModel.GetNodeAttribute(nodeType).color;
            if (edge.input == null)
            {
                return;
            }

            targetColor = (targetColor == default) ? edge.input.portColor : targetColor;
            edge.input.portColor = targetColor;
            if (edge.output != null)
            {
                edge.output.portColor = targetColor;
            }
        }
    }
}