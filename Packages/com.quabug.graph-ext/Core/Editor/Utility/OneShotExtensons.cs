using System;
using System.Collections.Generic;
using System.Linq;
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
            container.RegisterInstance<IBiDictionary<TId, TView>>(map);
            container.RegisterInstance<IReadOnlyBiDictionary<TId, TView>>(map);
            container.RegisterInstance(map.Forward);
            container.RegisterInstance(map.Reverse);
        }

        public static void RegisterBiDictionaryInterface<TId, TView>(this Container container)
        {
            container.Register<IBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register<IReadOnlyBiDictionary<TId, TView>>(container.Resolve<BiDictionary<TId, TView>>);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Forward);
            container.Register(() => container.Resolve<BiDictionary<TId, TView>>().Reverse);
        }

        public static void RegisterGraphView(this Container container)
        {
            container.RegisterSingleton<GraphView>();
            container.Register<UnityEditor.Experimental.GraphView.GraphView>(container.Resolve<GraphView>);
        }

        public static void RegisterGraphRuntime<TNode>(this Container container) where TNode : INode<GraphRuntime<TNode>>
        {
            container.RegisterSingleton<GraphRuntime<TNode>>();
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
            container.RegisterSingleton(() => typeNames
                .Select(Type.GetType)
                .Select(container.Instantiate)
                .OfType<T>()
                .ToArray()
            );
        }
    }
}
