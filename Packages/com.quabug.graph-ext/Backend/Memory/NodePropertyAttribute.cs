﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace GraphExt.Memory
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NodePropertyAttribute : Attribute
    {
        public bool ReadOnly = false;
        public bool HideLabel = false;
        public bool HideValue = false;
        public string InputPort = null;
        public string OutputPort = null;

        public static IEnumerable<INodeProperty> CreateProperties(object nodeObj, NodeId nodeId)
        {
            var nodeType = nodeObj.GetType();
            var members = nodeType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var nodePropertyPorts = new HashSet<string>();
            foreach (var mi in members)
            {
                var nodePropertyAttribute = mi.GetCustomAttribute<NodePropertyAttribute>();
                AddPropertyPort(nodePropertyAttribute?.InputPort, mi);
                AddPropertyPort(nodePropertyAttribute?.OutputPort, mi);
            }

            foreach (var mi in members)
            {
                var property = TryCreateNodeProperty(mi) ?? TryCreateNodePort(mi);
                if (property != null) yield return property;
            }

            void AddPropertyPort(string portName, MemberInfo mi)
            {
                if (portName == null) return;
                if (nodePropertyPorts.Contains(portName))
                    throw new Exception($"port {portName} of {nodeType.Name}.{mi.Name} have already been used in another property");
                if (nodeType.GetField(portName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) == null)
                    throw new Exception($"invalid port {portName} of {nodeType.Name}.{mi.Name}");
                nodePropertyPorts.Add(portName);
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
                    valueProperty: attribute.HideValue ? null : (INodeProperty) Activator.CreateInstance(propertyType, nodeObj, mi),
                    leftPort: attribute.InputPort == null ? null : new PortContainerProperty(new PortId(nodeId, attribute.InputPort)),
                    rightPort: attribute.OutputPort == null ? null : new PortContainerProperty(new PortId(nodeId, attribute.OutputPort))
                );
            }

            INodeProperty TryCreateNodePort(MemberInfo mi)
            {
                var attribute = mi.GetCustomAttribute<NodePortAttribute>();
                if (attribute == null) return null;
                Assert.IsTrue(mi is FieldInfo { IsStatic: true }, $"port must be a static field: {nodeType.Name}.{mi.Name}");
                if (attribute.Hide) return null;
                var portId = mi.Name;
                if (nodePropertyPorts.Contains(portId)) return null;

                var port = new PortContainerProperty(new PortId(nodeId, portId));
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