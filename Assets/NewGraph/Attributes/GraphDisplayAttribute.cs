using System;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GraphDisplayAttribute : Attribute
    {
        public DisplayType displayType = DisplayType.Inspector;
        public EditAbility editAbility = EditAbility.BothViews;
        public bool createGroup = true;

        public GraphDisplayAttribute(DisplayType displayType = DisplayType.Unspecified,
            EditAbility editAbility = EditAbility.Unspecified, bool createGroup = true)
        {
            if (displayType != DisplayType.Unspecified)
            {
                this.displayType = displayType;
            }

            if (editAbility != EditAbility.Unspecified)
            {
                this.editAbility = editAbility;
            }

            this.createGroup = createGroup;
        }
    }
}