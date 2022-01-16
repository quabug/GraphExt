using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class TitleProperty : INodeProperty
    {
        public int Order => 0;
        public string Value;
        public TitleProperty(string value) => Value = value;

        public class Factory : INodePropertyFactory
        {
            public INodeProperty Create(MemberInfo memberInfo, object nodeObj, NodeId nodeId, SerializedProperty fieldProperty = null, SerializedProperty nodeProperty = null)
            {
                return new TitleProperty(memberInfo.GetValue<string>(nodeObj));
            }
        }

        [UsedImplicitly]
        private class ViewFactory : SingleNodePropertyViewFactory<TitleProperty>
        {
            protected override VisualElement CreateView(Node node, TitleProperty property, INodePropertyViewFactory _)
            {
                var titleLabel = new Label(property.Value);
                titleLabel.name = "title-property";
                titleLabel.style.height = 40;
                titleLabel.style.flexGrow = 1;
                titleLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                titleLabel.style.fontSize = new StyleLength(14);
                titleLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                return titleLabel;
            }
        }
    }
}
