using System;
using System.Reflection;
using UnityEditor;

namespace NewGraph
{
    public class AttributesBag
    {
        private delegate FieldInfo GetFieldInfoAndStaticTypeFromProperty(SerializedProperty aProperty, out Type aType);

        private static GetFieldInfoAndStaticTypeFromProperty s_GetFieldInfoAndStaticTypeFromProperty;

        public object[] attributes = { };
        public Type fieldType = null;

        public void GetAttributes(SerializedProperty property, bool inherit)
        {
            var fieldInfo = GetFieldInfoAndStaticType(property, out _);
            if (fieldInfo == null)
            {
                return;
            }

            fieldType = fieldInfo.FieldType;
            attributes = fieldInfo.GetCustomAttributes(inherit);
        }

        private static FieldInfo GetFieldInfoAndStaticType(SerializedProperty prop, out Type type)
        {
            if (s_GetFieldInfoAndStaticTypeFromProperty != null)
            {
                return s_GetFieldInfoAndStaticTypeFromProperty(prop, out type);
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.Name != "ScriptAttributeUtility")
                    {
                        continue;
                    }

                    var mi = t.GetMethod(nameof(GetFieldInfoAndStaticTypeFromProperty),
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    s_GetFieldInfoAndStaticTypeFromProperty =
                        (GetFieldInfoAndStaticTypeFromProperty)Delegate.CreateDelegate(
                            typeof(GetFieldInfoAndStaticTypeFromProperty), mi);
                    break;
                }

                if (s_GetFieldInfoAndStaticTypeFromProperty != null)
                {
                    break;
                }
            }

            if (s_GetFieldInfoAndStaticTypeFromProperty != null)
            {
                return s_GetFieldInfoAndStaticTypeFromProperty(prop, out type);
            }

            UnityEngine.Debug.LogError("GetFieldInfoAndStaticType::Reflection failed!");
            type = null;
            return null;
        }
    }
}