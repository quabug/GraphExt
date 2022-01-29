using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class TitleProperty : INodeProperty
    {
        public int Order { get; set; } = -1;
        public string Value;
        public TitleProperty(string value) => Value = value;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null)
            {
                return new TitleProperty(memberInfo.GetValue<string>(nodeObj));
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<TitleProperty>
        {
            protected override VisualElement CreateView(Node node, TitleProperty property, INodePropertyViewFactory _)
            {
                return new TitleLabel(property.Value);
            }
        }
    }
}
