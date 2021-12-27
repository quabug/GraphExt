using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    [AddComponentMenu("")]
    public class FlatNodeComponent<TNode> : NodeComponent<TNode> where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeField, HideInInspector] private List<Connection> _serializedConnections = new List<Connection>();
        private readonly Lazy<HashSet<EdgeId>> _edges;
        public override IReadOnlySet<EdgeId> Edges => _edges.Value;

        public FlatNodeComponent()
        {
            _edges = new Lazy<HashSet<EdgeId>>(
                () => new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge()))
            );
        }

        protected override void OnConnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            if (!_edges.Value.Contains(edge))
            {
                _edges.Value.Add(edge);
                _serializedConnections.Add(new Connection(input, output));
            }
        }

        protected override void OnDisconnected(GameObjectNodes<TNode> graph, in PortId input, in PortId output)
        {
            var edge = new EdgeId(input, output);
            if (_edges.Value.Contains(edge))
            {
                _edges.Value.Remove(edge);
                _serializedConnections.Remove(new Connection(input, output));
            }
        }

        [Serializable]
        private struct Connection : IEquatable<Connection>
        {
            public string InputNode;
            public string InputPort;
            public string OutputNode;
            public string OutputPort;

            public Connection(in PortId input, in PortId output)
            {
                InputNode = input.NodeId.ToString();
                InputPort = input.Name;
                OutputNode = output.NodeId.ToString();
                OutputPort = output.Name;
            }

            public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(InputNode), InputPort), new PortId(Guid.Parse(OutputNode), OutputPort));

            public bool Equals(Connection other)
            {
                return InputNode == other.InputNode && InputPort == other.InputPort && OutputNode == other.OutputNode && OutputPort == other.OutputPort;
            }

            public override bool Equals(object obj)
            {
                return obj is Connection other && Equals(other);
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

    }
}