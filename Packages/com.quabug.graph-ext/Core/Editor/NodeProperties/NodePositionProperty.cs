using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodePositionProperty : INodeProperty
    {
        public int Order => 0;
        private readonly float _x;
        private readonly float _y;

        public NodePositionProperty(float x, float y)
        {
            _x = x;
            _y = y;
        }

        [UsedImplicitly]
        private class Factory : NodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, NodePositionProperty property, INodePropertyViewFactory _)
            {
                node.SetPosition(new Rect(property._x, property._y, 0, 0));
                return null;
            }
        }
    }
}