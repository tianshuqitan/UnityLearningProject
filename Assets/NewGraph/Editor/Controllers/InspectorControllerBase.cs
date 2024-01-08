using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    using static GraphSettingsSingleton;

    public abstract class InspectorControllerBase
    {
        private ReactiveSettings reactiveSettings;
        
        private VisualElement commandPanel;
        protected VisualElement inspectorRoot;
        private Label multipleNodesSelected;
        protected VisualElement inspectorContainer;
        private VisualElement selectedNodeInspector;
        protected VisualElement inspectorHeader;
        private Image inspectorButtonImage;

        protected int currentPickerWindow;
        protected bool pickerActive = false;
        private bool isInspectorVisible = true;

        public Action<IGraphModelData> OnShouldLoadGraph;
        public Action<IGraphModelData> OnAfterGraphCreated;
        public Action OnHomeClicked;

        protected InspectorControllerBase(VisualElement parent)
        {
            commandPanel = parent.Q<VisualElement>(nameof(commandPanel));
            inspectorRoot = parent.Q<VisualElement>(nameof(inspectorRoot));
            ReactiveSettings.Create(ref reactiveSettings, SettingsChanged);

            inspectorContainer = parent.Q<VisualElement>(nameof(inspectorContainer));

            Button createButton = commandPanel.Q<Button>(nameof(createButton));
            SetupCreateButton(createButton);

            Button loadButton = commandPanel.Q<Button>(nameof(loadButton));
            SetupLoadButton(loadButton);

            Button homeButton;
            homeButton = commandPanel.Q<Button>(nameof(homeButton));
            homeButton.clicked += HomeButtonClicked;
            homeButton.Add(GraphSettings.HomeButtonIcon);

            Button inspectorButton;
            inspectorButton = commandPanel.Q<Button>(nameof(inspectorButton));
            inspectorButton.clicked += InspectorButtonClicked;
            inspectorButtonImage = new Image();
            inspectorButton.Add(inspectorButtonImage);
            inspectorHeader = inspectorRoot.Q<VisualElement>(nameof(inspectorHeader));

            var startLabel = new Label(Settings.noGraphLoadedLabel);
            startLabel.AddToClassList(nameof(startLabel));

            inspectorHeader.Add(startLabel);
            multipleNodesSelected = inspectorRoot.Q<Label>(nameof(multipleNodesSelected));
            multipleNodesSelected.text = "";

            SetSelectedNodeInfoActive();
            SetInspectorVisibility(EditorPrefs.GetBool(GraphSettings.isInspectorVisiblePrefsKey, isInspectorVisible));
        }

        protected abstract void SetupCreateButton(Button createButton);

        protected abstract void SetupLoadButton(Button loadButton);

        private void HomeButtonClicked()
        {
            OnHomeClicked?.Invoke();
        }

        private void SetInspectorVisibility(bool visible = true)
        {
            if (visible != isInspectorVisible)
            {
                EditorPrefs.SetBool(GraphSettings.isInspectorVisiblePrefsKey, visible);
            }

            isInspectorVisible = visible;
            if (!isInspectorVisible)
            {
                inspectorContainer.style.visibility = Visibility.Hidden;
                inspectorHeader.style.visibility = Visibility.Hidden;
                inspectorRoot.style.minWidth = inspectorRoot.style.maxWidth = 0;
                inspectorButtonImage.image = GraphSettings.ShowInspectorIcon;
            }
            else
            {
                inspectorContainer.style.visibility = Visibility.Visible;
                inspectorHeader.style.visibility = Visibility.Visible;
                inspectorRoot.style.minWidth = inspectorRoot.style.maxWidth = Settings.inspectorWidth;
                inspectorButtonImage.image = GraphSettings.HideInspectorIcon;
            }
        }

        private void InspectorButtonClicked()
        {
            SetInspectorVisibility(!isInspectorVisible);
        }

        public void SetSelectedNodeInfoActive(int nodeCount = 0, int edgeCount = 0, bool active = true)
        {
            string SelectText(ref int selectedCount, string baseName)
            {
                return $"<b>{selectedCount} {baseName}{(selectedCount > 1 ? "s" : "")}</b>";
            }

            string DoNodeText(ref int selectedCount)
            {
                return SelectText(ref selectedCount, Settings.node);
            }

            string DoEdgeText(ref int selectedCount)
            {
                return SelectText(ref selectedCount, Settings.edge);
            }

            if (active && nodeCount == 0 && edgeCount == 0)
            {
                active = false;
            }
            else
            {
                if (nodeCount > 0 && edgeCount > 0)
                {
                    multipleNodesSelected.text =
                        $"{DoNodeText(ref nodeCount)} and {DoEdgeText(ref edgeCount)} {Settings.multipleSelectedMessagePartial}";
                }
                else
                {
                    multipleNodesSelected.text =
                        $"{(nodeCount > 0 ? DoNodeText(ref nodeCount) : DoEdgeText(ref edgeCount))} {Settings.multipleSelectedMessagePartial}";
                }
            }

            multipleNodesSelected.style.display = active ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetInspectorContent(VisualElement content, SerializedObject serializedObject = null)
        {
            if (selectedNodeInspector != null)
            {
                selectedNodeInspector.Unbind();
                selectedNodeInspector.RemoveFromHierarchy();
            }

            selectedNodeInspector = content;

            if (content != null)
            {
                inspectorContainer.Add(selectedNodeInspector);
                if (serializedObject != null)
                {
                    selectedNodeInspector.Bind(serializedObject);
                }
            }
        }

        private void SettingsChanged()
        {
            if (isInspectorVisible)
            {
                inspectorRoot.style.minWidth = inspectorRoot.style.maxWidth = Settings.inspectorWidth;
            }
        }

        public void Clear()
        {
            SetInspectorContent(null);
        }

        public abstract void CreateRenameGraphUI(IGraphModelData graph);
        
        public void Draw()
        {
            if (!pickerActive || Event.current.commandName != "ObjectSelectorClosed" ||
                EditorGUIUtility.GetObjectPickerControlID() != currentPickerWindow)
            {
                return;
            }
            
            var graph = EditorGUIUtility.GetObjectPickerObject();
            if (graph != null)
            {
                if (graph is IGraphModelData graphModelData)
                {
                    graphModelData.CreateSerializedObject();
                    CreateRenameGraphUI(graphModelData);
                    Clear();
                    OnShouldLoadGraph?.Invoke(graphModelData);
                }
            }

            pickerActive = false;
        }
    }
}