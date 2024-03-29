﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    public class NodeController
    {
        public GraphController graphController;
        public NodeView nodeView;
        public NodeModel nodeItem;
        private Vector2 startPosition = Vector2.zero;
        private PropertyBag propertyBag;
        private SerializedObject serializedObject;
        private SerializedProperty nodeDataProperty;
        
        public float GetViewScale() {
            return graphController.graphView.scale;
        }

        public void ForEachNode(Action<Node> callback)
        {
            graphController.ForEachNode(callback);
        }

        public NodeController(NodeModel nodeItem, GraphController graphController)
        {
            this.graphController = graphController;
            this.nodeItem = nodeItem;
            this.nodeDataProperty = nodeItem.GetSpecificSerializedProperty();
            this.propertyBag =
                PropertyBag.GetCachedOrCreate(nodeItem.nodeAttribute, nodeItem.nodeType, nodeDataProperty);
            this.serializedObject = graphController.graphData.SerializedGraphData;
            this.nodeView = new NodeView(this, nodeItem.nodeAttribute.color);

            string name = nodeItem.GetName();
            if (string.IsNullOrEmpty(name))
            {
                nodeItem.SetName(nodeItem.nodeAttribute.GetName(nodeItem.nodeType));
            }
        }

        public NodeController(NodeModel node, GraphController graphController, Vector2 startPosition) : this(node,
            graphController)
        {
            this.startPosition = startPosition;
        }

        public SerializedObject GetSerializedObject()
        {
            return serializedObject;
        }

        public void Initialize()
        {
            var customEditor = NodeEditor.CreateEditor(nodeItem.nodeType);
            if (customEditor != null)
            {
                nodeView.shouldSetBackgroundColor = customEditor.ShouldSetBackgroundColor();
                nodeView.InitializeView();
                customEditor.Initialize(this);
            }
            else
            {
                nodeView.InitializeView();
            }
        }

        public void SetPosition(float xMin, float yMin)
        {
            nodeItem.SetPosition(xMin, yMin);
        }

        public Vector2 GetStartPosition()
        {
            return startPosition == Vector2.zero ? nodeItem.GetPosition() : startPosition;
        }

        public void DoForEachPropertyOrGroup(VisualElement[] parents,
            Func<GroupInfo, VisualElement[], SerializedProperty, VisualElement[]> groupCreation,
            Action<VisualElement[], GraphPropertyInfo, SerializedProperty> propCreation)
        {
            DoForEachPropertyOrGroupRecursive(parents, propertyBag.graphPropertiesAndGroups, groupCreation,
                propCreation);
        }

        private void DoForEachPropertyOrGroupRecursive(VisualElement[] parents,
            List<PropertyInfo> propertiesIncludingGroups,
            Func<GroupInfo, VisualElement[], SerializedProperty, VisualElement[]> groupCreation,
            Action<VisualElement[], GraphPropertyInfo, SerializedProperty> propCreation)
        {
            foreach (var groupOrProperty in propertiesIncludingGroups)
            {
                if (groupOrProperty.GetType() == typeof(GroupInfo))
                {
                    var groupInfo = (GroupInfo)groupOrProperty;
                    if (!groupInfo.graphDisplay.createGroup)
                    {
                        propCreation(parents, (GraphPropertyInfo)groupOrProperty,
                            nodeDataProperty.FindPropertyRelative(groupOrProperty.relativePropertyPath));
                    }
                    else
                    {
                        if (groupInfo.graphProperties.Count > 0)
                        {
                            var groupParents = groupCreation(groupInfo, parents,
                                nodeDataProperty.FindPropertyRelative(groupOrProperty.relativePropertyPath));
                            DoForEachPropertyOrGroupRecursive(groupParents, groupInfo.graphProperties, groupCreation,
                                propCreation);
                        }
                    }
                }
                else
                {
                    propCreation(parents, (GraphPropertyInfo)groupOrProperty,
                        nodeDataProperty.FindPropertyRelative(groupOrProperty.relativePropertyPath));
                }
            }
        }

        private void DoForEachPortPropertyBase(ref List<PortInfo> portList, Action<PortInfo, SerializedProperty> action)
        {
            foreach (var info in portList)
            {
                action(info, nodeDataProperty.FindPropertyRelative(info.relativePropertyPath));
            }
        }

        public void DoForOutputPortProperty(Action<PortInfo, SerializedProperty> action)
        {
            DoForEachPortPropertyBase(ref propertyBag.ports, action);
        }

        public void DoForEachPortListProperty(Action<PortInfo, SerializedProperty> action)
        {
            DoForEachPortPropertyBase(ref propertyBag.portLists, action);
        }

        public void DoForInputPortProperty(Action<PortInfo, SerializedProperty> action)
        {
            if (propertyBag.inputPort != null)
            {
                action(propertyBag.inputPort, nodeDataProperty);
            }
        }

        public void DoForNameProperty(Action<SerializedProperty> action)
        {
            action(nodeItem.GetNameSerializedProperty());
        }
    }
}