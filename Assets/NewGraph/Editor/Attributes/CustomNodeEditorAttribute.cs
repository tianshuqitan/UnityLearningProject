using System;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeEditorAttribute : Attribute
    {
        public Type nodeType;
        public bool editorForChildClasses = false;

        public CustomNodeEditorAttribute(Type nodeType, bool editorForChildClasses = false)
        {
            this.nodeType = nodeType;
            this.editorForChildClasses = editorForChildClasses;
        }
    }
}