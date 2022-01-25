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

        public static void RegisterGraphView(this Container container)
        {
            container.RegisterSingleton(() =>
            {
                Func<GraphView.FindCompatiblePorts, GraphView> create = container.Resolve<IGraphViewFactory>().Create;
                return container.Call<GraphView>(create);
            });
            container.Register<UnityEditor.Experimental.GraphView.GraphView>(container.Resolve<GraphView>);
        }

        public static void RegisterGraphRuntimeInstance<TNode>(this Container container, GraphRuntime<TNode> graphRuntime)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graphRuntime);
            container.Register<IReadOnlyGraphRuntime<TNode>>(container.Resolve<GraphRuntime<TNode>>);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeIdMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().Edges);
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
            container.RegisterInstance(graph.NodeObjectMap);
            container.RegisterInstance(graph.ObjectNodeMap);
        }
    }
}
