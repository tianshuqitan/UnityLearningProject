using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NewGraph
{
    public static class UIElementsHelper
    {
        public static PropertyField CreatePropertyFieldWithCallback(SerializedProperty property,
            EventCallback<SerializedPropertyChangeEvent> changeCallback = null)
        {
            var propertyField = new PropertyField(property);
            if (changeCallback != null)
            {
                propertyField.RegisterCallback(changeCallback);
            }

            return propertyField;
        }

        public static void CreateGenericUI(SerializedObject serializedObject, VisualElement root,
            EventCallback<SerializedPropertyChangeEvent> changeCallback = null,
            System.Action<SerializedProperty, VisualElement> CreateAdditionalUI = null, string rootPropertyPath = null)
        {
            SerializedProperty rootProperty = null;
            if (rootPropertyPath != null)
            {
                rootProperty = serializedObject.FindProperty(rootPropertyPath);
            }

            Action<SerializedProperty, ScrollView> creationLogic;
            if (CreateAdditionalUI != null)
            {
                creationLogic = (prop, scrollView) =>
                {
                    var container = new VisualElement();
                    container.AddToClassList(nameof(container) + nameof(PropertyField));
                    container.Add(CreatePropertyFieldWithCallback(prop, changeCallback));
                    CreateAdditionalUI(prop, container);
                    scrollView.Add(container);
                };
            }
            else
            {
                creationLogic = (prop, scrollView) =>
                {
                    scrollView.Add(CreatePropertyFieldWithCallback(prop, changeCallback));
                };
            }

            void ForEachProperty(ref ScrollView scrollView)
            {
                var prop = rootProperty ?? serializedObject.GetIterator();
                if (!prop.NextVisible(true))
                {
                    return;
                }

                do
                {
                    if (prop.name != "m_Script")
                    {
                        creationLogic(prop.Copy(), scrollView);
                    }
                } while (prop.NextVisible(false));
            }

            var scrollView = new ScrollView();
            scrollView.AddToClassList("propertyList" + nameof(ScrollView));
            root.Add(scrollView);
            ForEachProperty(ref scrollView);
        }
    }
}