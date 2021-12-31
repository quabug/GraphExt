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
        private HashSet<EdgeId> _edges;

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            if (_edges != null) return _edges;

#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
            for (var i = _serializedConnections.Count - 1; i >= 0; i--)
            {
                var connection = _serializedConnections[i];
                graph[Guid.Parse(connection.InputNode)].CorrectIdName(portId: ref connection.InputPortId, portName: ref connection.InputPort);
                graph[Guid.Parse(connection.OutputNode)].CorrectIdName(portId: ref connection.OutputPortId, portName: ref connection.OutputPort);
                if (!connection.IsValid()) _serializedConnections.RemoveAt(i);
            }
#endif
            _edges = new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge()));
            return _edges;
        }

        public void OnConnected(GraphRuntime<TNode> graph, in EdgeId edge)
        {
            if (!_edges.Contains(edge))
            {
                _edges.Add(edge);
                _serializedConnections.Add(new Connection(edge, inputNode: graph[edge.Input.NodeId], outputNode: graph[edge.Output.NodeId]));
            }
        }

        public void OnDisconnected(GraphRuntime<TNode> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge))
            {
                _edges.Remove(edge);
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

            public Connection(in EdgeId edge, TNode inputNode, TNode outputNode)
            {
                InputNode = edge.Input.NodeId.ToString();
                InputPort = edge.Input.Name;
                OutputNode = edge.Output.NodeId.ToString();
                OutputPort = edge.Output.Name;
#if UNITY_EDITOR || ENABLE_RUNTIME_PORT_NAME_CORRECTION
                InputPortId = inputNode.FindSerializedId(InputPort);
                OutputPortId = outputNode.FindSerializedId(OutputPort);
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