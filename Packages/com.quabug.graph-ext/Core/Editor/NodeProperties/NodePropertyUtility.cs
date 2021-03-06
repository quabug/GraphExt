using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public static class NodePropertyUtility
    {
        public static IEnumerable<INodeProperty> CreateProperties(
            [NotNull] object nodeObj,
            NodeId nodeId,
            Func<string, UnityEditor.SerializedProperty> findProperty = null,
            BindingFlags flags = NodePropertyAttribute.BindingFlags
        )
        {
            var nodeType = nodeObj.GetType();
            var members = nodeType.GetMembers(flags);
            var nodePropertyPorts = new HashSet<string>();
            foreach (var mi in members)
            {
                var nodePropertyAttribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                AddPropertyPort(nodePropertyAttribute?.InputPort, mi);
                AddPropertyPort(nodePropertyAttribute?.OutputPort, mi);
            }
            var ports = NodePortUtility.FindPorts(nodeObj).Where(port => !nodePropertyPorts.Contains(port.Name)).ToDictionary(port => port.Name, port => port);

            var inputPortsVerticalContainer = new VerticalPortsProperty { Name = "vertical-input-ports", Order = -10000 };
            var outputPortsVerticalContainer = new VerticalPortsProperty { Name = "vertical-output-ports", Order = 10000 };

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
                    throw new ArgumentException($"port {portName} of {nodeType.Name}.{mi.Name} have already been used in another property");
                if (nodeType.GetField(portName, flags) == null)
                    throw new ArgumentException($"invalid port {portName} of {nodeType.Name}.{mi.Name}");
                nodePropertyPorts.Add(portName);
            }

            INodeProperty TryCreateNodeProperty(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                if (attribute == null) return null;

                var serializedProperty = findProperty?.Invoke(mi.Name);
                if (attribute.CustomFactory != null)
                {
                    Assert.IsTrue(typeof(INodePropertyFactory).IsAssignableFrom(attribute.CustomFactory), $"factory {attribute.CustomFactory.Name} must implement {nameof(INodePropertyFactory)}");
                    return ((INodePropertyFactory)Activator.CreateInstance(attribute.CustomFactory)).Create(mi, nodeObj, nodeId, serializedProperty);
                }

                INodeProperty valueProperty;
                if (attribute.SerializedField && serializedProperty != null)
                {
                    valueProperty = new SerializedFieldProperty(serializedProperty);
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
                    leftPort: attribute.InputPort == null ? null : new PortContainerProperty(attribute.InputPort),
                    rightPort: attribute.OutputPort == null ? null : new PortContainerProperty(attribute.OutputPort)
                );
            }

            INodeProperty TryCreateNodePort(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePortAttribute>();
                if (attribute == null) return null;
                if (attribute.Hide) return null;
                if (!ports.TryGetValue(mi.Name, out var port)) return null;

                var portContainer = new PortContainerProperty(port.Name);
                if (port.Orientation == PortOrientation.Horizontal)
                {
                    return new LabelValuePortProperty(
                        labelProperty: attribute.HideLabel ? null : new LabelProperty(attribute.DisplayName ?? port.Name),
                        valueProperty: null,
                        leftPort: port.Direction == PortDirection.Input ? portContainer : null,
                        rightPort: port.Direction == PortDirection.Output ? portContainer : null
                    );
                }

                if (port.Direction == PortDirection.Input) inputPortsVerticalContainer.Ports.Add(portContainer);
                else if (port.Direction == PortDirection.Output) outputPortsVerticalContainer.Ports.Add(portContainer);
                return null;
            }
        }
    }
}