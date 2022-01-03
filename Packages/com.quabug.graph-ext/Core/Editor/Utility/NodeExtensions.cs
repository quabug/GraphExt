using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    internal static class NodeExtensions
    {
        public static Vector2 GetVector2Position(this Node node) => node.GetPosition().position;
        public static void SetPosition(this Node node, Vector2 position) => node.SetPosition(new Rect(position, Vector2.zero));
        public static VisualElement ContentContainer(this Node node) => node.mainContainer.Q("contents");

        public static Direction ToEditor(this PortDirection direction) => direction switch
        {
            PortDirection.Input => Direction.Input,
            PortDirection.Output => Direction.Output,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

        public static Orientation ToEditor(this PortOrientation orientation) => orientation switch
        {
            PortOrientation.Horizontal => Orientation.Horizontal,
            PortOrientation.Vertical => Orientation.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };
    }
}