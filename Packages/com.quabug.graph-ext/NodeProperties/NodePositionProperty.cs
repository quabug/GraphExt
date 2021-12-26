using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodePositionProperty : INodeProperty
    {
        private readonly Vector2 _initPosition;
        private readonly Action<Vector2> _setter;

        public NodePositionProperty(Vector2 initPosition, [NotNull] Action<Vector2> setter)
        {
            _initPosition = initPosition;
            _setter = setter;
        }

        public class EventView : NodeEventElement<NodePositionChangeEvent>
        {
            private readonly NodePositionProperty _property;

            public EventView(UnityEditor.Experimental.GraphView.Node node, NodePositionProperty property) : base(node)
            {
                name = "node-position-event";
                _property = property;
                Node.SetPosition(new Rect(_property._initPosition, Vector2.zero));
            }

            protected override void OnEvent(NodePositionChangeEvent @event)
            {
                _property._setter(Node.GetPosition().position);
            }
        }

        public class Factory : NodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement Create(UnityEditor.Experimental.GraphView.Node node, NodePositionProperty property, Editor.INodePropertyViewFactory _)
            {
                return new EventView(node, property);
            }
        }
    }
}