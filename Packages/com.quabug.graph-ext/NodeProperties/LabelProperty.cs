using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelProperty : INodeProperty
    {
        public string Value;
        public LabelProperty(string value) => Value = value;

#if UNITY_EDITOR
        public class Factory : Editor.NodePropertyViewFactory<LabelProperty>
        {
            protected override VisualElement Create(Node node, LabelProperty property, INodePropertyViewFactory _)
            {
                return new Label(property.Value);
            }
        }
#endif
    }
}