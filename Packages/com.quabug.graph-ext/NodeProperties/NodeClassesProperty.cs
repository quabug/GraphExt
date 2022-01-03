using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeClassesProperty : INodeProperty
    {
        public int Order => 0;
        public string[] AdditionalClasses { get; }
        public NodeClassesProperty(IEnumerable<string> classes) => AdditionalClasses = classes.ToArray();

        public class ViewFactory : NodePropertyViewFactory<NodeClassesProperty>
        {
            protected override VisualElement Create(Node node, NodeClassesProperty property, INodePropertyViewFactory factory)
            {
                foreach (var @class in property.AdditionalClasses) node.AddToClassList(@class);
                return null;
            }
        }
    }
}