using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
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
                     from fi in nodeType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
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
                yield return new PortData(portName, orientation.ToEditor(), direction.ToEditor(), capacity.ToEditor(), portType);
            }

            void AssertPropertyPort(string portName)
            {
                Assert.IsFalse(propertyInputPorts.Contains(portName) || propertyOutputPorts.Contains(portName), $"port {portName} can only be assign to one property only.");
            }
        }

        public static UnityEditor.Experimental.GraphView.Direction ToEditor(this PortDirection direction) => direction switch
        {
            PortDirection.Input => UnityEditor.Experimental.GraphView.Direction.Input,
            PortDirection.Output => UnityEditor.Experimental.GraphView.Direction.Output,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        public static UnityEditor.Experimental.GraphView.Orientation ToEditor(this PortOrientation orientation) => orientation switch
        {
            PortOrientation.Horizontal => UnityEditor.Experimental.GraphView.Orientation.Horizontal,
            PortOrientation.Vertical => UnityEditor.Experimental.GraphView.Orientation.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };

        public static UnityEditor.Experimental.GraphView.Port.Capacity ToEditor(this PortCapacity capacity) => capacity switch
        {
            PortCapacity.Single => UnityEditor.Experimental.GraphView.Port.Capacity.Single,
            PortCapacity.Multi => UnityEditor.Experimental.GraphView.Port.Capacity.Multi,
            _ => throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null)
        };
    }
}