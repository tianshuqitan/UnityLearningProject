﻿using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NewGraph
{
    public class MonoGraphModel : MonoBehaviour, IGraphModelData
    {
        [SerializeField, HideInInspector] private GraphModelBase baseModel;

        [SerializeField, HideInInspector] private int id;

        public List<NodeModel> Nodes => baseModel.nodes;

#if UNITY_EDITOR
        private const string lastOpenedMonoGraphPrefsKey = nameof(NewGraph) + "." + nameof(lastOpenedMonoGraphPrefsKey);

        public List<NodeModel> UtilityNodes => baseModel.utilityNodes;

        public SerializedObject SerializedGraphData
        {
            get
            {
                if (baseModel.serializedGraphData == null)
                {
                    CreateSerializedObject();
                }

                return baseModel.serializedGraphData;
            }
        }

        public bool ViewportInitiallySet => baseModel.ViewportInitiallySet;

        public Vector3 ViewPosition => baseModel.ViewPosition;

        public Vector3 ViewScale => baseModel.ViewScale;

        public UnityEngine.Object BaseObject
        {
            get
            {
                if (baseModel.baseObject == null)
                {
                    CreateSerializedObject();
                }

                return baseModel.baseObject;
            }
        }

        public string GUID => CreateID().ToString();

        public NodeModel AddNode(INode node, bool isUtilityNode)
        {
            return baseModel.AddNode(node, isUtilityNode, this);
        }

        public NodeModel AddNode(NodeModel nodeItem)
        {
            return baseModel.AddNode(nodeItem);
        }

        public void CreateSerializedObject()
        {
            baseModel.CreateSerializedObject(this, nameof(baseModel));
        }

        public void ForceSerializationUpdate()
        {
            if (this != null)
            {
                baseModel.ForceSerializationUpdate(this);
            }
        }

        public SerializedProperty GetLastAddedNodeProperty(bool isUtilityNode)
        {
            return baseModel.GetLastAddedNodeProperty(isUtilityNode);
        }

        public SerializedProperty GetNodesProperty(bool isUtilityNode)
        {
            return baseModel.GetNodesProperty(isUtilityNode);
        }

        public SerializedProperty GetTmpNameProperty()
        {
            return baseModel.GetTmpNameProperty();
        }

        public SerializedProperty GetOriginalNameProperty()
        {
            return baseModel.GetOriginalNameProperty();
        }

        public void RemoveNode(NodeModel node)
        {
            baseModel.RemoveNode(node);
        }

        public void RemoveNodes(List<NodeModel> nodesToRemove)
        {
            baseModel.RemoveNodes(nodesToRemove, this);
        }

        public void SetViewport(Vector3 position, Vector3 scale)
        {
            baseModel.SetViewport(position, scale);
        }

        private int CreateID()
        {
            if (id == 0)
            {
                id = Guid.NewGuid().ToString("D").GetHashCode();
            }

            return id;
        }

        protected virtual void OnValidate()
        {
            CreateID();
        }

        public static IGraphModelData GetGraphData(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            var graphModels =
                FindObjectsByType<MonoGraphModel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            return graphModels.FirstOrDefault(gm => gm.GUID == guid);
        }
#endif
    }
}