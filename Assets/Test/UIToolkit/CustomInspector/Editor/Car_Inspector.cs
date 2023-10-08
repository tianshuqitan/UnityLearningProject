using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Test.UIToolkit.CustomInspector.Editor
{
    [UnityEditor.CustomEditor(typeof(Car))]
    public class Car_Inspector : UnityEditor.Editor
    {
        public VisualTreeAsset m_InspectorXML;
        
        public override VisualElement CreateInspectorGUI()
        {
            var inspector = new VisualElement();
            m_InspectorXML.CloneTree(inspector);
            
            // Get a reference to the default inspector foldout control
            VisualElement inspectorFoldout = inspector.Q("Default_Inspector");
            InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);
            
            return inspector;
        }
    }
}
