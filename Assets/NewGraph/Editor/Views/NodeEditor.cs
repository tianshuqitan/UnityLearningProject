using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEditor;

namespace NewGraph
{
    public abstract class NodeEditor
    {
        [NonSerialized] private static Dictionary<Type, Type> editorLookup = null;

        public static Dictionary<Type, Type> EditorLookup
        {
            get
            {
                if (editorLookup != null)
                {
                    return editorLookup;
                }

                editorLookup = new Dictionary<Type, Type>();
                SetupEditorLookup();
                return editorLookup;
            }
        }

        private static void SetupEditorLookup()
        {
            var types = TypeCache.GetTypesWithAttribute<CustomNodeEditorAttribute>();
            foreach (var type in types)
            {
                if (!type.ImplementsOrInherits(typeof(NodeEditor)))
                {
                    continue;
                }

                var customNodeEditorAttribute = type.GetAttribute<CustomNodeEditorAttribute>();
                if (!customNodeEditorAttribute.nodeType.ImplementsOrInherits(typeof(INode)))
                {
                    continue;
                }

                if (editorLookup.ContainsKey(customNodeEditorAttribute.nodeType))
                {
                    continue;
                }

                editorLookup.Add(customNodeEditorAttribute.nodeType, type);
                if (!customNodeEditorAttribute.editorForChildClasses)
                {
                    continue;
                }

                var derivedTypes = TypeCache.GetTypesDerivedFrom(customNodeEditorAttribute.nodeType);
                foreach (var derivedType in derivedTypes.Where(derivedType => !editorLookup.ContainsKey(derivedType)))
                {
                    editorLookup.Add(derivedType, type);
                }
            }
        }

        public static NodeEditor CreateEditor(Type nodeType)
        {
            if (EditorLookup.ContainsKey(nodeType))
            {
                return Activator.CreateInstance(EditorLookup[nodeType]) as NodeEditor;
            }

            return null;
        }

        public abstract void Initialize(NodeController nodeController);

        public virtual bool ShouldSetBackgroundColor()
        {
            return true;
        }
    }
}