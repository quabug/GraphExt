using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeSerializedPositionProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public SerializedProperty PositionProperty;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                Assert.IsNotNull(fieldProperty);
                Assert.IsTrue(fieldProperty.propertyType == SerializedPropertyType.Vector2);
                return new NodeSerializedPositionProperty { PositionProperty = fieldProperty };
            }
        }

        private class View : VisualElement, IDisposable
        {
            private readonly Node _node;
            private readonly SerializedProperty _positionProperty;

            public View([NotNull] Node node, [NotNull] SerializedProperty positionProperty)
            {
                _node = node;
                _positionProperty = positionProperty;
                SetPosition(positionProperty);
                style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _node.RegisterCallback<ElementMovedEvent>(OnNodeMoved);
                this.TrackPropertyValue(positionProperty, SetPosition);

                void SetPosition(SerializedProperty position)
                {
                    node.SetPosition(new Rect(position.vector2Value.x, position.vector2Value.y, 0, 0));
                }
            }

            public void Dispose()
            {
                _node.UnregisterCallback<ElementMovedEvent>(OnNodeMoved);
            }

            private void OnNodeMoved(ElementMovedEvent evt)
            {
                evt.StopImmediatePropagation();
                _positionProperty.vector2Value = evt.Position;
                _positionProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<NodeSerializedPositionProperty>
        {
            protected override VisualElement CreateView(Node node, NodeSerializedPositionProperty property, INodePropertyViewFactory factory)
            {
                return new View(node, property.PositionProperty);
            }
        }
    }
}