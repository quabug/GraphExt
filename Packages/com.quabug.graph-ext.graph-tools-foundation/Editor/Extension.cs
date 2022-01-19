using System;
using JetBrains.Annotations;
using UnityEditor.GraphToolsFoundation.Overdrive;
using UnityEngine.GraphToolsFoundation.Overdrive;

namespace GraphExt.GTF.Editor
{
    public static class SerializableGUIDExtension
    {
        public static Guid ToGuid(this SerializableGUID guid) => Guid.Parse(guid.ToString());
    }

    public static class ElementIdExtension
    {
        public static EdgeId ToEdgeId([NotNull] this IEdgeModel edge)
        {
            return new EdgeId(input: edge.ToPort.ToPortId(), output: edge.FromPort.ToPortId());
        }

        public static PortId ToPortId([NotNull] this IPortModel port)
        {
            return new PortId(port.NodeModel.Guid.ToGuid(), port.UniqueName);
        }

        public static NodeId ToNodeId([NotNull] this INodeModel node)
        {
            return node.Guid.ToGuid();
        }
    }
}