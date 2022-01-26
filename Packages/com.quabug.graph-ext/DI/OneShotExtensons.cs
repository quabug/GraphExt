using System;
using System.Collections.Generic;
using OneShot;

namespace GraphExt.Editor
{
    public static class OneShotExtension
    {
        public static void RegisterDictionaryInstance<TId, TView>(this Container container, Dictionary<TId, TView> map)
        {
            container.RegisterInstance(map);
            container.RegisterInstance<IDictionary<TId, TView>>(map);
            container.RegisterInstance<IReadOnlyDictionary<TId, TView>>(map);
        }

        public static void RegisterBiDictionaryInstance<TId, TView>(this Container container, BiDictionary<TId, TView> map)
        {
            container.RegisterInstance(map);
            container.Register<IBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register<IReadOnlyBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Forward);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Reverse);
        }

        public static void RegisterGraphRuntimeInstance<TNode>(this Container container, GraphRuntime<TNode> graphRuntime)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graphRuntime);
            container.Register<IReadOnlyGraphRuntime<TNode>>(container.Resolve<GraphRuntime<TNode>>);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeIdMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().IdNodeMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeMap);
            container.Register(() => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Keys);
            container.Register(() => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Values);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().Edges);
            container.Register<IReadOnlyCollection<EdgeId>>(container.Resolve<IReadOnlySet<EdgeId>>);
            container.Register<IEnumerable<EdgeId>>(container.Resolve<IReadOnlySet<EdgeId>>);
        }

        public static void RegisterTypeNameSingleton<T>(this Container container, string typeName) where T : class
        {
            var type = Type.GetType(typeName);
            container.RegisterSingleton(() => (T) container.Instantiate(type));
        }

        public static void RegisterTypeNameArraySingleton<T>(this Container container, IEnumerable<string> typeNames) where T : class
        {
            foreach (var type in typeNames) container.RegisterTypeNameSingleton<T>(type);
        }

        public static void RegisterGraphBackend<TNode, TNodeComponent>(this Container container, IGraphBackend<TNode, TNodeComponent> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graph);
            container.RegisterGraphRuntimeInstance(graph.Runtime);
            container.RegisterInstance(graph.Nodes);
            container.RegisterInstance(graph.NodeMap);
            container.Register(() => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Forward);
            container.Register(() => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Reverse);
        }

        public static void RegisterSerializableGraphBackend<TNode, TNodeComponent>(
            this Container container,
            ISerializableGraphBackend<TNode, TNodeComponent> graph
        ) where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterGraphBackend(graph);
#if UNITY_EDITOR
            container.RegisterInstance(graph.SerializedObjects);
#endif
        }
    }
}
