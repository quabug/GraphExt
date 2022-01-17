using System;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class ElementMovedEvent : EventBase<ElementMovedEvent>
    {
        public Vector2 Position { get; private set; }

        public static ElementMovedEvent GetPooled(GraphElement element)
        {
            var pooled = EventBase<ElementMovedEvent>.GetPooled();
            pooled.Position = element.GetPosition().position;
            pooled.target = element;
            return pooled;
        }
    }

    public class ElementMovedEventEmitter : IDisposable
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;

        public ElementMovedEventEmitter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graphView
        )
        {
            _graphView = graphView;
            _graphView.graphViewChanged += OnGraphViewChanged;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange evt)
        {
            if (evt.movedElements != null)
            {
                foreach (var element in evt.movedElements)
                {
                    element.SendEvent(ElementMovedEvent.GetPooled(element));
                }
            }
            return evt;
        }

        public void Dispose()
        {
            _graphView.graphViewChanged -= OnGraphViewChanged;
        }
    }
}