using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class SerializedProperty : INodeProperty
    {
        public UnityEditor.SerializedProperty Value;
        public IEnumerable<IPortModule> Ports => Enumerable.Empty<IPortModule>();

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is SerializedProperty p ? new PropertyField(p.Value) : null;
            }
        }
    }
}