using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Test.GraphicView.Editor
{
    public class DialogGraph : EditorWindow
    {
        private DialogGraphView m_GraphView;
        private string m_FileName = "New File";

        [MenuItem("Tools/Test/DialogGraph")]
        public static void OpenDialogGraphWindow()
        {
            var window = GetWindow<DialogGraph>();
            window.titleContent = new GUIContent("DialogGraph");
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
        }

        private void OnDisable()
        {
            if (null != m_GraphView)
            {
                rootVisualElement.Remove(m_GraphView);
            }
        }

        private void ConstructGraphView()
        {
            if (null != m_GraphView)
            {
                return;
            }

            m_GraphView = new DialogGraphView
            {
                name = "Dialog Graph",
            };

            m_GraphView.StretchToParentSize();
            rootVisualElement.Add(m_GraphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(m_FileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => m_FileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button((() => RequestDataOperation(true))) { text = "Save Data" });
            toolbar.Add(new Button((() => RequestDataOperation(false))) { text = "Load Data" });

            var nodeCreateButton = new Button(() => m_GraphView.CreateNode("DialogNode"))
            {
                text = "Create Node"
            };
            toolbar.Add(nodeCreateButton);

            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (string.IsNullOrEmpty(m_FileName))
            {
                EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid name", "OK");
                return;
            }

            var utility = GraphSaveUtility.GetInstance(m_GraphView);
            if (save)
            {
                utility.SaveGraph(m_FileName);
            }
            else
            {
                utility.LoadGraph(m_FileName);
            }
        }
        
        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap { anchored = true };
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            m_GraphView.Add(miniMap);
        }
    }
}