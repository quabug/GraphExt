using System;
using UnityEngine;

namespace GraphExt
{
    [DisallowMultipleComponent]
    public class FlatNodeComponent<TNode, TComponent> : MonoBehaviour, INodeComponent<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : FlatNodeComponent<TNode, TComponent>
    {
        [SerializeReference]
#if UNITY_EDITOR
        [NodeProperty(CustomFactory = typeof(Editor.InnerNodeProperty.Factory))]
#endif
        private TNode _node;
        public TNode Node { get => _node; set => _node = value; }

        [SerializeField, HideInInspector] private string _nodeId;
        public NodeId Id { get => Guid.Parse(_nodeId); set => _nodeId = value.ToString(); }

        [SerializeField]
#if UNITY_EDITOR
        [NodeProperty(CustomFactory = typeof(Editor.NodeSerializedPositionProperty.Factory))]
#endif
        protected Vector2 _Position;
        public Vector2 Position { get => _Position; set => _Position = value; }

        public INodeComponent.NodeComponentConnect OnNodeComponentConnect { get; set; }
        public INodeComponent.NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }

        [SerializeField, HideInInspector] private FlatEdges _edges = new FlatEdges();

        public IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph)
        {
            return _edges.GetEdges(graph);
        }

        public bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output)
        {
            return true;
        }

        public void OnConnected(GameObjectNodes<TNode, TComponent> graph, in EdgeId edge)
        {
            _edges.Connect(Id, edge, graph.Runtime);
        }

        public void OnDisconnected(GameObjectNodes<TNode, TComponent> _, in EdgeId edge)
        {
            _edges.Disconnect(Id, edge);
        }
    }
}