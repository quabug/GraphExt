using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public static class NodePropertyUtility
    {
        public static IEnumerable<INodeProperty> CreateProperties(
            object nodeObj,
            NodeId nodeId,
            UnityEditor.SerializedProperty nodeSerializedProperty = null
        )
        {
            var nodeType = nodeObj.GetType();
            var members = nodeType.GetMembers(NodePropertyAttribute.BindingFlags);
            var nodePropertyPorts = new HashSet<string>();
            foreach (var mi in members)
            {
                var nodePropertyAttribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                AddPropertyPort(nodePropertyAttribute?.InputPort, mi);
                AddPropertyPort(nodePropertyAttribute?.OutputPort, mi);
            }
            var ports = NodePortUtility.FindPorts(nodeObj).Where(port => !nodePropertyPorts.Contains(port.Name)).ToDictionary(port => port.Name, port => port);

            var inputPortsVerticalContainer = new VerticalPortsProperty { Order = -10000 };
            var outputPortsVerticalContainer = new VerticalPortsProperty { Order = 10000 };

            yield return inputPortsVerticalContainer;

            foreach (var mi in members)
            {
                var property = TryCreateNodeProperty(mi) ?? TryCreateNodePort(mi);
                if (property != null) yield return property;
            }

            yield return outputPortsVerticalContainer;

            void AddPropertyPort(string portName, MemberInfo mi)
            {
                if (portName == null) return;
                if (nodePropertyPorts.Contains(portName))
                    throw new Exception($"port {portName} of {nodeType.Name}.{mi.Name} have already been used in another property");
                if (nodeType.GetField(portName, NodePortAttribute.BindingFlags) == null)
                    throw new Exception($"invalid port {portName} of {nodeType.Name}.{mi.Name}");
                nodePropertyPorts.Add(portName);
            }

            INodeProperty TryCreateNodeProperty(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                if (attribute == null) return null;

                if (attribute.CustomFactory != null)
                {
                    return ((INodePropertyFactory)Activator.CreateInstance(attribute.CustomFactory)).Create(nodeObj, nodeId, nodeSerializedProperty);
                }

                INodeProperty valueProperty = null;
                if (attribute.SerializedField && nodeSerializedProperty != null && mi is FieldInfo)
                {
                    valueProperty = new SerializedFieldProperty(nodeSerializedProperty.FindPropertyRelative(mi.Name));
                }
                else
                {
                    var propertyType = mi switch
                    {
                        FieldInfo fi when attribute.ReadOnly => typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        FieldInfo { IsInitOnly: true } fi => typeof(ReadOnlyFieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        FieldInfo fi => typeof(FieldInfoProperty<>).MakeGenericType(fi.FieldType),
                        PropertyInfo pi when attribute.ReadOnly || !pi.CanWrite => typeof(ReadOnlyPropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        PropertyInfo pi => typeof(PropertyInfoProperty<>).MakeGenericType(pi.PropertyType),
                        _ => null
                    };
                    valueProperty = propertyType == null ? null : (INodeProperty)Activator.CreateInstance(propertyType, nodeObj, mi);
                }

                return valueProperty == null ? null : new LabelValuePortProperty(
                    labelProperty: attribute.HideLabel ? null : new LabelProperty(attribute.Name ?? mi.Name),
                    valueProperty: attribute.HideValue ? null : valueProperty,
                    leftPort: attribute.InputPort == null ? null : new PortContainerProperty(new PortId(nodeId, attribute.InputPort)),
                    rightPort: attribute.OutputPort == null ? null : new PortContainerProperty(new PortId(nodeId, attribute.OutputPort))
                );
            }

            INodeProperty TryCreateNodePort(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePortAttribute>();
                if (attribute == null) return null;
                if (attribute.Hide) return null;
                if (!ports.TryGetValue(mi.Name, out var port)) return null;

                var portContainer = new PortContainerProperty(new PortId(nodeId, port.Name));
                if (port.Orientation == Orientation.Horizontal)
                {
                    return new LabelValuePortProperty(
                        labelProperty: attribute.HideLabel ? null : new LabelProperty(attribute.Name ?? port.Name),
                        valueProperty: null,
                        leftPort: port.Direction == Direction.Input ? portContainer : null,
                        rightPort: port.Direction == Direction.Output ? portContainer : null
                    );
                }

                if (port.Direction == Direction.Input) inputPortsVerticalContainer.Ports.Add(portContainer);
                else outputPortsVerticalContainer.Ports.Add(portContainer);
                return null;
            }
        }
    }
}