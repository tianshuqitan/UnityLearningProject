using System.Collections.Generic;

namespace NewGraph
{
    public class GroupInfo : GraphPropertyInfo
    {
        public string groupName;
        public List<PropertyInfo> graphProperties = new();
        public string[] levels;
        public bool hasEmbeddedManagedReference = false;

        public GroupInfo(string groupName, string relativePropertyPath, GraphDisplayAttribute graphDisplayAttribute) :
            base(relativePropertyPath, graphDisplayAttribute)
        {
            this.groupName = groupName;
            this.levels = relativePropertyPath.Split('.');
        }
    }
}