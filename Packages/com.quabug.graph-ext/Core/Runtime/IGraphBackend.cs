using System.Collections.Generic;

namespace GraphExt
{
    public interface IGraphBackend<TNode, TNodeComponent> where TNode : INode<GraphRuntime<TNode>>
    {
        GraphRuntime<TNode> Runtime { get; }
        public IReadOnlyList<TNodeComponent> Nodes { get; }
        public IReadOnlyDictionary<NodeId, TNodeComponent> NodeObjectMap { get; }
        public IReadOnlyDictionary<TNodeComponent, NodeId> ObjectNodeMap { get; }
    }

    public interface ISerializableGraphBackend<TNode, TNodeComponent> : IGraphBackend<TNode, TNodeComponent> where TNode : INode<GraphRuntime<TNode>>
    {
#if UNITY_EDITOR
        IReadOnlyDictionary<NodeId, UnityEditor.SerializedObject> SerializedObjects { get; }
#endif
    }
}