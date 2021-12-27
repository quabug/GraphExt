using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Assertions;

namespace GraphExt
{
    public enum PortDirection
    {
        Input, Output, Invalid
    }

    public enum PortOrientation
    {
        Horizontal, Vertical, Invalid
    }

    public enum PortCapacity
    {
        Single, Multi, Invalid
    }

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

        public static IEnumerable<string> FindPortNames(Type nodeType)
        {
            const BindingFlags flags = BindingFlags.Static |
                                       BindingFlags.Public |
                                       BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy
            ;
            return nodeType.GetFields(flags)
                .Where(fi => fi.GetCustomAttribute<NodePortAttribute>() != null)
                .Select(fi => fi.Name)
            ;
        }
    }
}