using UnityEngine;

namespace GraphExt
{
    public interface INodeComponent
    {
        NodeId Id { get; set; }
        Vector2 Position { get; set; }

        delegate void NodeComponentConnect(in NodeId nodeId, in EdgeId edge);
        NodeComponentConnect OnNodeComponentConnect { get; set; }

        delegate void NodeComponentDisconnect(in NodeId nodeId, in EdgeId edge);
        NodeComponentDisconnect OnNodeComponentDisconnect { get; set; }
    }

    public interface INodeComponent<TNode, TComponent> : INodeComponent
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        TNode Node { get; set; }
        IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph);
        bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
    }
}