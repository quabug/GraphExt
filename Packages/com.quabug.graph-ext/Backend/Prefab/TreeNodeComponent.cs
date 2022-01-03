using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    public interface ITreeNodeComponent : INodeComponent
    {
        public PortId InputPort { get; }
        public PortId OutputPort { get; }
    }

    [ExecuteAlways, DisallowMultipleComponent]
    public abstract class TreeNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>, ITreeNodeComponent
        where TNode : ITreeNode<GraphRuntime<TNode>>
        where TComponent : TreeNodeComponent<TNode, TComponent>
    {
        [SerializeReference] protected TNode _node;
        public TNode Node { get => _node; set => _node = value; }

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        private TreeEdge _TreeEdge => GetComponent<TreeEdge>() ?? gameObject.AddComponent<TreeEdge>();
        private FlatEdges _FlatEdges => GetComponent<FlatEdges>() ?? gameObject.AddComponent<FlatEdges>();

        IReadOnlySet<EdgeId> INodeComponent<TNode, TComponent>.GetEdges(GraphRuntime<TNode> graph)
        {
            _edges.Clear();
            _edges.UnionWith(_FlatEdges.GetEdges(graph));
            var treeEdge = _TreeEdge.Edge;
            if (treeEdge.HasValue) _edges.Add(treeEdge.Value);
            return _edges;
        }

        public virtual NodeData FindNodeProperties(GameObjectNodes<TNode, TComponent> data)
        {
#if UNITY_EDITOR
            return Editor.Utility.CreateDefaultNodeData<TNode, TComponent>((TComponent)this, nameof(_node), Position);
#else
            return new NodeData(Array.Empty<INodeProperty>());
#endif
        }

        public virtual IEnumerable<PortData> FindNodePorts(GameObjectNodes<TNode, TComponent> data)
        {
#if UNITY_EDITOR
            return Editor.NodePortUtility.FindPorts(Node)
                .Select(port => port.Name == Node.InputPortName || port.Name == Node.OutputPortName ? port.AddClass("tree") : port)
            ;
#else
            return Enumerable.Empty<PortData>();
#endif
        }

        bool INodeComponent<TNode, TComponent>.IsPortCompatible(GameObjectNodes<TNode, TComponent> graph, in PortId input, in PortId output)
        {
            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            return _TreeEdge.IsPortCompatible(input, output, graph[input.NodeId].InputPort);
        }

        void INodeComponent<TNode, TComponent>.OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            var (input, output) = edge;
            // set parent for tree edges
            _TreeEdge.ConnectParent(edge, graph[output.NodeId].transform);
            // save non-tree output edges
            if (input != InputPort && output != OutputPort) _FlatEdges.Connect(Id, edge, graph.Graph);
        }

        void INodeComponent<TNode, TComponent>.OnDisconnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            var (input, output) = edge;
            // delete non-tree output edges
            if (input != InputPort && output != OutputPort) _FlatEdges.Disconnect(Id, edge);
            // reset parent for tree edges
            _TreeEdge.DisconnectParent(edge);
            _edges.Remove(edge);
        }
    }
}