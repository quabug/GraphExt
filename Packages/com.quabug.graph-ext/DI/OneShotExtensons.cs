using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace GraphExt.Editor
{
    public static class OneShotExtension
    {
        public static void RegisterInstanceWithBaseAndInterfaces<T>(this Container container, T instance)
        {
            var builder = container.RegisterInstance(instance).AsSelf();
            foreach (var interfaceType in instance.GetType().GetBaseClassesAndInterfaces()) builder.As(interfaceType);
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
            container.RegisterInstance(map).AsSelf().As<IDictionary<TId, TView>>().As<IReadOnlyDictionary<TId, TView>>();
        }

        public static void RegisterBiDictionaryInstance<TId, TView>(this Container container, BiDictionary<TId, TView> map)
        {
            container.RegisterInstance(map).AsSelf()
                .As<IBiDictionary<TId, TView>>()
                .As<IBiDictionary<TId, TView>>()
                .As<IReadOnlyBiDictionary<TId, TView>>()
                .As<IDictionary<TId, TView>>()
                .As<IReadOnlyDictionary<TId, TView>>()
            ;
            container.Register((resolveContainer, contractType) => container.Resolve<BiDictionary<TId, TView>>().Forward).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<BiDictionary<TId, TView>>().Reverse).AsSelf();
        }

        public static void RegisterGraphRuntimeInstance<TNode>(this Container container, GraphRuntime<TNode> graphRuntime)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstanceWithBaseAndInterfaces(graphRuntime);
            container.Register((resolveContainer, contractType) => container.Resolve<GraphRuntime<TNode>>().NodeIdMap).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<GraphRuntime<TNode>>().IdNodeMap).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<GraphRuntime<TNode>>().NodeMap).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Keys).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<IReadOnlyDictionary<NodeId, TNode>>().Values).AsSelf();
        }

        public static void RegisterTypeNameSingleton<T>(this Container container, string typeName) where T : class
        {
            var type = Type.GetType(typeName);
            Assert.IsNotNull(type);
            container.Register(type).Singleton().As<T>();
        }

        public static void RegisterTypeNameArraySingleton<T>(this Container container, IEnumerable<string> typeNames) where T : class
        {
            foreach (var type in typeNames) container.RegisterTypeNameSingleton<T>(type);
        }

        public static void RegisterGraphBackend<TNode, TNodeComponent>(this Container container, IGraphBackend<TNode, TNodeComponent> graph)
            where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graph).As<IGraphBackend<TNode, TNodeComponent>>();
            container.RegisterGraphRuntimeInstance(graph.Runtime);
            container.Register((resolveContainer, contractType) => container.Resolve<IGraphBackend<TNode, TNodeComponent>>().Nodes).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<IGraphBackend<TNode, TNodeComponent>>().NodeMap).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Forward).AsSelf();
            container.Register((resolveContainer, contractType) => container.Resolve<IReadOnlyBiDictionary<NodeId, TNodeComponent>>().Reverse).AsSelf();
        }

        public static void RegisterSerializableGraphBackend<TNode, TNodeComponent>(
            this Container container,
            ISerializableGraphBackend<TNode, TNodeComponent> graph
        ) where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterInstance(graph).As<ISerializableGraphBackend<TNode, TNodeComponent>>();
            container.RegisterGraphBackend(graph);
        }
    }
}
