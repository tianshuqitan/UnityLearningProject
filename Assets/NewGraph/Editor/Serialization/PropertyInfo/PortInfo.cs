using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NewGraph
{
    public class PortInfo : PropertyInfo
    {
        public PortBaseAttribute portDisplay;
        public Type fieldType;
        public string fieldName = null;
        public List<Type> connectableTypes = new();

        public PortInfo(string relativePropertyPath, Type fieldType, PortBaseAttribute portDisplay,
            string portName = null) : base(relativePropertyPath)
        {
            this.portDisplay = portDisplay;
            this.fieldType = fieldType;
            this.fieldName = portName;

            if (portDisplay.connectionPolicy == ConnectionPolicy.IdenticalOrSubclass)
            {
                var typeCollection = TypeCache.GetTypesDerivedFrom(fieldType);
                foreach (var type in typeCollection)
                {
                    connectableTypes.Add(type);
                }
            }

            if (!connectableTypes.Contains(fieldType))
            {
                connectableTypes.Add(fieldType);
            }
        }
    }
}