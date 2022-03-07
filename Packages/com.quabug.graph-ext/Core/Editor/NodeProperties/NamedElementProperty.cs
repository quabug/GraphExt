using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NamedElementProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public string Name { get; }
        public NamedElementProperty(string name) => Name = name;

        private class ViewFactory : SingleNodePropertyViewFactory<NamedElementProperty>
        {
            protected override VisualElement CreateView(Node node, NamedElementProperty property, INodePropertyViewFactory factory)
            {
                return new VisualElement { name = property.Name };
            }
        }
    }
}