using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GraphExt.Memory
{
    public interface IMemoryNode {}

    public class NodePropertyAttribute : Attribute
    {
        public bool ReadOnly = false;
    }

    public class Node : INodeModule
    {
        public Vector2 Position { get; set; }
        public event Action OnDeleted;

        private readonly IReadOnlyList<INodeProperty> _properties;
        public IEnumerable<INodeProperty> Properties => _properties;

        public IMemoryNode Inner { get; }

        public Node(IMemoryNode inner)
        {
            Inner = inner;
            _properties = CreateProperties().ToArray();
        }

        IEnumerable<INodeProperty> CreateProperties()
        {
            var innerType = Inner?.GetType();
            var members = innerType?.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var (mi, attribute) in
                from mi in members
                from attribute in mi.GetCustomAttributes<NodePropertyAttribute>()
                select (mi, attribute)
            )
            {
                Type propertyType = null;
                if (mi is FieldInfo fi)
                {
                    if (attribute.ReadOnly) propertyType = typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType);
                    else propertyType = typeof(FieldInfoProperty<>).MakeGenericType(fi.FieldType);
                }
                else if (mi is PropertyInfo pi)
                {
                    if (attribute.ReadOnly || !pi.CanWrite) propertyType = typeof(ReadOnlyPropertyInfoProperty<>).MakeGenericType(pi.PropertyType);
                    else propertyType = typeof(PropertyInfoProperty<>).MakeGenericType(pi.PropertyType);
                }

                if (propertyType != null)
                {
                    var valueProperty = (INodeProperty) Activator.CreateInstance(propertyType, Inner, mi);
                    var labelProperty = new LabelProperty(mi.Name);
                    yield return new LabelValueProperty(labelProperty, valueProperty);
                }
            }
        }

        public void Dispose()
        {
            OnDeleted?.Invoke();
            OnDeleted = null;
        }
    }
}