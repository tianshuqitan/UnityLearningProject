using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace NewGraph
{
    public class NodeView : Node
    {
        public Action<NodeView> OnNodeViewSelected;

        private VisualElement m_InspectorContent;

        public List<EditableLabelElement> editableLabels = new();
        private readonly Color m_NodeColor;
        public bool shouldSetBackgroundColor = true;
        private bool m_HasInspectorProperty = false;

        public List<PortView> OutputPorts => outputContainer.Query<PortView>().ToList();
        public PortView InputPort => inputContainer.Query<PortView>().ToList().First();

        public List<Foldout> foldouts = new();
        public NodeController controller { get; }

        public VisualElement InspectorContent => m_InspectorContent;

        public NodeView(NodeController controller, Color nodeColor)
        {
            this.controller = controller;
            m_NodeColor = nodeColor;
        }

        // 给背景上色
        private void ColorizeBackground()
        {
            if (shouldSetBackgroundColor && m_NodeColor != default)
            {
                style.backgroundColor = m_NodeColor;
            }
            else
            {
                style.backgroundColor = new Color(0.27f, 0.27f, 0.27f, 1.0f);;
            }
        }

        public void InitializeView()
        {
            editableLabels.Clear();

            SettingsChanged();

            var position = controller.GetStartPosition();
            SetPosition(position);

            var nodeModel = controller.nodeItem;
            if (!nodeModel.isUtilityNode)
            {
                ColorizeBackground();

                m_InspectorContent = new VisualElement();
                controller.DoForNameProperty(CreateLabelUI);
                controller.DoForOutputPortProperty(CreateOutputPort);
                controller.DoForInputPortProperty(CreateInputPort);
                controller.DoForEachPropertyOrGroup(new[] { this.extensionContainer, m_InspectorContent }, CreateGroupUI,
                    CreatePropertyUI);

                if (m_InspectorContent.childCount > 1 && !m_HasInspectorProperty)
                {
                    m_InspectorContent[1].style.display = DisplayStyle.None;
                }
            }
            else
            {
                var utilityNode = nodeModel.nodeData as IUtilityNode;
                if (utilityNode.ShouldColorizeBackground())
                {
                    ColorizeBackground();
                }

                if (utilityNode.CreateNameUI())
                {
                    controller.DoForNameProperty(CreateLabelUI);
                }

                if (utilityNode.CreateInspectorUI())
                {
                    m_InspectorContent = new VisualElement();
                    controller.DoForEachPropertyOrGroup(new[] { extensionContainer, m_InspectorContent },
                        CreateGroupUI,
                        CreatePropertyUI);
                }

                (nodeModel.nodeData as IUtilityNode)?.Initialize(controller);
            }
            
            RefreshExpandedState();
            
            nodeModel.CleanupFoldoutStates();
            BindUI(controller.GetSerializedObject());
        }

        private void CreateInputPort(PortInfo info, SerializedProperty property)
        {
            var port = CreateNodePort(info, property);
            inputContainer.Add(port);
        }

        private void CreateOutputPort(PortInfo info, SerializedProperty property)
        {
            var outputPort = CreateNodePort(info, property);
            outputContainer.Add(outputPort);
        }

        private static PortView CreateNodePort(PortInfo info, SerializedProperty property)
        {
            var port = PortView.Create<Edge>(info, property.Copy());
            if (!string.IsNullOrEmpty(info.portDisplay.name))
            {
                port.portName = info.portDisplay.name;
            }
            else
            {
                if (info.fieldName == null)
                {
                    port.portName = property != null ? property.displayName : info.fieldType.Name;
                }
                else
                {
                    port.portName = info.fieldName;
                }
            }

            return port;
        }

        private void CreatePropertyUI(VisualElement[] groupParents, GraphPropertyInfo propertyInfo,
            SerializedProperty property)
        {
            PropertyField Create(VisualElement elementParent, EditAbility editAbility)
            {
                var propertyField = CreatePropertyField(property);
                SetupPropertyField(propertyField, propertyInfo, editAbility);
                elementParent.Add(propertyField);
                return propertyField;
            }

            PropertyField nodeViewPropField = null;
            PropertyField inspectorPropField = null;
            
            var displayType = propertyInfo.graphDisplay.displayType;
            if (groupParents[0] != null && !controller.nodeItem.isUtilityNode &&
                displayType.HasFlag(DisplayType.NodeView))
            {
                nodeViewPropField = Create(groupParents[0], EditAbility.NodeView);
            }

            if (groupParents[1] != null && displayType.HasFlag(DisplayType.Inspector))
            {
                m_HasInspectorProperty = true;
                inspectorPropField = Create(groupParents[1], EditAbility.Inspector);
            }

            if (inspectorPropField != null && nodeViewPropField != null)
            {
                var inspectorChanged = false;
                var nodeViewChanged = false;

                nodeViewPropField.RegisterValueChangeCallback((evt) =>
                {
                    if (!inspectorChanged)
                    {
                        inspectorPropField.Unbind();
                        inspectorPropField.BindProperty(property);
                        nodeViewChanged = true;
                    }

                    inspectorChanged = false;
                });

                inspectorPropField.RegisterValueChangeCallback((evt) =>
                {
                    if (!nodeViewChanged)
                    {
                        nodeViewPropField.Unbind();
                        nodeViewPropField.BindProperty(property);
                        inspectorChanged = true;
                    }

                    nodeViewChanged = false;
                });
            }
        }


        private VisualElement[] CreateGroupUI(GroupInfo groupInfo, VisualElement[] parents, SerializedProperty property)
        {
            var newGroups = new VisualElement[parents.Length];

            void HandleEmbeddedReferenceCase(VisualElement foldoutContent)
            {
                var propertyField = foldoutContent.Q<PropertyField>();
                var foldout = propertyField?.Q<Foldout>();
                if (foldout == null)
                {
                    return;
                }

                foldout.value = true;
                foldout.RegisterValueChangedCallback((evt) =>
                {
                    if (evt.newValue != true)
                    {
                        foldout.value = true;
                    }
                });

                var toggle = foldout.Q<Toggle>();
                if (toggle == null)
                {
                    return;
                }

                // foldout.AddToClassList("managedReference");
                // toggle.AddToClassList(nameof(groupInfo.hasEmbeddedManagedReference));
            }

            // handle our custom foldout state for a group
            void HandleFoldoutState(Foldout newGroup, int index)
            {
                var propertyPathHash = newGroup.name.GetHashCode();
                var foldOutState = controller.nodeItem.GetOrCreateFoldout(propertyPathHash);
                foldOutState.used = true;

                newGroup.value = foldOutState.isExpanded;
                newGroup.RegisterValueChangedCallback((evt) =>
                {
                    foldOutState.isExpanded = evt.newValue;
                    controller.GetSerializedObject().ApplyModifiedPropertiesWithoutUndo();
                });
                newGroups[index] = newGroup;
                foldouts.Add(newGroup);
            }

            void AddAtIndex(int index, bool empty, string prefix)
            {
                if (!empty)
                {
                    var newGroup = new Foldout
                    {
                        pickingMode = PickingMode.Ignore,
                        text = groupInfo.groupName,
                        name = prefix + groupInfo.relativePropertyPath
                    };

                    // newGroup.AddToClassList(nameof(GroupInfo));
                    HandleFoldoutState(newGroup, index);

                    void GeomChanged(GeometryChangedEvent _)
                    {
                        var unityContent = newGroup.Q<VisualElement>("unity-content");
                        if (groupInfo.hasEmbeddedManagedReference)
                        {
                            HandleEmbeddedReferenceCase(unityContent);
                        }

                        unityContent.pickingMode = PickingMode.Ignore;
                        newGroup.UnregisterCallback<GeometryChangedEvent>(GeomChanged);
                    }

                    newGroup.RegisterCallback<GeometryChangedEvent>(GeomChanged);
                }
                else
                {
                    newGroups[index] = null;
                }

                if (parents[index] != null)
                {
                    parents[index].Add(newGroups[index]);
                }
            }

            var noNodeView = !groupInfo.graphDisplay.displayType.HasFlag(DisplayType.NodeView);
            AddAtIndex(0, noNodeView, nameof(DisplayType.NodeView));

            var noInspector = !groupInfo.graphDisplay.displayType.HasFlag(DisplayType.Inspector);
            AddAtIndex(1, noInspector, nameof(DisplayType.Inspector));

            return newGroups;
        }

        private void CreateLabelUI(SerializedProperty property)
        {
            var propertyField = CreatePropertyField(property);
            editableLabels.Add(new EditableLabelElement(propertyField));
            titleContainer.Add(propertyField);

            if (m_InspectorContent == null)
            {
                return;
            }

            var propertyFieldInspector = CreatePropertyField(property);
            editableLabels.Add(new EditableLabelElement(propertyFieldInspector));
            m_InspectorContent.Add(propertyFieldInspector);
        }

        private void BindUI(SerializedObject serializedObject)
        {
            this.Bind(serializedObject);
        }

        private static void SetupPropertyField(VisualElement propertyField, GraphPropertyInfo propertyInfo,
            EditAbility editAbility)
        {
            if (!propertyInfo.graphDisplay.editAbility.HasFlag(editAbility))
            {
                propertyField.SetEnabled(false);
            }
        }

        private static PropertyField CreatePropertyField(SerializedProperty property)
        {
            var propertyField = new PropertyField(property.Copy())
            {
                name = property.name,
                bindingPath = property.propertyPath,
            };
            return propertyField;
        }
        
        public void SetPosition(Vector2 newPosition)
        {
            base.SetPosition(new Rect(newPosition, Vector2.zero));
            controller.SetPosition(newPosition.x, newPosition.y);
        }

        public new Vector2 GetPosition()
        {
            var rect = base.GetPosition();
            return new Vector2(rect.x, rect.y);
        }

        public void SetInspectorActive(bool active = true)
        {
            if (!active)
            {
                foreach (var editableLabel in editableLabels)
                {
                    editableLabel.EnableInput(false);
                }
            }

            m_InspectorContent.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public VisualElement GetInspectorContent()
        {
            return m_InspectorContent;
        }

        private void SettingsChanged()
        {
            style.width = 300;
        }

        public override void OnSelected()
        {
            OnNodeViewSelected?.Invoke(this);
            base.OnSelected();
        }
    }
}