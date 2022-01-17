using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
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

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                var position = memberInfo.GetValue<Vector2>(nodeObj);
                return new NodePositionProperty(position.x, position.y);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<NodePositionProperty>
        {
            protected override VisualElement CreateView(UnityEditor.Experimental.GraphView.Node node, NodePositionProperty property, INodePropertyViewFactory _)
            {
                node.SetPosition(new Rect(property._x, property._y, 0, 0));
                return null;
            }
        }
    }
}