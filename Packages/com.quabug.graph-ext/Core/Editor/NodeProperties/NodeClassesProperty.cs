using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeClassesProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public string[] AdditionalClasses { get; }
        public NodeClassesProperty(params string[] classes) => AdditionalClasses = classes;

        public class ViewFactory : SingleNodePropertyViewFactory<NodeClassesProperty>
        {
            protected override VisualElement CreateView(Node node, NodeClassesProperty property, INodePropertyViewFactory factory)
            {
                foreach (var @class in property.AdditionalClasses) node.AddToClassList(@class);
                return null;
            }
        }
    }
}