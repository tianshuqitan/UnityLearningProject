using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NewGraph
{
    using static GraphSettingsSingleton;

    public class PropertyBag
    {
        public PortInfo inputPort = null;
        public List<PortInfo> ports = new();
        public List<PortInfo> portLists = new();
        public List<PropertyInfo> graphPropertiesAndGroups = new();
        private AttributesBag attributeBag = new();
        private Type nodeType;

        private static Dictionary<Type, PropertyBag> propertyBagLookup = new();
        private List<GroupInfo> groupInfoLookup = new();
        private Dictionary<Type, Func<int>> attributebehaviors = null;

        private string currentRelativePropertyPath;
        private object currentAttribute;
        private GraphDisplayAttribute currentGraphDisplayAttribute = null;
        private SerializedProperty currentProperty;

        private PropertyBag(NodeAttribute nodeAttribute, Type nodeType, SerializedProperty nodeProperty)
        {
            this.nodeType = nodeType;

            if (nodeAttribute.createInputPort)
            {
                inputPort = new PortInfo(nodeProperty.propertyPath, nodeType,
                    new PortBaseAttribute(nodeAttribute.inputPortName, nodeAttribute.inputPortCapacity,
                        Direction.Input), Settings.defaultInputName);
            }

            InitializeAttributeBehaviors();
            RetrieveAll(nodeProperty);
        }

        private void InitializeAttributeBehaviors()
        {
            attributebehaviors = new Dictionary<Type, Func<int>>()
            {
                { typeof(HideInInspector), GetHideInInspectorAttribute },
                { typeof(PortBaseAttribute), GetPortAttribute },
                { typeof(PortAttribute), GetPortAttribute },
                { typeof(GraphDisplayAttribute), GetGraphDisplayAttribute },
                { typeof(PortListAttribute), GetPortListAttribute },
            };

            int GetPortAttributeBase(ref List<PortInfo> portInfosList, Type type, string portName = null)
            {
                foreach (var attribute in attributeBag.attributes)
                {
                    var attributeType = attribute.GetType();
                    if (attributeType != typeof(SerializeReference))
                    {
                        continue;
                    }

                    portInfosList.Add(new PortInfo(currentRelativePropertyPath, type,
                        (PortBaseAttribute)currentAttribute, portName));
                    return currentProperty.depth;
                }

                Logger.LogAlways(
                    $"Invalid Port declaration detected! Be sure to add [SerializeReference] next to the [Output/PortList] attribute. Field: {attributeBag.fieldType}. Related PropertyPath: {currentProperty.propertyPath}");
                return currentProperty.depth;
            }

            //[PortList]
            int GetPortListAttribute()
            {
                if (attributeBag.fieldType == typeof(string) ||
                    (!typeof(IEnumerable<object>).IsAssignableFrom(attributeBag.fieldType) &&
                     !attributeBag.fieldType.IsArray))
                {
                    Logger.LogAlways(
                        $"Wrong use of [PortList] attribute detected! Make sure to use it only on lists and arrays!");
                    return currentProperty.depth;
                }

                var type = attributeBag.fieldType.IsArray
                    ? attributeBag.fieldType.GetElementType()
                    : attributeBag.fieldType.GetGenericArguments()[0];
                var listFieldName = currentProperty.displayName;
                return GetPortAttributeBase(ref portLists, type, listFieldName);
            }

            //[HideInInspector]
            int GetHideInInspectorAttribute()
            {
                return currentProperty.depth;
            }

            //[Port]
            int GetPortAttribute()
            {
                return GetPortAttributeBase(ref ports, attributeBag.fieldType);
            }

            //[GraphDisplay]
            int GetGraphDisplayAttribute()
            {
                if (currentGraphDisplayAttribute != null)
                {
                    return -1;
                }

                var displayAttribute = (GraphDisplayAttribute)currentAttribute;
                if (displayAttribute.displayType == DisplayType.Hide)
                {
                    return currentProperty.depth;
                }
                
                currentGraphDisplayAttribute = displayAttribute;
                return -1;
            }
        }


        public static PropertyBag GetCachedOrCreate(NodeAttribute nodeAttribute, Type nodeType,
            SerializedProperty nodeProperty)
        {
            if (propertyBagLookup.ContainsKey(nodeType))
            {
                return propertyBagLookup[nodeType];
            }
            else
            {
                var propertyBag = new PropertyBag(nodeAttribute, nodeType, nodeProperty);
                propertyBagLookup.Add(nodeType, propertyBag);
                return propertyBag;
            }
        }

        private void RetrieveAll(SerializedProperty nodeProperty)
        {
            var endProperty = nodeProperty.GetEndProperty();
            var property = nodeProperty.Copy();
            bool diveIntoChildren;
            var ignoreFurtherDepth = -1;

            if (!property.NextVisible(true))
            {
                return;
            }

            do
            {
                diveIntoChildren = true;

                if (SerializedProperty.EqualContents(property, endProperty))
                {
                    break;
                }

                if (property.propertyType == SerializedPropertyType.ManagedReference)
                {
                    diveIntoChildren = false;
                }

                if (ignoreFurtherDepth >= 0)
                {
                    if (property.depth > ignoreFurtherDepth)
                    {
                        continue;
                    }
                }

                if (IsRealArray(property) || IsTypeThatShouldNotEnter(property))
                {
                    diveIntoChildren = false;
                }

                var relativePropertyPath = property.propertyPath.Replace(nodeProperty.propertyPath, "");
                relativePropertyPath = relativePropertyPath.Substring(1, relativePropertyPath.Length - 1);

                currentRelativePropertyPath = relativePropertyPath;
                currentProperty = property;

                ignoreFurtherDepth = RetrieveCurrentAttributes();
            } while (property.NextVisible(diveIntoChildren));

            DoVisibilityExceptions(graphPropertiesAndGroups);
        }

        private void DoVisibilityExceptions(List<PropertyInfo> propertiesIncludingGroups)
        {
            foreach (var groupOrProperty in propertiesIncludingGroups)
            {
                if (groupOrProperty.GetType() == typeof(GroupInfo))
                {
                    var groupInfo = (GroupInfo)groupOrProperty;
                    if (groupInfo.graphProperties.Count > 0)
                    {
                        DoVisibilityExceptions(groupInfo.graphProperties);
                    }
                }
                else
                {
                    if (groupOrProperty is GraphPropertyInfo graphProperty &&
                        graphProperty.graphDisplay.displayType != DisplayType.Hide &&
                        graphProperty.graphDisplay.displayType != DisplayType.Unspecified)
                    {
                        DoVisibilityRecursive(graphProperty, graphProperty.groupInfo);
                    }
                }
            }
        }

        private void DoVisibilityRecursive(GraphPropertyInfo graphProperty, GroupInfo parent)
        {
            if (parent == null ||
                parent.graphDisplay.displayType.HasFlag(graphProperty.graphDisplay.displayType))
            {
                return;
            }

            parent.graphDisplay.displayType |= graphProperty.graphDisplay.displayType;
            DoVisibilityRecursive(graphProperty, parent.groupInfo);
        }

        private int RetrieveCurrentAttributes()
        {
            var ignoreFurtherDepth = -1;
            currentGraphDisplayAttribute = null;

            attributeBag.GetAttributes(currentProperty, true);

            foreach (var attribute in attributeBag.attributes)
            {
                var attributeType = attribute.GetType();
                currentAttribute = attribute;

                if (!attributebehaviors.ContainsKey(attributeType))
                {
                    continue;
                }

                ignoreFurtherDepth = attributebehaviors[attributeType]();
                if (ignoreFurtherDepth < 0)
                {
                    break;
                }
            }

            if (ignoreFurtherDepth >= 0)
            {
                return ignoreFurtherDepth;
            }

            var graphProperty =
                new GraphPropertyInfo(currentRelativePropertyPath, currentGraphDisplayAttribute);
            if (graphProperty.graphDisplay.displayType != DisplayType.Hide)
            {
                if (!IsRealArray(currentProperty) &&
                    (currentProperty.propertyType == SerializedPropertyType.Generic ||
                     currentProperty.propertyType == SerializedPropertyType.ManagedReference))
                {
                    var groupInfo = new GroupInfo(currentProperty.displayName, currentRelativePropertyPath,
                        currentGraphDisplayAttribute);

                    FindGroupAndAdd(ref currentRelativePropertyPath, groupInfo);

                    groupInfoLookup.Add(groupInfo);

                    if (currentProperty.propertyType == SerializedPropertyType.ManagedReference)
                    {
                        FindGroupAndAdd(ref currentRelativePropertyPath, graphProperty, true);
                    }
                }
                else
                {
                    FindGroupAndAdd(ref currentRelativePropertyPath, graphProperty);
                }
            }
            else
            {
                ignoreFurtherDepth = currentProperty.depth;
            }

            return ignoreFurtherDepth;
        }

        private void FindGroupAndAdd(ref string relativePath, GraphPropertyInfo propertyInfo,
            bool embeddedManagedReference = false)
        {
            GroupInfo groupPropertyBelongsTo = null;
            if (groupInfoLookup.Count > 0)
            {
                var levels = relativePath.Split('.');
                var levelLength = levels.Length;

                for (var i = groupInfoLookup.Count - 1; i >= 0; i--)
                {
                    var groupInfo = groupInfoLookup[i];
                    var groupLevelLength = groupInfo.levels.Length;
                    if (groupInfo.levels.Length < levels.Length)
                    {
                        if (levels[levelLength - 2] == groupInfo.levels[groupLevelLength - 1])
                        {
                            groupPropertyBelongsTo = groupInfo;
                            break;
                        }
                    }
                    else if (groupInfo.levels.Length == levels.Length)
                    {
                        if (levels[levelLength - 1] == groupInfo.levels[groupLevelLength - 1])
                        {
                            groupPropertyBelongsTo = groupInfo;
                            groupPropertyBelongsTo.hasEmbeddedManagedReference = embeddedManagedReference;
                            break;
                        }
                    }
                }
            }

            if (groupPropertyBelongsTo != null)
            {
                propertyInfo.groupInfo = groupPropertyBelongsTo;
                groupPropertyBelongsTo.graphProperties.Add(propertyInfo);

                if (propertyInfo.hasCustomGraphDisplay)
                {
                    return;
                }

                var displayAttribute = propertyInfo.graphDisplay;
                displayAttribute.displayType = groupPropertyBelongsTo.graphDisplay.displayType;
            }
            else
            {
                graphPropertiesAndGroups.Add(propertyInfo);
            }
        }

        private static bool IsTypeThatShouldNotEnter(SerializedProperty property)
        {
            if (property.hasChildren)
            {
                return property.propertyType == SerializedPropertyType.ManagedReference ||
                       property.propertyType == SerializedPropertyType.Vector2 ||
                       property.propertyType == SerializedPropertyType.Vector2Int ||
                       property.propertyType == SerializedPropertyType.Vector3 ||
                       property.propertyType == SerializedPropertyType.Vector3Int ||
                       property.propertyType == SerializedPropertyType.Quaternion ||
                       property.propertyType == SerializedPropertyType.Vector4 ||
                       property.propertyType == SerializedPropertyType.Rect ||
                       property.propertyType == SerializedPropertyType.RectInt ||
                       property.propertyType == SerializedPropertyType.Bounds ||
                       property.propertyType == SerializedPropertyType.BoundsInt ||
                       property.propertyType == SerializedPropertyType.Hash128;
            }

            return false;
        }

        private bool IsRealArray(SerializedProperty property)
        {
            return property.isArray && property.propertyType != SerializedPropertyType.Character &&
                   property.propertyType != SerializedPropertyType.String;
        }
    }
}