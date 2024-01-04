using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace NewGraph
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PortBaseAttribute : Attribute
    {
        public string name = null;
        public Port.Capacity capacity = Port.Capacity.Single;
        public Direction direction = Direction.Input;
        public ConnectionPolicy connectionPolicy = ConnectionPolicy.IdenticalOrSubclass;
        public Func<Type, Type, bool> isValidConnectionCheck = null;

        private static Dictionary<ConnectionPolicy, Func<Type, Type, bool>> connectionPolicybehaviors =
            new Dictionary<ConnectionPolicy, Func<Type, Type, bool>>()
            {
                { ConnectionPolicy.Identical, (input, output) => input == output },
                {
                    ConnectionPolicy.IdenticalOrSubclass,
                    (input, output) => input == output || input.IsSubclassOf(output)
                },
            };

        /// <summary>
        /// Port Base attribute. This needs to be added to every SerializeReference field that should show up in the graph as a assignable node.
        /// You'll mostly want to use the Port attribute instead.
        /// </summary>
        /// <param name="capacity">How many connections are allowed.</param>
        /// <param name="direction">What port direction do we want to display?</param>
        /// <param name="connectionPolicy">What connections are allowed only to the matching class or subclasses as well?</param>
        public PortBaseAttribute(string name = null, Port.Capacity capacity = Port.Capacity.Single,
            Direction direction = Direction.Input, ConnectionPolicy connectionPolicy = ConnectionPolicy.Unspecified)
        {
            this.capacity = capacity;
            this.direction = direction;

            if (connectionPolicy != ConnectionPolicy.Unspecified)
            {
                this.connectionPolicy = connectionPolicy;
            }

            this.name = name;
            isValidConnectionCheck = connectionPolicybehaviors[this.connectionPolicy];
        }
    }
}