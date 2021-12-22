using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodePropertyAddedEvent : EventBase<NodePropertyAddedEvent>
    {
        public Node Node { get; private set; }

        public static NodePropertyAddedEvent GetPooled(Node node, VisualElement element)
        {
          var pooled = EventBase<NodePropertyAddedEvent>.GetPooled();
          pooled.Node = node;
          pooled.target = element;
          return pooled;
        }
    }

    public class NodePositionChangeEvent : EventBase<NodePositionChangeEvent>
    {
        public Vector2 Position { get; private set; }

        public static NodePositionChangeEvent GetPooled(Node node)
        {
          var pooled = EventBase<NodePositionChangeEvent>.GetPooled();
          pooled.Position = node.GetVector2Position();
          pooled.target = node;
          return pooled;
        }
    }

    public abstract class NodeEventElement<T> : VisualElement, IDisposable where T : EventBase<T>, new()
    {
        protected Node Node { get; private set; }

        public NodeEventElement()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            if (evt is NodePropertyAddedEvent added)
            {
                Node = added.Node;
                Node.RegisterCallback<T>(OnEvent);
                OnInit();
            }
        }

        public void Dispose()
        {
            Node.UnregisterCallback<T>(OnEvent);
        }

        protected virtual void OnInit() {}
        protected abstract void OnEvent(T @event);
    }
}