using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface INodeViewFactory
    {
        [NotNull] Node Create(NodeData nodeData);
    }

    public class DefaultNodeViewFactory : INodeViewFactory
    {
        public GroupNodePropertyViewFactory NodePropertyViewFactory = new GroupNodePropertyViewFactory
        {
            Factories = new INodePropertyViewFactory[]{ new DefaultPropertyViewFactory() }
        };

        public Node Create(NodeData data)
        {
            var defaultUxml = Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml");
            var nodeView = new Node(defaultUxml);
            var container = nodeView.ContentContainer();
            var orders = new List<int>();
            foreach (var property in data.Properties)
            {
                var propertyViews = CreatePropertyView(property);
                foreach (var view in propertyViews.Where(view => view != null))
                {
                    var index = orders.FindLastIndex(order => order <= property.Order);
                    orders.Insert(index + 1, property.Order);
                    container.Insert(index + 1, view);
                }
            }
            return nodeView;

            IEnumerable<VisualElement> CreatePropertyView(INodeProperty property)
            {
                return NodePropertyViewFactory.Create(nodeView, property, NodePropertyViewFactory);
            }
        }

    }
}