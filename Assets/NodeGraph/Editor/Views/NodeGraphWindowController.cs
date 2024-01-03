using System;
using System.IO;
using NodeGraph.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeGraph.Editor
{
    public class NodeGraphWindowController
    {
        private readonly VisualElement m_RootElement;
        private int m_PickerControlID;
        private bool m_IsPickerActive = false;
        
        public Action<IGraphModelData> OnAfterGraphCreated;
        public Action<IGraphModelData> OnShouldLoadGraph;

        public NodeGraphWindowController(VisualElement rootElement)
        {
            m_RootElement = rootElement;
            InitController();
        }

        private void InitController()
        {
            var loadButton = m_RootElement.Q<Button>("LoadButton");
            loadButton.clicked += OnLoadButtonClick;

            var createButton = m_RootElement.Q<Button>("CreateButton");
            createButton.clicked += OnCreateButtonClick;
        }

        private void OnLoadButtonClick()
        {
            EditorGUIUtility.ShowObjectPicker<ScriptableGraphModel>(null, false, "", m_PickerControlID);
            m_IsPickerActive = true;
        }

        public void OnGraphAssetSelectFromPicker()
        {
            if (m_IsPickerActive == false || Event.current.commandName != "ObjectSelectorUpdated")
            {
                return;
            }
            
            var graph = EditorGUIUtility.GetObjectPickerObject();
            if (graph is IGraphModelData graphModel)
            {
                graphModel.CreateSerializedObject();
                OnShouldLoadGraph?.Invoke(graphModel);
            }

            m_IsPickerActive = false;
        }

        private void OnCreateButtonClick()
        {
            const string fileEnding = "asset";

            var filePath = Path.Combine(Application.dataPath, nameof(ScriptableGraphModel) + "." + fileEnding);
            if (File.Exists(filePath))
            {
                filePath = AssetDatabase.GenerateUniqueAssetPath(filePath);
            }

            filePath = Path.GetFileName(filePath);
            
            var path = EditorUtility.SaveFilePanel("Create Graph Asset", Application.dataPath, filePath, fileEnding);
            if (path.Length == 0)
            {
                return;
            }

            var graphAsset = ScriptableObject.CreateInstance<ScriptableGraphModel>();
            path = path.Substring(Application.dataPath.Length - 6);
            AssetDatabase.CreateAsset(graphAsset, path);
            AssetDatabase.SaveAssets();
            
            if (graphAsset is IGraphModelData graphModel)
            {
                graphModel.CreateSerializedObject();
                OnAfterGraphCreated?.Invoke(graphModel);
            }
        }
    }
}