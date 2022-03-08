using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface INodeViewFactory : IGraphElementViewFactory
    {
        [NotNull] Node Create(NodeData nodeData);
    }

    [Serializable]
    public class DefaultNodeViewFactory : INodeViewFactory
    {
        public GroupNodePropertyViewFactory NodePropertyViewFactory = new GroupNodePropertyViewFactory
        {
            Factories = new INodePropertyViewFactory[]{ new DefaultPropertyViewFactory() }
        };

        public Node Create(NodeData data)
        {
            // HACK: use hard-coded guid to locate uxml to avoid error on different path of remote package.
            var nodeUxml = UnityEditor.AssetDatabase.GUIDToAssetPath("5744524cc2f5b4fc5ad6b88b4f720e5f");
            var nodeView = new Node(nodeUxml);
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