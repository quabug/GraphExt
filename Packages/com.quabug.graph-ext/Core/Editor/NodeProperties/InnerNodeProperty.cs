using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class InnerNodeProperty : INodeProperty
    {
        public int Order { get; }
        private readonly INodeProperty[] _properties;

        public InnerNodeProperty(object nodeObj, NodeId nodeId, SerializedProperty nodeSerializedProperty)
        {
            _properties = NodePropertyUtility.CreateProperties(nodeObj, nodeId, nodeSerializedProperty).ToArray();
        }

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null, SerializedProperty nodeProperty = null)
            {
                return new InnerNodeProperty(memberInfo.GetValue(nodeObj), nodeId, fieldProperty);
            }
        }

        public class ViewFactory : NodePropertyViewFactory<InnerNodeProperty>
        {
            protected override IEnumerable<VisualElement> CreateViews(Node node, InnerNodeProperty property, INodePropertyViewFactory factory)
            {
                return property._properties.SelectMany(p => factory.Create(node, p, factory));
            }
        }
    }
}