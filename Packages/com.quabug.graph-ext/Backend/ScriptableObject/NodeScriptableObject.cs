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
                () =>
                {
#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
                    for (var i = _serializedConnections.Count - 1; i >= 0; i--)
                    {
                        var connection = _serializedConnections[i];
                        _node.CorrectIdName(portId: ref connection.InputPortId, portName: ref connection.InputPort);
                        _node.CorrectIdName(portId: ref connection.OutputPortId, portName: ref connection.OutputPort);
                        if (!connection.IsValid()) _serializedConnections.RemoveAt(i);
                    }
#endif
                    return new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge()));
                });
        }

        public void OnConnected(in EdgeId edge)
        {
            if (!_edges.Value.Contains(edge))
            {
                _edges.Value.Add(edge);
                _serializedConnections.Add(new Connection(edge, _node));
            }
        }

        public void OnDisconnected(in EdgeId edge)
        {
            if (_edges.Value.Contains(edge))
            {
                _edges.Value.Remove(edge);
                var inputNode = edge.Input.NodeId.ToString();
                var inputPort = edge.Input.Name;
                var outputNode = edge.Output.NodeId.ToString();
                var outputPort = edge.Output.Name;
                var index = _serializedConnections.FindIndex(c =>
                    c.InputNode == inputNode && c.InputPort == inputPort &&
                    c.OutputNode == outputNode && c.OutputPort == outputPort
                );
                _serializedConnections.RemoveAt(index);
            }
        }

        [Serializable]
        private class Connection
        {
            public string InputNode;
            public string InputPort;
            public string InputPortId;
            public string OutputNode;
            public string OutputPort;
            public string OutputPortId;

            public Connection(in EdgeId edge, TNode node)
            {
                InputNode = edge.Input.NodeId.ToString();
                InputPort = edge.Input.Name;
                OutputNode = edge.Output.NodeId.ToString();
                OutputPort = edge.Output.Name;
#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
                InputPortId = node.FindSerializedId(InputPort);
                OutputPortId = node.FindSerializedId(OutputPort);
#endif
            }

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(InputPort) && !string.IsNullOrEmpty(InputNode) &&
                       !string.IsNullOrEmpty(OutputPort) && !string.IsNullOrEmpty(OutputNode)
                ;
            }

            public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(InputNode), InputPort), new PortId(Guid.Parse(OutputNode), OutputPort));
        }
    }
}