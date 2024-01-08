using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NewGraph
{
    public class ScriptableInspectorControllerGeneric<T> : InspectorControllerBase where T : ScriptableGraphModel
    {
        public ScriptableInspectorControllerGeneric(VisualElement parent) : base(parent)
        {
        }

        protected override void SetupCreateButton(Button createButton)
        {
            createButton.clicked += CreateButtonClicked;
            createButton.Add(GraphSettings.CreateButtonIcon);
        }

        private void CreateButtonClicked()
        {
            var fileEnding = "asset";

            var lastOpenedDirectoryGUID = EditorPrefs.GetString(GraphSettings.lastOpenedDirectoryPrefsKey, null);
            var lastFolder = "";
            if (lastOpenedDirectoryGUID != null)
            {
                lastFolder = AssetDatabase.GUIDToAssetPath(lastOpenedDirectoryGUID) ?? "";
            }

            var uniqueFilename = Path.Combine(lastFolder, typeof(T).Name + "." + fileEnding);
            if (File.Exists(uniqueFilename))
            {
                uniqueFilename = AssetDatabase.GenerateUniqueAssetPath(uniqueFilename);
            }

            uniqueFilename = Path.GetFileName(uniqueFilename);

            var path = EditorUtility.SaveFilePanel("Create New Graph", lastFolder, uniqueFilename, fileEnding);
            if (path.Length == 0)
            {
                return;
            }
            
            UnityEngine.Object graphData = ScriptableObject.CreateInstance<T>();
            path = path.Substring(Application.dataPath.Length - 6);

            // save the last selcted folder
            var folder = AssetDatabase.AssetPathToGUID(Path.GetDirectoryName(path));
            EditorPrefs.SetString(GraphSettings.lastOpenedDirectoryPrefsKey, folder);

            AssetDatabase.CreateAsset(graphData, path);
            AssetDatabase.SaveAssets();

            var graphModel = (IGraphModelData)graphData;
            graphModel.CreateSerializedObject();
            
            CreateRenameGraphUI(graphModel);
            Clear();

            OnAfterGraphCreated?.Invoke(graphModel);
        }

        protected override void SetupLoadButton(Button loadButton)
        {
            loadButton.clicked += LoadButtonClicked;
            loadButton.Add(GraphSettings.LoadButtonIcon);
        }

        private void LoadButtonClicked()
        {
            currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
            EditorGUIUtility.ShowObjectPicker<T>(null, false, "", currentPickerWindow);
            pickerActive = true;
        }

        public override void CreateRenameGraphUI(IGraphModelData graph)
        {
            inspectorHeader.Unbind();
            inspectorHeader.Clear();

            var graphModel = graph as T;
            if (graphModel == null)
            {
                return;
            }

            void SetStringValueAndApply(SerializedProperty prop, string otherValue)
            {
                prop.stringValue = otherValue;
                prop.serializedObject.ApplyModifiedProperties();
            }

            graphModel.SerializedGraphData.Update();
            var assetNameProperty = graphModel.GetOriginalNameProperty().Copy();

            var tmpNameProperty = graphModel.GetTmpNameProperty().Copy();
            SetStringValueAndApply(tmpNameProperty, assetNameProperty.stringValue);

            var labelField = new PropertyField(tmpNameProperty);
            inspectorHeader.Add(labelField);
            inspectorHeader.Bind(graphModel.SerializedGraphData);

            var editableLabel = new EditableLabelElement(labelField, onEditModeLeft: () =>
            {
                var graphAssetPath = AssetDatabase.GetAssetPath(graph.BaseObject);
                var originalName = Path.GetFileNameWithoutExtension(graphAssetPath);
                var name = tmpNameProperty.stringValue;

                if (graphAssetPath == null || originalName == name)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(name) && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                {
                    var pathToDirectory = Path.GetDirectoryName(graphAssetPath);
                    var uniquePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(pathToDirectory, name));
                    var uniqueFilename = Path.GetFileNameWithoutExtension(uniquePath);

                    if (name != uniqueFilename)
                    {
                        Logger.LogAlways(
                            $"The provided name was already present or not valid! Changing {name} to {uniqueFilename}!");
                    }

                    SetStringValueAndApply(tmpNameProperty, uniqueFilename);
                    AssetDatabase.RenameAsset(graphAssetPath, uniqueFilename);
                    AssetDatabase.Refresh();
                    return;
                }

                Logger.LogAlways($"The provided name ({name}) was empty or not a valid filename!");
                SetStringValueAndApply(tmpNameProperty, originalName);
            });
        }
    }
}