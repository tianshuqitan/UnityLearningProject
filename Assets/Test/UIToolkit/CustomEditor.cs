using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CustomEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_UXMLTree;

    private int m_ClickCount = 0;
    private const string m_ButtonPrefix = "button";
    
    [MenuItem("Chen/UI Toolkit/CustomEditor")]
    public static void ShowExample()
    {
        var wnd = GetWindow<CustomEditor>();
        wnd.titleContent = new GUIContent("CustomEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;
        
        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        root.Add(new Label("These controls were created using c# code."));
        root.Add(new Button{name = "button3", text = "This is Button3"});
        root.Add(new Toggle{name = "toggle3", label = "Number?"});
        
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Test/UIToolkit/CustomEditor.uxml");
        root.Add(visualTree.Instantiate());
        
        root.Add(m_UXMLTree.Instantiate());
        
        // Call the event handler
        SetupButtonHandler();
    }

    private void SetupButtonHandler()
    {
        var buttons = rootVisualElement.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    private void RegisterHandler(Button obj)
    {
        obj.RegisterCallback<ClickEvent>(PrintClickMessage);
    }

    private void PrintClickMessage(ClickEvent evt)
    {
        ++m_ClickCount;

        if (evt.currentTarget is Button button)
        {
            var buttonNumber = button.name.Substring(m_ButtonPrefix.Length);
            var toggleName = "toggle" + buttonNumber;
            Toggle toggle = rootVisualElement.Query<Toggle>(toggleName);
            Debug.Log("Button was clicked!" + (toggle.value ? " Count: " + m_ClickCount : ""));
        }
    }
}
