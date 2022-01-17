using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface INodePropertyViewFactory
    {
        [NotNull] IEnumerable<VisualElement> Create(Node node, INodeProperty property, [NotNull] INodePropertyViewFactory factory);

        public sealed class Null : INodePropertyViewFactory
        {
            public IEnumerable<VisualElement> Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
            {
                return Enumerable.Empty<VisualElement>();
            }
        }

        public sealed class Exception : INodePropertyViewFactory
        {
            public IEnumerable<VisualElement> Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
            {
                throw new NotImplementedException();
            }
        }
    }

    public abstract class NodePropertyViewFactory<T> : INodePropertyViewFactory where T : INodeProperty
    {
        public IEnumerable<VisualElement> Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return property is T p ? CreateViews(node, p, factory) : Enumerable.Empty<VisualElement>();
        }

        protected abstract IEnumerable<VisualElement> CreateViews(Node node, [NotNull] T property, INodePropertyViewFactory factory);
    }

    public abstract class SingleNodePropertyViewFactory<T> : NodePropertyViewFactory<T> where T : INodeProperty
    {
        protected override IEnumerable<VisualElement> CreateViews(Node node, T property, INodePropertyViewFactory factory)
        {
            return CreateView(node, property, factory).Yield();
        }

        protected abstract VisualElement CreateView(Node node, [NotNull] T property, INodePropertyViewFactory factory);
    }

    public class DefaultPropertyViewFactory : INodePropertyViewFactory
    {
        private static readonly GroupNodePropertyViewFactory _groupFactory;

        static DefaultPropertyViewFactory()
        {
            var factories = UnityEditor.TypeCache.GetTypesDerivedFrom(typeof(NodePropertyViewFactory<>))
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Select(type => (INodePropertyViewFactory)Activator.CreateInstance(type))
                .ToArray()
            ;
            _groupFactory = new GroupNodePropertyViewFactory { Factories = factories };
        }

        public IEnumerable<VisualElement> Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return _groupFactory.Create(node, property, factory);
        }
    }

    [Serializable]
    public class GroupNodePropertyViewFactory : INodePropertyViewFactory
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public INodePropertyViewFactory[] Factories;

        public IEnumerable<VisualElement> Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            if (property == null) return Enumerable.Empty<VisualElement>();
            return Factories.Select(f => f.Create(node, property, factory)).FirstOrDefault(element => element.Any()) ??
                   Enumerable.Empty<VisualElement>();
        }
    }
}