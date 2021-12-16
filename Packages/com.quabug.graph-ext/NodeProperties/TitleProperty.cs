using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class TitleProperty : INodeProperty
    {
        public string Value;
        public TitleProperty(string value) => Value = value;
        public IEnumerable<IPortModule> Ports => Enumerable.Empty<IPortModule>();

        public class Factory : INodePropertyViewFactory
        {
            public VisualElement Create(INodeProperty property, INodePropertyViewFactory _)
            {
                if (property is TitleProperty title)
                {
                    var titleLabel = new Label(title.Value);
                    titleLabel.name = "title-property";
                    titleLabel.style.height = 40;
                    titleLabel.style.flexGrow = 1;
                    titleLabel.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
                    titleLabel.style.fontSize = new StyleLength(14);
                    titleLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                    return titleLabel;
                }
                return null;
            }
        }
    }
}
