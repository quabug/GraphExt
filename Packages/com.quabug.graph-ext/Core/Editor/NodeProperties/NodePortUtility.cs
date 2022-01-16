using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public static class NodePortUtility
    {
        public static IEnumerable<PortData> FindPorts(object node)
        {
            var nodeType = node.GetType();
            var propertyInputPorts = new HashSet<string>();
            var propertyOutputPorts = new HashSet<string>();
            foreach (var (input, output) in
                     from mi in nodeType.GetMembers(NodePortAttribute.BindingFlags)
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
                     from fi in nodeType.GetFields(NodePortAttribute.BindingFlags)
                     from portAttribute in fi.GetCustomAttributes<NodePortAttribute>()
                     select (fi, portAttribute)
                    )
            {
                var portName = fi.Name;
                var direction = portAttribute.Direction switch
                {
                    PortDirection.Invalid when portName.ToLower().Contains("input") => PortDirection.Input,
                    PortDirection.Invalid when portName.ToLower().Contains("output") => PortDirection.Output,
                    _ => portAttribute.Direction
                };
                var orientation = portAttribute.Orientation;
                var capacity = portAttribute.Capacity > 0 ? portAttribute.Capacity : CapacityOfFieldInfo(fi);
                var portType = portAttribute.PortType ?? (fi.FieldType.IsArray ? fi.FieldType.GetElementType() : fi.FieldType);

                if (propertyInputPorts.Contains(portName))
                {
                    direction = PortDirection.Input;
                    orientation = PortOrientation.Horizontal;
                }
                else if (propertyOutputPorts.Contains(portName))
                {
                    direction = PortDirection.Output;
                    orientation = PortOrientation.Horizontal;
                }
                Assert.AreNotEqual(direction, PortDirection.Invalid);
                Assert.AreNotEqual(orientation, PortOrientation.Invalid);
                Assert.IsTrue(capacity > 0);
                Assert.IsNotNull(portType);
                yield return new PortData(portName, orientation, direction, capacity, portType, portAttribute.Classes);
            }

            void AssertPropertyPort(string portName)
            {
                Assert.IsFalse(propertyInputPorts.Contains(portName) || propertyOutputPorts.Contains(portName), $"port {portName} can only be assign to one property only.");
            }

            int CapacityOfFieldInfo(FieldInfo fi)
            {
                if (!fi.FieldType.IsArray) return 1;
                var fieldValue = fi.GetValue(node) as Array;
                return fieldValue?.Length ?? int.MaxValue;
            }
        }

        public static Direction ToEditor(this PortDirection direction) => direction switch
        {
            PortDirection.Input => Direction.Input,
            PortDirection.Output => Direction.Output,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        public static Orientation ToEditor(this PortOrientation orientation) => orientation switch
        {
            PortOrientation.Horizontal => Orientation.Horizontal,
            PortOrientation.Vertical => Orientation.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
    }
}