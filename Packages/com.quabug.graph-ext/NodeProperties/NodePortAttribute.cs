using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NodePortAttribute : Attribute
    {
        public bool Hide = false;
        public bool HideLabel = false;
        public Type PortType = null;
        public PortDirection Direction = PortDirection.Invalid;
        public PortCapacity Capacity = PortCapacity.Invalid;
        public PortOrientation Orientation = PortOrientation.Horizontal;
        public string Name = null;
    }

    public static class NodePortUtility
    {
        public static IEnumerable<PortData> FindPorts(Type nodeType)
        {
            var propertyInputPorts = new HashSet<string>();
            var propertyOutputPorts = new HashSet<string>();
            foreach (var (input, output) in
                     from mi in nodeType.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
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
                     from fi in nodeType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                     from portAttribute in fi.GetCustomAttributes<NodePortAttribute>()
                     select (fi, portAttribute)
                    )
            {
                var portName = fi.Name;
                var direction = portAttribute.Direction;
                var orientation = portAttribute.Orientation;
                var capacity = portAttribute.Capacity;
                var portType = portAttribute.PortType ?? fi.FieldType;

                if (propertyInputPorts.Contains(portName))
                {
                    direction = PortDirection.Input;
                    orientation = PortOrientation.Horizontal;
                    capacity = capacity == PortCapacity.Invalid ? PortCapacity.Single : capacity;
                }
                else if (propertyOutputPorts.Contains(portName))
                {
                    direction = PortDirection.Output;
                    orientation = PortOrientation.Horizontal;
                    capacity = capacity == PortCapacity.Invalid ? PortCapacity.Multi : capacity;
                }
                Assert.AreNotEqual(direction, PortDirection.Invalid);
                Assert.AreNotEqual(orientation, PortOrientation.Invalid);
                Assert.AreNotEqual(capacity, PortCapacity.Invalid);
                Assert.IsNotNull(portType);
                yield return new PortData(portName, orientation, direction, capacity, portType);
            }

            void AssertPropertyPort(string portName)
            {
                Assert.IsFalse(propertyInputPorts.Contains(portName) || propertyOutputPorts.Contains(portName), $"port {portName} can only be assign to one property only.");
            }
        }

    }
}