using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Test.UIToolkit.CustomInspector.Editor
{
    [CustomPropertyDrawer(typeof(Tire))]
    public class Tire_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var popup = new UnityEngine.UIElements.PopupWindow
            {
                text = "Tire Details"
            };
            popup.Add(new PropertyField(property.FindPropertyRelative("m_AirPressure"), "Air Pressure(psi)"));
            popup.Add(new PropertyField(property.FindPropertyRelative("m_ProfileDepth"), "Profile Depth(mm)"));
            container.Add(popup);
            
            return container;
        }
    }
}
