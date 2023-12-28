using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Tools.AssetDatabaseHelper
{
    public class AssetDatabaseHelper : EditorWindow
    {
        [MenuItem("Tools/AssetDatabase/Helper")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetDatabaseHelper>();
            window.Show();
        }

        private StyleSheet m_ExtStyleSheet;
        private ObjectField m_ObjectField;
        private GroupBox m_ContentGroup;
        private TextField m_LabelPath;
        private TextField m_LabelGUID;

        private void CreateGUI()
        {
            m_ObjectField = new ObjectField("Select Object")
            {
                objectType = typeof(UnityEngine.Object)
            };
            m_ObjectField.RegisterValueChangedCallback((evt) =>
            {
                m_LabelPath.value = AssetDatabase.GetAssetPath(evt.newValue);
                m_LabelGUID.value = AssetDatabase.AssetPathToGUID(m_LabelPath.text);
            });
            rootVisualElement.Add(m_ObjectField);

            m_ContentGroup = new GroupBox()
            {
                style =
                {
                    borderLeftColor = Color.black,
                    borderRightColor = Color.black,
                    borderTopColor = Color.black,
                    borderBottomColor = Color.black,
                    borderLeftWidth = 1f,
                    borderRightWidth = 1f,
                    borderTopWidth = 1f,
                    borderBottomWidth = 1f
                }
            };
            rootVisualElement.Add(m_ContentGroup);

            m_LabelPath = new TextField("Object Path");
            m_ContentGroup.Add(m_LabelPath);

            m_LabelGUID = new TextField("Object GUID");
            m_ContentGroup.Add(m_LabelGUID);
        }

        // private void OnGUI()
        // {
        //     EditorGUILayout.BeginHorizontal();
        //     source = EditorGUILayout.ObjectField(source, typeof(Object), true);
        //     EditorGUILayout.EndHorizontal();
        //
        //     if (source != null)
        //     {
        //         var assetPath = AssetDatabase.GetAssetPath(source);
        //         GUI.Label(new Rect(0,0,300,50),assetPath);
        //         GUI.Label(new Rect(0,100,300,50),AssetDatabase.AssetPathToGUID(assetPath));
        //     }
        // }
    }
}