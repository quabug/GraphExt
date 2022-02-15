using System;
using System.Collections.Generic;
using System.Linq;
using OneShot;

namespace GraphExt.Editor
{
    public static class OneShotExtension
    {
        public static void RegisterInstanceWithBaseAndInterfaces<T>(this Container container, T instance)
        {
            container.RegisterInstance(instance);
            foreach (var interfaceType in instance.GetType().GetBaseClassesAndInterfaces())
                container.RegisterInstance(interfaceType, instance);
        }

        public static void RegisterBaseAndInterfaces<T>(this Container container)
        {
            foreach (var interfaceType in typeof(T).GetBaseClassesAndInterfaces())
                container.Register(interfaceType, () => container.Resolve<T>());
        }

        private static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type)
        {
            return type.BaseType == typeof(object) ? type.GetInterfaces() : type.BaseType.Yield()
                .Concat(type.GetInterfaces())
                .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                .Distinct()
            ;
        }

        public static void RegisterDictionaryInstance<TId, TView>(this Container container, Dictionary<TId, TView> map)
        {
            container.RegisterInstance(map);
            container.Register<IDictionary<TId, TView>>(container.Resolve<Dictionary<TId, TView>>);
            container.Register<IReadOnlyDictionary<TId, TView>>(container.Resolve<Dictionary<TId, TView>>);
        }

        public static void RegisterBiDictionaryInstance<TId, TView>(this Container container, BiDictionary<TId, TView> map)
        {
            container.RegisterInstance(map);
            container.Register<IBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register<IReadOnlyBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register<IDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register<IReadOnlyDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Forward);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Reverse);
        }

        public static void RegisterGraphRuntimeInstance<TNode>(this Container container, GraphRuntime<TNode> graphRuntime)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstanceWithBaseAndInterfaces(graphRuntime);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeIdMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().IdNodeMap);
            container.Register(() => container.Resolve<GraphRuntime<TNode>>().NodeMap);
            container.Register(() => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Keys);
            container.Register(() => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Values);
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
            container.Register(() => container.Resolve<IGraphBackend<TNode, TNodeComponent>>().Nodes);
            container.Register(() => container.Resolve<IGraphBackend<TNode, TNodeComponent>>().NodeMap);
            container.Register(() => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Forward);
            container.Register(() => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Reverse);
        }

        public static void RegisterSerializableGraphBackend<TNode, TNodeComponent>(
            this Container container,
            ISerializableGraphBackend<TNode, TNodeComponent> graph
        ) where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graph);
            container.RegisterGraphBackend(graph);
        }
    }
}
