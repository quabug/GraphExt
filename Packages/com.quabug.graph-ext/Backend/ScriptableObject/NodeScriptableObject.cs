using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public class NodeScriptableObject<TNode> : ScriptableObject where TNode : INode<GraphRuntime<TNode>>
    {
        [SerializeReference] private TNode _node;
        public TNode Node { get => _node; set => _node = value; }
        public string NodeSerializedPropertyName => nameof(_node);

        [SerializeField, HideInInspector] private List<Connection> _serializedConnections = new List<Connection>();
        private readonly Lazy<HashSet<EdgeId>> _edges;
        public IReadOnlySet<EdgeId> Edges => _edges.Value;

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public NodeScriptableObject()
        {
            _edges = new Lazy<HashSet<EdgeId>>(
                () => new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge()))
            );
        }

        public void OnConnected(in EdgeId edge)
        {
            if (!_edges.Value.Contains(edge))
            {
                _edges.Value.Add(edge);
                _serializedConnections.Add(new Connection(edge));
            }
        }

        public void OnDisconnected(in EdgeId edge)
        {
            if (_edges.Value.Contains(edge))
            {
                _edges.Value.Remove(edge);
                _serializedConnections.Remove(new Connection(edge));
            }
        }

        [Serializable]
        private struct Connection : IEquatable<Connection>
        {
            public string InputNode;
            public string InputPort;
            public string OutputNode;
            public string OutputPort;

            public Connection(in EdgeId edge)
            {
                InputNode = edge.Input.NodeId.ToString();
                InputPort = edge.Input.Name;
                OutputNode = edge.Output.NodeId.ToString();
                OutputPort = edge.Output.Name;
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