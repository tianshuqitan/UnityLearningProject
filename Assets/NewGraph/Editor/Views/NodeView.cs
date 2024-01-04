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
    using static GraphSettingsSingleton;

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

        public List<Foldout> foldouts = new List<Foldout>();
        public NodeController controller { get; }

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
                style.backgroundColor = Settings.defaultNodeColor;
            }
        }

        public void InitializeView()
        {
            editableLabels.Clear();

            SettingsChanged();

            var position = controller.GetStartPosition();
            SetPosition(position);

            if (!controller.nodeItem.isUtilityNode)
            {
                ColorizeBackground();
                
                m_InspectorContent = new VisualElement();
                controller.DoForNameProperty(CreateLabelUI);
                controller.DoForEachPortProperty(CreateOutputPort);
                controller.DoForInputPortProperty(CreateInputPort);
                controller.DoForEachPropertyOrGroup(new[] { extensionContainer, m_InspectorContent }, CreateGroupUI,
                    CreatePropertyUI);
                
                if (m_InspectorContent.childCount > 1 && !m_HasInspectorProperty)
                {
                    m_InspectorContent[1].style.display = DisplayStyle.None;
                }
            }
            else
            {
                var utilityNode = controller.nodeItem.nodeData as IUtilityNode;
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

                (controller.nodeItem.nodeData as IUtilityNode).Initialize(controller);
            }
            
            controller.nodeItem.CleanupFoldoutStates();
            BindUI(controller.GetSerializedObject());
        }

        private void CreateInputPort(PortInfo info, SerializedProperty property)
        {
            Debug.LogFormat("{0} CreateInputPort", this.GetHashCode());
            var port = CreateNodePort(info, property);
            inputContainer.Add(port);
        }

        private void CreateOutputPort(PortInfo info, SerializedProperty property)
        {
            Debug.LogFormat("{0} CreateOutputPort", this.GetHashCode());
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
            PropertyField Create(VisualElement groupParent, Editability edtability)
            {
                var propertyField = CreatePropertyField(property);
                SetupPropertyField(propertyField, propertyInfo, edtability);

                groupParent.Add(propertyField);
                return propertyField;
            }

            PropertyField nodeViewPropField = null;
            PropertyField inspectorPropField = null;

            if (groupParents[0] != null && !controller.nodeItem.isUtilityNode &&
                propertyInfo.graphDisplay.displayType.HasFlag(DisplayType.NodeView))
            {
                nodeViewPropField = Create(groupParents[0], Editability.NodeView);
            }

            if (groupParents[1] != null && propertyInfo.graphDisplay.displayType.HasFlag(DisplayType.Inspector))
            {
                m_HasInspectorProperty = true;
                inspectorPropField = Create(groupParents[1], Editability.Inspector);
            }

            // workaround for value change disconnection bug, this can only happen if we have an inspector & and a nodeview together
            if (inspectorPropField != null && nodeViewPropField != null)
            {
                bool inspectorChanged = false;
                bool nodeViewChanged = false;

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
            VisualElement[] newGroups = new VisualElement[parents.Length];

            // handle special cases where we have an embedded managed reference that is within a group
            // normally we would have double header labels, so to be able to hide them via styling we need to add some classes.
            void HandleEmbeddedReferenceCase(VisualElement foldoutContent)
            {
                PropertyField propertyField = foldoutContent.Q<PropertyField>();
                if (propertyField != null)
                {
                    // get the containing foldout
                    Foldout foldout = propertyField.Q<Foldout>();
                    if (foldout != null)
                    {
                        // make sure to force the foldoutvalue to stay open
                        foldout.value = true;
                        foldout.RegisterValueChangedCallback((evt) =>
                        {
                            if (evt.newValue != true)
                            {
                                foldout.value = true;
                            }
                        });
                        // get the toggle
                        Toggle toggle = foldout.Q<Toggle>();
                        if (toggle != null)
                        {
                            // we found everything we need so tag classes
                            foldout.AddToClassList("managedReference");
                            toggle.AddToClassList(nameof(groupInfo.hasEmbeddedManagedReference));
                        }
                    }
                }
            }

            // handle our custom foldout state for a group
            void HandleFoldoutState(Foldout newGroup, int index)
            {
                int propertyPathHash = newGroup.name.GetHashCode();
                NodeModel.FoldoutState foldOutState = controller.nodeItem.GetOrCreateFoldout(propertyPathHash);
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
                // add label/ foldout etc.
                if (!empty)
                {
                    Foldout newGroup = new Foldout();
                    newGroup.pickingMode = PickingMode.Ignore;
                    newGroup.AddToClassList(nameof(GroupInfo));
                    newGroup.text = groupInfo.groupName;
                    newGroup.name = prefix + groupInfo.relativePropertyPath;

                    // handle our custom foldout state
                    HandleFoldoutState(newGroup, index);

                    // wait until the visual tree was built
                    void GeomChanged(GeometryChangedEvent _)
                    {
                        VisualElement unityContent = newGroup.Q<VisualElement>("unity-content");

                        // check if we need to apply special behavior for managed references
                        if (groupInfo.hasEmbeddedManagedReference)
                        {
                            HandleEmbeddedReferenceCase(unityContent);
                        }

                        // make sure to disable, that the ui captures input an prevents panning
                        unityContent.pickingMode = PickingMode.Ignore;

                        // toss the callback away, since we are done
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

            bool noNodeView = true;
            if (groupInfo.graphDisplay.displayType.HasFlag(DisplayType.NodeView))
            {
                noNodeView = false;
            }

            AddAtIndex(0, noNodeView, nameof(DisplayType.NodeView));

            bool noInspector = true;
            if (groupInfo.graphDisplay.displayType.HasFlag(DisplayType.Inspector))
            {
                noInspector = false;
            }

            AddAtIndex(1, noInspector, nameof(DisplayType.Inspector));

            return newGroups;
        }

        private void CreateLabelUI(SerializedProperty property)
        {
            // Add label to title Container
            PropertyField propertyField = CreatePropertyField(property);
            editableLabels.Add(new EditableLabelElement(propertyField));
            titleContainer.Add(propertyField);

            // Add label to inspector
            if (m_InspectorContent != null)
            {
                PropertyField propertyFieldInspector = CreatePropertyField(property);
                editableLabels.Add(new EditableLabelElement(propertyFieldInspector));
                m_InspectorContent.Add(propertyFieldInspector);
            }
        }

        private void BindUI(SerializedObject serializedObject)
        {
            this.Bind(serializedObject);
        }


        private void SetupPropertyField(VisualElement propertyField, GraphPropertyInfo propertyInfo,
            Editability editability)
        {
            if (!propertyInfo.graphDisplay.editability.HasFlag(editability))
            {
                propertyField.SetEnabled(false);
            }
        }

        private PropertyField CreatePropertyField(SerializedProperty property)
        {
            PropertyField propertyField = new PropertyField(property.Copy())
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
            style.width = Settings.nodeWidth;
        }
        
        public override void OnSelected()
        {
            Debug.Log("NodeView.OnSelected");
            OnNodeViewSelected?.Invoke(this);
            base.OnSelected();
        }
    }
}