using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class NodePositionProperty : INodeProperty
    {
        private readonly Func<Vector2> _getter;
        private readonly Action<Vector2> _setter;

        public NodePositionProperty([NotNull] Func<Vector2> getter, [NotNull] Action<Vector2> setter)
        {
            _getter = getter;
            _setter = setter;
        }

#if UNITY_EDITOR
        public class EventView : Editor.NodeEventElement<Editor.NodePositionChangeEvent>
        {
            private NodePositionProperty _property;

            public EventView(NodePositionProperty property)
            {
                _property = property;
            }

            protected override void OnInit()
            {
                Node.SetPosition(new Rect(_property._getter(), Vector2.zero));
            }

            protected override void OnEvent(Editor.NodePositionChangeEvent @event)
            {
                _property._setter(Node.GetPosition().position);
            }
        }

        public class Factory : NodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement Create(NodePositionProperty property, INodePropertyViewFactory _)
            {
                return new EventView(property);
            }
        }
#endif
    }
}