using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace GraphExt.Memory
{
    public interface IMemoryNode
    {
        NodeId Id { get; }
        bool IsPortCompatible(Graph graph, in PortId start, in PortId end);
        void OnConnected(Graph graph, in PortId start, in PortId end);
        void OnDisconnected(Graph graph, in PortId start, in PortId end);
    }

    public abstract class MemoryNode : IMemoryNode
    {
        public NodeId Id { get; set; /* for deserialization */ } = Guid.NewGuid();
        public virtual bool IsPortCompatible(Graph graph, in PortId start, in PortId end) => true;
        public virtual void OnConnected(Graph graph, in PortId start, in PortId end) {}
        public virtual void OnDisconnected(Graph graph, in PortId start, in PortId end) {}
    }

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
        public bool HideValue = false;
        public string InputPort = null;
        public string OutputPort = null;
    }

    public class Node : INodeData
    {
        public string UXMLPath => null; // use default node ui
        public NodeId Id => Inner.Id;
        public Vector2 Position { get; set; }

        private readonly Lazy<IReadOnlyList<INodeProperty>> _properties;
        public IReadOnlyList<INodeProperty> Properties => _properties.Value;
        public IReadOnlyList<PortData> Ports { get; }

        public IMemoryNode Inner { get; }

        public Node([NotNull] IMemoryNode inner)
        {
            Inner = inner;
            Ports = CollectPorts(inner).ToArray();
            _properties = new Lazy<IReadOnlyList<INodeProperty>>(() => CreateProperties().ToArray());
        }

        IEnumerable<PortData> CollectPorts(IMemoryNode inner)
        {
            var propertyInputPorts = new HashSet<string>();
            var propertyOutputPorts = new HashSet<string>();
            foreach (var (input, output) in
                     from mi in inner.GetType().GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                     from propertyAttribute in mi.GetCustomAttributes<NodePropertyAttribute>()
                     select (propertyAttribute.InputPort, propertyAttribute.OutputPort)
                    )
            {
                AssertPropertyPort(input);
                AssertPropertyPort(output);
                if (!string.IsNullOrEmpty(input)) propertyInputPorts.Add(input);
                if (!string.IsNullOrEmpty(output)) propertyOutputPorts.Add(output);
            }

            foreach (var (fi, portAttribute) in
                     from fi in inner.GetType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     from portAttribute in fi.GetCustomAttributes<NodePortAttribute>()
                     select (fi, portAttribute)
                    )
            {
                var portId = new PortId(inner.Id, fi.Name);
                var direction = portAttribute.Direction;
                var orientation = portAttribute.Orientation;
                var capacity = portAttribute.Capacity;
                var portType = portAttribute.PortType ?? fi.FieldType;

                if (propertyInputPorts.Contains(portId.Name))
                {
                    direction = PortDirection.Input;
                    orientation = PortOrientation.Horizontal;
                    capacity = capacity == PortCapacity.Invalid ? PortCapacity.Single : capacity;
                }
                else if (propertyOutputPorts.Contains(portId.Name))
                {
                    direction = PortDirection.Output;
                    orientation = PortOrientation.Horizontal;
                    capacity = capacity == PortCapacity.Invalid ? PortCapacity.Multi : capacity;
                }
                Assert.AreNotEqual(direction, PortDirection.Invalid);
                Assert.AreNotEqual(orientation, PortOrientation.Invalid);
                Assert.AreNotEqual(capacity, PortCapacity.Invalid);
                Assert.IsNotNull(portType);
                yield return new PortData(portId, orientation, direction, capacity, portType);
            }

            void AssertPropertyPort(string portName)
            {
                Assert.IsFalse(propertyInputPorts.Contains(portName) || propertyOutputPorts.Contains(portName), $"port {portName} can only be assign to one property only.");
            }
        }

        IEnumerable<INodeProperty> CreateProperties()
        {
            var innerType = Inner.GetType();
            var members = innerType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var nodePropertyPorts = new HashSet<string>();
            foreach (var mi in members)
            {
                var nodePropertyAttribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                AddPropertyPort(nodePropertyAttribute?.InputPort, mi);
                AddPropertyPort(nodePropertyAttribute?.OutputPort, mi);
            }

            yield return new NodePositionProperty(() => Position, pos => Position = pos);
            var titleProperty = CreateTitleProperty();
            if (titleProperty != null) yield return titleProperty;
            foreach (var mi in members)
            {
                var property = TryCreateNodeProperty(mi) ?? TryCreateNodePort(mi);
                if (property != null) yield return property;
            }

            void AddPropertyPort(string portName, MemberInfo mi)
            {
                if (portName == null) return;
                if (nodePropertyPorts.Contains(portName))
                    throw new Exception($"port {portName} of {innerType.Name}.{mi.Name} have already been used in another property");
                if (innerType.GetField(portName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) == null)
                    throw new Exception($"invalid port {portName} of {innerType.Name}.{mi.Name}");
                nodePropertyPorts.Add(portName);
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
                        FieldInfo fi when attribute.ReadOnly => typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        FieldInfo { IsInitOnly: true } fi => typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        FieldInfo fi => typeof(FieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        PropertyInfo pi when attribute.ReadOnly || !pi.CanWrite => typeof(ReadOnlyPropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        PropertyInfo pi => typeof(PropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        _ => null
                    };
                }

                if (propertyType == null) return null;

                return new LabelValuePortProperty(
                    labelProperty: attribute.HideLabel ? null : new LabelProperty(mi.Name),
                    valueProperty: attribute.HideValue ? null : (INodeProperty) Activator.CreateInstance(propertyType, Inner, mi),
                    leftPort: attribute.InputPort == null ? null : new PortContainerProperty(new PortId(Id, attribute.InputPort)),
                    rightPort: attribute.OutputPort == null ? null : new PortContainerProperty(new PortId(Id, attribute.OutputPort))
                );
            }

            INodeProperty TryCreateNodePort(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePortAttribute>();
                if (attribute == null) return null;
                Assert.IsTrue(mi is FieldInfo { IsStatic: true }, $"port must be a static field: {innerType.Name}.{mi.Name}");
                if (attribute.Hide) return null;
                var portId = mi.Name;
                if (nodePropertyPorts.Contains(portId)) return null;

                var port = new PortContainerProperty(new PortId(Id, portId));
                return new LabelValuePortProperty(
                    labelProperty: attribute.HideLabel ? null : new LabelProperty(attribute.Name ?? portId),
                    valueProperty: null,
                    leftPort: attribute.Direction == PortDirection.Input ? port : null,
                    rightPort: attribute.Direction == PortDirection.Output ? port : null
                );
            }
        }
    }
}