using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
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

    [AttributeUsage(AttributeTargets.Field)]
    public class NodePortAttribute : Attribute
    {
        /// <summary>
        /// true: hide entire port
        /// will force set to true if this is a property port
        /// </summary>
        public bool Hide = false;

        /// <summary>
        /// true: hide label only
        /// </summary>
        public bool HideLabel = false;
        public Type PortType = null;

        /// <summary>
        /// input or output
        /// </summary>
        public PortDirection Direction = PortDirection.Invalid;

        /// <summary>
        /// restrict number of connections
        /// </summary>
        public int Capacity = 0;

        /// <summary>
        /// horizontal or vertical
        /// </summary>
        public PortOrientation Orientation = PortOrientation.Horizontal;

        /// <summary>
        /// override port name for UI only
        /// </summary>
        public string DisplayName = null;

        /// <summary>
        /// identity the port
        /// useful for renaming port
        /// </summary>
        public string Id = null;

        public const System.Reflection.BindingFlags BindingFlags =
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.FlattenHierarchy;

    }

    public static class NodePortExtension
    {
        private class Port
        {
            public Dictionary<string /*portName*/, string /*portId*/> NameIdMap = new Dictionary<string, string>();
            public Dictionary<string /*portId*/, string /*portName*/> IdNameMap = new Dictionary<string, string>();
        }

        private static readonly Dictionary<Type/*nodeType*/, Port> _NODE_PORT_MAP = new Dictionary<Type, Port>();

        public static string FindSerializedId<TGraph>(this INode<TGraph> node, string portName)
        {
            return GetOrCreatePort(node.GetType()).NameIdMap.TryGetValue(portName, out var portId) ? portId : null;
        }

        public static void CorrectIdName<TGraph>(this INode<TGraph> node, ref string portId, ref string portName)
        {
            var port = GetOrCreatePort(node.GetType());
            if (string.IsNullOrEmpty(portId))
            {
                if (port.NameIdMap.TryGetValue(portName, out var newId))
                {
                    portId = newId;
                }
                else
                {
                    portName = null;
                    portId = null;
                }
            }
            else
            {
                if (port.IdNameMap.TryGetValue(portId, out var newName))
                {
                    portName = newName;
                }
                else
                {
                    portName = null;
                    portId = null;
                }
            }
        }

        private static Port GetOrCreatePort(Type nodeType)
        {
            if (!_NODE_PORT_MAP.TryGetValue(nodeType, out var port))
            {
                var idNames = FindIdNames(nodeType).ToArray();
                port = new Port
                {
                    IdNameMap = idNames.Where(t => !string.IsNullOrEmpty(t.id)).ToDictionary(t => t.id, t => t.name),
                    NameIdMap = idNames.ToDictionary(t => t.name, t => t.id),
                };
                _NODE_PORT_MAP[nodeType] = port;
            }
            return port;
        }

        private static IEnumerable<(string id, string name)> FindIdNames(Type nodeType)
        {
            return from fi in nodeType.GetFields(NodePortAttribute.BindingFlags)
                from attribute in fi.GetCustomAttributes<NodePortAttribute>()
                select (id: attribute.Id, port: fi.Name)
            ;
        }
    }
}