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
        IReadOnlySet<EdgeId> Edges { get; }

        bool IsPortCompatible(GameObjectNodes<TNode, TComponent> data, in PortId input, in PortId output);
        void OnConnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
        void OnDisconnected(GameObjectNodes<TNode, TComponent> data, in EdgeId edge);
    }
}