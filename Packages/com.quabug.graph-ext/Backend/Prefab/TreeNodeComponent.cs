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

    [DisallowMultipleComponent]
    public abstract class TreeNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>, ITreeNodeComponent
        where TNode : ITreeNode<GraphRuntime<TNode>>
        where TComponent : TreeNodeComponent<TNode, TComponent>
    {
        [SerializeReference] protected TNode _node;
        public TNode Node { get => _node; set => _node = value; }

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public PortId InputPort => new PortId(Id, Node.InputPortName);
        public PortId OutputPort => new PortId(Id, Node.OutputPortName);

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [SerializeField, HideInInspector] private FlatEdges _flatEdges = new FlatEdges();
        private TreeEdge _TreeEdge => GetComponent<TreeEdge>() ?? gameObject.AddComponent<TreeEdge>();

        private readonly HashSet<EdgeId> _edges = new HashSet<EdgeId>();

        IReadOnlySet<EdgeId> INodeComponent<TNode, TComponent>.GetEdges(GraphRuntime<TNode> graph)
        {
            _edges.Clear();
            _edges.UnionWith(_flatEdges.GetEdges(graph));
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
            // free to connect each other if they are not tree ports
            var isInputTreePort = graph.Graph.IsTreePort(input);
            var isOutputTreePort = graph.Graph.IsTreePort(output);
            if (!isInputTreePort && !isOutputTreePort) return true;

            if (input.NodeId == Id && output.NodeId == Id) return false; // same node
            if (input.NodeId == Id) return true; // only check compatible on output/start node
            // tree port must connect to another tree port
            if (!isInputTreePort || !isOutputTreePort) return false;
            // cannot connect to input/end node which is parent of output/start node to avoid circle dependency
            return !_TreeEdge.IsParentInputPort(input);
        }

        void INodeComponent<TNode, TComponent>.OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            if (_edges.Contains(edge)) return;
            _edges.Add(edge);
            var (input, output) = edge;
            // set parent for tree edges
            _TreeEdge.ConnectParent(edge, graph[output.NodeId].transform);
            // save non-tree output edges
            if (input != InputPort && output != OutputPort) _flatEdges.Connect(Id, edge, graph.Graph);
        }

        void INodeComponent<TNode, TComponent>.OnDisconnected(GameObjectNodes<TNode, TComponent> _, in EdgeId edge)
        {
            if (!_edges.Contains(edge)) return;
            var (input, output) = edge;
            // delete non-tree output edges
            if (input != InputPort && output != OutputPort) _flatEdges.Disconnect(Id, edge);
            // reset parent for tree edges
            _TreeEdge.DisconnectParent(edge);
            _edges.Remove(edge);
        }
    }
}