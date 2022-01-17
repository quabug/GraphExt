using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeSerializedPositionProperty : INodeProperty
    {
        public int Order => 0;
        public SerializedProperty PositionProperty;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null, SerializedProperty nodeProperty = null)
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
                style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _node.RegisterCallback<ElementMovedEvent>(OnNodeMoved);
            }

            public void Dispose()
            {
                _node.UnregisterCallback<ElementMovedEvent>(OnNodeMoved);
            }

            private void OnNodeMoved(ElementMovedEvent evt)
            {
                evt.StopImmediatePropagation();
                _positionProperty.vector2Value = evt.Position;
            }
        }

        private class ViewFactory : SingleNodePropertyViewFactory<NodeSerializedPositionProperty>
        {
            protected override VisualElement CreateView(Node node, NodeSerializedPositionProperty property, INodePropertyViewFactory factory)
            {
                return new View(node, property.PositionProperty);
            }
        }
    }
}