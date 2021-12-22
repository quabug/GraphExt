using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface INodePropertyViewFactory
    {
        [CanBeNull] VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory);

        public sealed class Null : INodePropertyViewFactory
        {
            public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
            {
                return null;
            }
        }

        public sealed class Exception : INodePropertyViewFactory
        {
            public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
            {
                throw new NotImplementedException();
            }
        }
    }

    public abstract class NodePropertyViewFactory<T> : INodePropertyViewFactory where T : INodeProperty
    {
        public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return property is T p ? Create(node, p, factory) : null;
        }

        protected abstract VisualElement Create(Node node, [NotNull] T property, INodePropertyViewFactory factory);
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

        public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return _groupFactory.Create(node, property, factory);
        }
    }

    [Serializable]
    public class GroupNodePropertyViewFactory : INodePropertyViewFactory
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public INodePropertyViewFactory[] Factories;

        public VisualElement Create(Node node, INodeProperty property, INodePropertyViewFactory factory)
        {
            return property == null ? null : Factories.Select(f => f.Create(node, property, this)).FirstOrDefault(element => element != null);
        }
    }
}