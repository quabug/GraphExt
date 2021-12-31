using System.Collections.Generic;
using UnityEngine;

namespace GraphExt
{
    public interface INodeComponent<TNode, TComponent>
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        NodeId Id { get; set; }
        TNode Node { get; set; }
        string NodeSerializedPropertyName { get; }
        Vector2 Position { get; set; }

        IReadOnlySet<EdgeId> GetEdges(GraphRuntime<TNode> graph);

        bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);

        delegate void NodeComponentConnect(in NodeId nodeId, in EdgeId edge);
        event NodeComponentConnect OnNodeComponentConnect;

        delegate void NodeComponentDisconnect(in NodeId nodeId, in EdgeId edge);
        event NodeComponentDisconnect OnNodeComponentDisconnect;
    }
}