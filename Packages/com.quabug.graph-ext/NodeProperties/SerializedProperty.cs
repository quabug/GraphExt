#if UNITY_EDITOR

using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;

        public class Factory : Editor.NodePropertyViewFactory<SerializedProperty>
        {
            protected override VisualElement Create(Node node, SerializedProperty property, INodePropertyViewFactory _)
            {
                return new UnityEditor.UIElements.PropertyField(property.Value);
            }
        }
    }
}

#endif
