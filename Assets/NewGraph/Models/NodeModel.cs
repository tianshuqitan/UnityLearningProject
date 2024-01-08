using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif

namespace NewGraph
{
    [Serializable]
    public class NodeModel
    {
        [SerializeReference] public INode nodeData;

#if UNITY_EDITOR
        [Serializable]
        public class FoldoutState
        {
            public int relativePropertyPathHash;
            [NonSerialized] public bool used = false;
            public bool isExpanded = true;
        }

        [SerializeField] private float nodeX, nodeY;
        [SerializeField] private string name;
        [SerializeField] public bool isExpanded = false;
        [SerializeField] private List<FoldoutState> foldouts = new();

        public const string nameIdentifier = nameof(name);

        public bool isUtilityNode = false;

        private Dictionary<int, FoldoutState> foldoutsLookup = null;

        private Dictionary<int, FoldoutState> FoldoutsLookup
        {
            get
            {
                if (foldoutsLookup != null)
                {
                    return foldoutsLookup;
                }

                foldoutsLookup = new Dictionary<int, FoldoutState>();
                foreach (var foldoutState in foldouts)
                {
                    foldoutsLookup.Add(foldoutState.relativePropertyPathHash, foldoutState);
                }

                return foldoutsLookup;
            }
        }

        [NonSerialized] private bool dataIsSet = false;
        [NonSerialized] private SerializedProperty serializedProperty;
        [NonSerialized] private SerializedProperty nodeDataSerializedProperty;
        [NonSerialized] private SerializedProperty nameProperty;
        [NonSerialized] private SerializedProperty nodeXProperty;
        [NonSerialized] private SerializedProperty nodeYProperty;
        [NonSerialized] private static Dictionary<Type, NodeAttribute> nodeInfo = new();
        [NonSerialized] public Type nodeType;
        [NonSerialized] public NodeAttribute nodeAttribute;

        public NodeModel(INode nodeData)
        {
            this.nodeData = nodeData;
            Initialize();
        }

        public FoldoutState GetOrCreateFoldout(int pathHash, bool defaultState = true)
        {
            if (FoldoutsLookup.ContainsKey(pathHash))
            {
                return FoldoutsLookup[pathHash];
            }

            var foldoutState = new FoldoutState()
                { relativePropertyPathHash = pathHash, isExpanded = defaultState };
            FoldoutsLookup.Add(pathHash, foldoutState);
            foldouts.Add(foldoutState);

            return FoldoutsLookup[pathHash];
        }

        public void CleanupFoldoutStates()
        {
            for (var i = foldouts.Count - 1; i >= 0; i--)
            {
                var state = foldouts[i];
                if (!state.used || state.relativePropertyPathHash == default)
                {
                    foldouts.RemoveAt(i);
                }
            }
        }

        public Vector2 GetPosition()
        {
            return new Vector2(nodeX, nodeY);
        }

        public string GetName()
        {
            return name;
        }

        public static NodeAttribute GetNodeAttribute(Type type)
        {
            if (!nodeInfo.ContainsKey(type))
            {
                nodeInfo.Add(type, Attribute.GetCustomAttribute(type, typeof(NodeAttribute)) as NodeAttribute);
            }

            return nodeInfo[type];
        }

        public void Initialize()
        {
            nodeType = nodeData.GetType();
            nodeAttribute = GetNodeAttribute(nodeType);
        }

        public string SetName(string name)
        {
            this.name = name;
            if (dataIsSet)
            {
                nameProperty.serializedObject.Update();
            }

            return name;
        }

        public void SetPosition(float positionX, float positionY)
        {
            if (dataIsSet)
            {
                if (positionX != nodeX || positionY != nodeY)
                {
                    nodeXProperty.floatValue = positionX;
                    nodeYProperty.floatValue = positionY;
                    nodeXProperty.serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                nodeX = positionX;
                nodeY = positionY;
            }
        }

        public SerializedProperty GetSerializedProperty()
        {
            return serializedProperty;
        }

        public SerializedProperty GetSpecificSerializedProperty()
        {
            return nodeDataSerializedProperty;
        }

        public SerializedProperty GetNameSerializedProperty()
        {
            return nameProperty;
        }

        public void SetData(SerializedProperty serializedProperty)
        {
            this.serializedProperty = serializedProperty;
            nodeDataSerializedProperty = serializedProperty.FindPropertyRelative(nameof(nodeData));
            nameProperty = serializedProperty.FindPropertyRelative(nameof(name));
            nodeXProperty = serializedProperty.FindPropertyRelative(nameof(nodeX));
            nodeYProperty = serializedProperty.FindPropertyRelative(nameof(nodeY));
            dataIsSet = true;
        }
#endif
    }
}