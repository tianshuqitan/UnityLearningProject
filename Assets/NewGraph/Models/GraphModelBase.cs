using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NewGraph
{
    [Serializable]
    public class GraphModelBase
    {
        [SerializeReference, HideInInspector] public List<NodeModel> nodes = new();

#if UNITY_EDITOR
        [SerializeReference, HideInInspector] public List<NodeModel> utilityNodes = new();

        [NonSerialized] public SerializedObject serializedGraphData;

        [NonSerialized] public UnityEngine.Object baseObject;

        [NonSerialized] private SerializedProperty nodesProperty = null;

        [NonSerialized] private SerializedProperty utilityNodesProperty = null;

        [SerializeField, HideInInspector] private string tmpName;
        private SerializedProperty tmpNameProperty = null;
        private SerializedProperty originalNameProperty = null;

        [NonSerialized] private string basePropertyPath = null;

        private SerializedProperty GetProperty(string propertyToSearch)
        {
            return serializedGraphData.FindProperty(basePropertyPath + "." + propertyToSearch);
        }

        [SerializeField, HideInInspector] private bool viewportInitiallySet = false;
        public bool ViewportInitiallySet => viewportInitiallySet;

        private SerializedProperty viewportInitiallySetProperty = null;

        private SerializedProperty ViewportInitiallySetProperty
        {
            get
            {
                if (viewportInitiallySetProperty != null)
                {
                    return viewportInitiallySetProperty;
                }

                viewportInitiallySetProperty = GetProperty(nameof(viewportInitiallySet));
                return viewportInitiallySetProperty;
            }
        }

        [SerializeField, HideInInspector] private Vector3 viewPosition;
        public Vector3 ViewPosition => viewPosition;

        [NonSerialized] private SerializedProperty viewPositionProperty = null;

        private SerializedProperty ViewPositionProperty
        {
            get
            {
                if (viewPositionProperty != null)
                {
                    return viewPositionProperty;
                }

                viewPositionProperty = GetProperty(nameof(viewPosition));
                return viewPositionProperty;
            }
        }

        [SerializeField, HideInInspector] private Vector3 viewScale;
        public Vector3 ViewScale => viewScale;

        [NonSerialized] private SerializedProperty viewScaleProperty = null;

        private SerializedProperty ViewScaleProperty
        {
            get
            {
                if (viewScaleProperty != null)
                {
                    return viewScaleProperty;
                }

                viewScaleProperty = GetProperty(nameof(viewScale));
                return viewScaleProperty;
            }
        }

        public void SetViewport(Vector3 position, Vector3 scale)
        {
            if (position == viewPosition && scale == viewScale)
            {
                return;
            }

            ViewportInitiallySetProperty.boolValue = true;
            ViewScaleProperty.vector3Value = scale;
            ViewPositionProperty.vector3Value = position;
            serializedGraphData.ApplyModifiedProperties();
        }

        public NodeModel AddNode(INode node, bool isUtilityNode, UnityEngine.Object scope)
        {
            var baseNodeItem = new NodeModel(node)
            {
                isUtilityNode = isUtilityNode
            };

            if (!isUtilityNode)
            {
                nodes.Add(baseNodeItem);
            }
            else
            {
                utilityNodes.Add(baseNodeItem);
            }

            ForceSerializationUpdate(scope);
            return baseNodeItem;
        }

        public NodeModel AddNode(NodeModel nodeItem)
        {
            if (!nodeItem.isUtilityNode)
            {
                nodes.Add(nodeItem);
            }
            else
            {
                utilityNodes.Add(nodeItem);
            }

            return nodeItem;
        }

        public void RemoveNode(NodeModel node)
        {
            if (!node.isUtilityNode)
            {
                nodes.Remove(node);
            }
            else
            {
                utilityNodes.Remove(node);
            }
        }

        public void RemoveNodes(List<NodeModel> nodesToRemove, UnityEngine.Object scope)
        {
            if (nodesToRemove.Count <= 0)
            {
                return;
            }

            Undo.RecordObject(scope, "Remove Nodes");
            foreach (var node in nodesToRemove)
            {
                RemoveNode(node);
            }
        }

        public void ForceSerializationUpdate(UnityEngine.Object scope)
        {
            serializedGraphData.Update();
            EditorUtility.SetDirty(scope);
            serializedGraphData.ApplyModifiedProperties();
        }

        public void CreateSerializedObject(UnityEngine.Object scope, string rootFieldName)
        {
            basePropertyPath = rootFieldName;
            baseObject = scope;
            serializedGraphData = new SerializedObject(scope);
            nodesProperty = null;
            utilityNodesProperty = null;
            viewPositionProperty = null;
            viewScaleProperty = null;
            viewportInitiallySetProperty = null;
        }

        public SerializedProperty GetOriginalNameProperty()
        {
            if (originalNameProperty != null)
            {
                return originalNameProperty;
            }

            originalNameProperty = serializedGraphData.FindProperty("m_Name");
            return originalNameProperty;
        }

        public SerializedProperty GetTmpNameProperty()
        {
            if (tmpNameProperty != null)
            {
                return tmpNameProperty;
            }

            tmpNameProperty = GetProperty(nameof(tmpName));
            return tmpNameProperty;
        }

        public SerializedProperty GetNodesProperty(bool isUtilityNode)
        {
            if (!isUtilityNode)
            {
                if (nodesProperty != null)
                {
                    return nodesProperty;
                }

                nodesProperty = GetProperty(nameof(nodes));
                return nodesProperty;
            }

            if (utilityNodesProperty != null)
            {
                return utilityNodesProperty;
            }

            utilityNodesProperty = GetProperty(nameof(utilityNodes));
            return utilityNodesProperty;
        }

        public SerializedProperty GetLastAddedNodeProperty(bool isUtilityNode)
        {
            return isUtilityNode
                ? GetNodesProperty(true).GetArrayElementAtIndex(utilityNodes.Count - 1)
                : GetNodesProperty(false).GetArrayElementAtIndex(nodes.Count - 1);
        }
#endif
    }
}