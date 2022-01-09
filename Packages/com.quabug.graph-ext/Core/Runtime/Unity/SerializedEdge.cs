using System;

namespace GraphExt
{
    [Serializable]
    public class SerializableEdge : IEquatable<SerializableEdge>
    {
        public string InputNode;
        public string InputPort;
        public string InputPortId;
        public string OutputNode;
        public string OutputPort;
        public string OutputPortId;

        public SerializableEdge(in EdgeId edge, string inputPortId, string outputPortId)
        {
            InputNode = edge.Input.NodeId.ToString();
            InputPort = edge.Input.Name;
            OutputNode = edge.Output.NodeId.ToString();
            OutputPort = edge.Output.Name;
            InputPortId = inputPortId;
            OutputPortId = outputPortId;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(InputPort) && !string.IsNullOrEmpty(InputNode) &&
                   !string.IsNullOrEmpty(OutputPort) && !string.IsNullOrEmpty(OutputNode)
            ;
        }

        public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(InputNode), InputPort), new PortId(Guid.Parse(OutputNode), OutputPort));

        public bool Equals(SerializableEdge other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return InputNode == other.InputNode && InputPort == other.InputPort && OutputNode == other.OutputNode && OutputPort == other.OutputPort;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SerializableEdge)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (InputNode != null ? InputNode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InputPort != null ? InputPort.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OutputNode != null ? OutputNode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OutputPort != null ? OutputPort.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public static class SerializableEdgeExtension
    {
        public static SerializableEdge ToSerializable(this in EdgeId edge)
        {
            return new SerializableEdge(edge, "", "");
        }

        public static SerializableEdge ToSerializable<TNode>(this in EdgeId edge, IReadOnlyGraphRuntime<TNode> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            var inputPortId = graph[edge.Input.NodeId].FindSerializedId(edge.Input.Name);
            var outputPortId = graph[edge.Output.NodeId].FindSerializedId(edge.Output.Name);
            return new SerializableEdge(edge, inputPortId, outputPortId);
        }

        public static EdgeId ToEdge<TNode>(this SerializableEdge edge, GraphRuntime<TNode> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            graph[Guid.Parse(edge.InputNode)].CorrectIdName(portName: ref edge.InputPort, portId: ref edge.InputPortId);
            graph[Guid.Parse(edge.OutputNode)].CorrectIdName(portName: ref edge.OutputPort, portId: ref edge.OutputPortId);
            return edge.ToEdge();
        }
    }
}