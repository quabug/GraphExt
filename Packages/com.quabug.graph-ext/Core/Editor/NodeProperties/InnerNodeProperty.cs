using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class InnerNodeProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        private readonly INodeProperty[] _properties;

        public InnerNodeProperty(object nodeObj, NodeId nodeId, SerializedProperty nodeSerializedProperty)
        {
            _properties = NodePropertyUtility.CreateProperties(nodeObj, nodeId, name => nodeSerializedProperty?.FindPropertyRelative(name)).ToArray();
        }

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                return new InnerNodeProperty(memberInfo.GetValue(nodeObj), nodeId, fieldProperty);
            }
        }

        [UsedImplicitly]
        private class ViewFactory : NodePropertyViewFactory<InnerNodeProperty>
        {
            protected override IEnumerable<VisualElement> CreateViews(Node node, InnerNodeProperty property, INodePropertyViewFactory factory)
            {
                return property._properties.SelectMany(p => factory.Create(node, p, factory));
            }
        }
    }
}