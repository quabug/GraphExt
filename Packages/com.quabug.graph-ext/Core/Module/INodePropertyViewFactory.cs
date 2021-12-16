using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public interface INodePropertyViewFactory
    {
        [CanBeNull] VisualElement Create([NotNull] INodeProperty property, [NotNull] INodePropertyViewFactory factory);

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

    [Serializable]
    public class GroupNodePropertyViewFactory : INodePropertyViewFactory
    {
        [SerializeReference, SerializeReferenceDrawer(Nullable = false)]
        public INodePropertyViewFactory[] Factories;

        public VisualElement Create(INodeProperty property, INodePropertyViewFactory factory)
        {
            return Factories.Append(factory).Select(f => f.Create(property, this)).FirstOrDefault(element => element != null);
        }
    }
}