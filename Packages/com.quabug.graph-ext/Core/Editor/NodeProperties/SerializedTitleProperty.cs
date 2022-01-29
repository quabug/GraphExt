using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class SerializedTitleProperty : INodeProperty
    {
        public int Order { get; set; } = 0;
        public SerializedProperty Property;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty)
            {
                Assert.IsNotNull(fieldProperty);
                Assert.AreEqual(SerializedPropertyType.String, fieldProperty.propertyType);
                return new SerializedTitleProperty { Property = fieldProperty };
            }
        }

        private class View : TitleLabel
        {
            public View(SerializedProperty property) : base(property.stringValue)
            {
                Assert.AreEqual(SerializedPropertyType.String, property.propertyType);
                this.TrackPropertyValue(property, p => text = p.stringValue);
            }
        }

        [UsedImplicitly]
        public class ViewFactory : SingleNodePropertyViewFactory<SerializedTitleProperty>
        {
            protected override VisualElement CreateView(Node node, SerializedTitleProperty property, INodePropertyViewFactory factory)
            {
                return new View(property.Property);
            }
        }
    }
}