using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEditor;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace NewGraph
{
    public class CopyPasteHandler
    {
        private readonly Dictionary<INode, INode> m_OriginalsToClones = new();
        private readonly List<NodeDataInfo> m_Originals = new();
        private readonly List<NodeModel> m_Clones = new();
        private IGraphModelData m_BaseGraphData;

        public bool HasNodes()
        {
            return m_Originals.Count > 0;
        }

        public void CaptureSelection(List<NodeView> nodes, IGraphModelData rootData)
        {
            void AddPortAsExternalReference(NodeDataInfo nodeResolveData, PortView port, NodeView node)
            {
                var relativePath = port.boundProperty.propertyPath.Replace(
                    node.controller.nodeItem.GetSerializedProperty().propertyPath, "");
                relativePath = relativePath.Substring(1, relativePath.Length - 1);

                if (port.boundProperty.managedReferenceValue != null)
                {
                    nodeResolveData.externalReferences.Add(new NodeReference()
                    {
                        nodeData = port.boundProperty.managedReferenceValue,
                        relativePropertyPath = relativePath
                    });
                }
            }

            Clear();

            m_BaseGraphData = rootData;
            foreach (var node in nodes)
            {
                var nodeResolveData = new NodeDataInfo();
                foreach (var outputPort in node.OutputPorts)
                {
                    AddPortAsExternalReference(nodeResolveData, outputPort, node);
                }

                nodeResolveData.baseNodeItem = node.controller.nodeItem;
                m_Originals.Add(nodeResolveData);
            }
            
            foreach (var nodeData in m_Originals)
            {
                for (var i = nodeData.externalReferences.Count - 1; i >= 0; i--)
                {
                    for (var j = m_Originals.Count - 1; j >= 0; j--)
                    {
                        if (nodeData.externalReferences[i].nodeData != m_Originals[j].baseNodeItem.nodeData)
                        {
                            continue;
                        }

                        nodeData.internalReferences.Add(nodeData.externalReferences[i]);
                        nodeData.externalReferences.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void Resolve(IGraphModelData rootData, System.Action<List<NodeModel>> onBeforeAdding,
            System.Action onAfterAdding)
        {
            var originalsCopy = new List<NodeDataInfo>(m_Originals);

            Clear();

            var isSameData = !(m_BaseGraphData != null && m_BaseGraphData != rootData);
            foreach (var original in originalsCopy)
            {
                if (original?.baseNodeItem == null)
                {
                    continue;
                }

                m_Originals.Add(original);
                var deepClone = DeepClone(original.baseNodeItem);
                m_Clones.Add(deepClone);
                m_OriginalsToClones.Add(original.baseNodeItem.nodeData, deepClone.nodeData);
            }

            if (m_Clones.Count <= 0)
            {
                return;
            }

            onBeforeAdding(m_Clones);

            for (var i = 0; i < m_Clones.Count; i++)
            {
                var clone = m_Clones[i];
                var originalResolveData = m_Originals[i];

                rootData.AddNode(clone);
                rootData.SerializedGraphData.ApplyModifiedProperties();

                ResolveExternalReferences(ref originalResolveData.internalReferences,
                    ref originalResolveData.externalReferences, ref isSameData, ref rootData);
            }

            onAfterAdding();
        }

        private void ResolveExternalReferences(ref List<NodeReference> internalReferences,
            ref List<NodeReference> externalReferences, ref bool isSameData, ref IGraphModelData graphData)
        {
            SerializedProperty portReference;

            if (externalReferences.Count <= 0 && internalReferences.Count <= 0)
            {
                return;
            }

            graphData.SerializedGraphData.Update();

            var node = graphData.GetLastAddedNodeProperty(false);
            foreach (var internalReference in internalReferences)
            {
                portReference = node.FindPropertyRelative(internalReference.relativePropertyPath);
                if (portReference == null)
                {
                    continue;
                }

                portReference.managedReferenceValue = m_OriginalsToClones[(INode)internalReference.nodeData];
                graphData.SerializedGraphData.ApplyModifiedProperties();
            }

            foreach (var externalReference in externalReferences)
            {
                portReference = node.FindPropertyRelative(externalReference.relativePropertyPath);
                if (portReference == null)
                {
                    continue;
                }

                portReference.managedReferenceValue = isSameData ? externalReference.nodeData : null;
                graphData.SerializedGraphData.ApplyModifiedProperties();
            }
        }

        private NodeModel DeepClone(NodeModel original)
        {
            var serializedNode =
                SerializationUtility.SerializeValueWeak(original, DataFormat.Binary, out var unityObjects);
            return SerializationUtility.DeserializeValueWeak(serializedNode, DataFormat.Binary, unityObjects) as
                NodeModel;
        }

        private void Clear()
        {
            m_Clones.Clear();
            m_Originals.Clear();
            m_OriginalsToClones.Clear();
        }
    }
}