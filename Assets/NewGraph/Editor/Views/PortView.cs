using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NewGraph
{
    using static GraphSettingsSingleton;

    public class PortView : Port
    {
        private UnityEngine.Color DisabledPortColor => Settings.disabledPortColor;
        private UnityEngine.Color DefaultPortColor => Settings.portColor;
        public SerializedProperty boundProperty;
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
            var port = new PortView(Orientation.Horizontal, direction, capacity, info.fieldType)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener),
                m_IsValidConnectionCheck = info.portDisplay.isValidConnectionCheck,
                ConnectableTypes = info.connectableTypes,
                boundProperty = boundProperty,
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

            if (outputPort.boundProperty.managedReferenceValue != inputPort.boundProperty.managedReferenceValue)
            {
                outputPort.boundProperty.managedReferenceId = inputPort.boundProperty.managedReferenceId;
                outputPort.boundProperty.serializedObject.ApplyModifiedProperties();
                m_ConnectionChangedCallback?.Invoke();
            }

            // ColorizeEdgeAndPort(edge);
        }

        public void Reset()
        {
            if (boundProperty == null)
            {
                return;
            }

            boundProperty.managedReferenceValue = null;
            boundProperty.serializedObject.ApplyModifiedProperties();
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