using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
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
        protected Node Node { get; }

        public NodeEventElement(Node node)
        {
            Node = node;
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            Node.RegisterCallback<T>(OnEvent);
        }

        public void Dispose()
        {
            Node.UnregisterCallback<T>(OnEvent);
        }

        protected abstract void OnEvent(T @event);
    }
}