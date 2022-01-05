using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt
{
    [DisallowMultipleComponent]
    public class FlatNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : FlatNodeComponent<TNode, TComponent>
    {
        [SerializeReference] private TNode _node;
        public TNode Node { get => _node; set => _node = value; }

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [field: SerializeField, HideInInspector] public Vector2 Position { get; set; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [SerializeField] private FlatEdges _edges = new FlatEdges();

        private readonly Lazy<IReadOnlyDictionary<string, PortData>> _portsCache;

        public FlatNodeComponent()
        {
#if UNITY_EDITOR
            _portsCache = new Lazy<IReadOnlyDictionary<string, PortData>>(() =>
                Editor.NodePortUtility.FindPorts(Node).ToDictionary(port => port.Name, port => port)
            );
#else
            _portsCache = new Lazy<IReadOnlyDictionary<string, PortData>>(() => new Dictionary<string, PortData>());
#endif
        }

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            return _edges.GetEdges(graph);
        }

        public NodeData FindNodeProperties(GameObjectNodes<TNode, TComponent> data)
        {
#if UNITY_EDITOR
            return Editor.Utility.CreateDefaultNodeData<TNode, TComponent>((TComponent)this, nameof(_node), Position);
#else
            return new NodeData(Array.Empty<INodeProperty>());
#endif
        }

        public IReadOnlyDictionary<string, PortData> FindNodePorts(GameObjectNodes<TNode, TComponent> data)
        {
            return _portsCache.Value;
        }

        public bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output)
        {
            return true;
        }

        public void OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            _edges.Connect(Id, edge, graph.Graph);
        }

        public void OnDisconnected(GameObjectNodes<TNode, TComponent> _, in EdgeId edge)
        {
            _edges.Disconnect(Id, edge);
        }
    }
}