using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Memory
{
    public interface IMemoryNode {}

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeTitleAttribute : Attribute
    {
        public string ConstTitle;
        public string TitlePropertyName;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public bool ReadOnly = false;
        public bool HideLabel = false;
    }

    [Serializable]
    public class Node : INodeModule
    {
        public Vector2 Position { get; set; }
        public event Action OnDeleted;

        [NonSerialized]
        private readonly IReadOnlyList<INodeProperty> _properties;
        public IEnumerable<INodeProperty> Properties => _properties;

        public IMemoryNode Inner { get; }

        public Node([NotNull] IMemoryNode inner)
        {
            Inner = inner;
            _properties = CreateProperties().ToArray();
        }

        IEnumerable<INodeProperty> CreateProperties()
        {
            var innerType = Inner.GetType();
            var members = innerType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var titleProperty = CreateTitleProperty();
            if (titleProperty != null) yield return titleProperty;
            foreach (var mi in members)
            {
                var property = TryCreateNodeProperty(mi) ?? TryCreateNodePort(mi);
                if (property != null) yield return property;
            }

            TitleProperty CreateTitleProperty()
            {
                var titleAttribute = innerType.GetCustomAttribute<NodeTitleAttribute>();
                string title = null;
                if (titleAttribute?.ConstTitle != null)
                {
                    title = titleAttribute.ConstTitle;
                }
                else if (titleAttribute?.TitlePropertyName != null)
                {
                    title = innerType
                        .GetMember(titleAttribute.TitlePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single()
                        .GetValue<string>(Inner)
                    ;
                }
                else if (titleAttribute != null)
                {
                    title = innerType.Name;
                }
                return title == null ? null : new TitleProperty(title);
            }

            INodeProperty TryCreateNodeProperty(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                Type propertyType = null;
                if (attribute != null)
                {
                    propertyType = mi switch
                    {
                        FieldInfo fi when attribute.ReadOnly =>
                            typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        FieldInfo fi => typeof(FieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        PropertyInfo pi when attribute.ReadOnly || !pi.CanWrite =>
                            typeof(ReadOnlyPropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        PropertyInfo pi => typeof(PropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        _ => null
                    };
                }

                if (propertyType == null) return null;

                var valueProperty = (INodeProperty) Activator.CreateInstance(propertyType, Inner, mi);
                var labelProperty = new LabelProperty(mi.Name);
                return new LabelValueProperty(labelProperty, valueProperty);
            }

            INodeProperty TryCreateNodePort(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePortAttribute>();
                if (attribute == null) return null;

                var portValue = mi.GetValue<IMemoryPort>(Inner);
                if (portValue == null) return null;

                var capacity = attribute.AllowMultipleConnections ? UnityEditor.Experimental.GraphView.Port.Capacity.Multi : UnityEditor.Experimental.GraphView.Port.Capacity.Single;
                PortProperty inputPort = null;
                PortProperty outputPort = null;
                if (attribute.Direction.HasFlag(NodePortDirection.Input))
                    inputPort = CreatePort(Direction.Input);
                if (attribute.Direction.HasFlag(NodePortDirection.Output))
                    outputPort = CreatePort(Direction.Output);
                return new LabelPortProperty(
                    labelProperty: new LabelProperty(mi.Name),
                    inputPortProperty: inputPort,
                    outputPortProperty: outputPort
                );

                PortProperty CreatePort(Direction direction)
                {
                    var port = new Port(portValue, attribute.PortType, direction, capacity);
                    return new PortProperty(port);
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