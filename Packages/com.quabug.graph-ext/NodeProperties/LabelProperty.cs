using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class LabelProperty : INodeProperty
    {
        public IEnumerable<IPortModule> Ports => Enumerable.Empty<IPortModule>();
        public string Value;
        public LabelProperty(string value) => Value = value;

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                return property is LabelProperty label ? new Label(label.Value) : null;
            }
        }
    }
}