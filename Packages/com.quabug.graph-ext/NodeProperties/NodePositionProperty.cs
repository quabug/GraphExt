using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodePositionProperty : INodeProperty
    {
        private readonly float _x;
        private readonly float _y;

        public NodePositionProperty(float x, float y)
        {
            _x = x;
            _y = y;
        }


        public class Factory : NodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, NodePositionProperty property, Editor.INodePropertyViewFactory _)
            {
                var view = new VisualElement();
                view.name = "node-position";
                node.SetPosition(new Rect(property._x, property._y, 0, 0));
                return view;
            }
        }
    }
}