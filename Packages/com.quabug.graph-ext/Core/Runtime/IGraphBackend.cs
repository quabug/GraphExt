using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphBackend<TNode, TNodeComponent> where TNode : INode<GraphRuntime<TNode>>
    {
        GraphRuntime<TNode> Runtime { get; }
        public IEnumerable<TNodeComponent> Nodes { get; }
        public IReadOnlyBiDictionary<NodeId, TNodeComponent> NodeMap { get; }
    }

    public interface ISerializableGraphBackend<TNode, TNodeComponent> : IGraphBackend<TNode, TNodeComponent> where TNode : INode<GraphRuntime<TNode>>
    {
#if UNITY_EDITOR
        IReadOnlyDictionary<NodeId, UnityEditor.SerializedObject> SerializedObjects { get; }
#endif
    }
}