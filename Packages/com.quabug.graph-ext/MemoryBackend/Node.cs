using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphExt.Memory
{
    public interface IMemoryNode {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public Type NodePropertyType { get; }

        public NodePropertyAttribute([NotNull] Type nodePropertyType)
        {
            Assert.IsTrue(typeof(INodeProperty).IsAssignableFrom(nodePropertyType));
            NodePropertyType = nodePropertyType;
        }
    }

    public class Node : INodeModule
    {
        public Vector2 Position { get; set; }
        public event Action OnDeleted;

        public IEnumerable<INodeProperty> Properties
        {
            get
            {
                var innerType = Inner?.GetType();
                var fields = innerType?.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var (fi, attribute) in
                    from fi in fields
                    from attribute in fi.GetCustomAttributes<NodePropertyAttribute>()
                    select (fi, attribute)
                )
                {
                    yield return (INodeProperty) Activator.CreateInstance(attribute.NodePropertyType, Inner, fi);
                }
                var properties = innerType?.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var (pi, attribute) in
                    from pi in properties
                    from attribute in pi.GetCustomAttributes<NodePropertyAttribute>()
                    select (pi, attribute)
                )
                {
                    yield return (INodeProperty) Activator.CreateInstance(attribute.NodePropertyType, Inner, pi);
                }
            }
        }

        public IMemoryNode Inner { get; }

        public Node(IMemoryNode inner)
        {
            Inner = inner;
        }

        public void Dispose()
        {
            OnDeleted?.Invoke();
            OnDeleted = null;
        }
    }
}