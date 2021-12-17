using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public interface INodePropertyViewFactory
    {
        [CanBeNull] VisualElement Create(INodeProperty property, INodePropertyViewFactory factory);

        public sealed class Null : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
            {
                return null;
            }
        }

        public sealed class Exception : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
            {
                throw new NotImplementedException();
            }
        }
    }

    public abstract class NodePropertyViewFactory<T> : INodePropertyViewFactory where T : INodeProperty
    {
        public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
        {
            return property is T p ? Create(p, factory) : null;
        }

        protected abstract VisualElement Create([NotNull] T property, INodePropertyViewFactory factory);
    }

    public class DefaultPropertyViewFactory : INodePropertyViewFactory
    {
        private static GroupNodePropertyViewFactory _groupFactory;

        static DefaultPropertyViewFactory()
        {
            var factories = TypeCache.GetTypesDerivedFrom(typeof(NodePropertyViewFactory<>))
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .Select(type => (INodePropertyViewFactory)Activator.CreateInstance(type))
                .ToArray()
            ;
            _groupFactory = new GroupNodePropertyViewFactory { Factories = factories };
        }

        public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
        {
            return _groupFactory.Create(property, factory);
        }
    }

    [Serializable]
    public class GroupNodePropertyViewFactory : INodePropertyViewFactory
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public INodePropertyViewFactory[] Factories;

        public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
        {
            return property == null ? null : Factories.Select(f => f.Create(property, this)).FirstOrDefault(element => element != null);
        }
    }
}