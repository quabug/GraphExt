using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class TitleLabel : Label
    {
        public TitleLabel() : this("") {}

        public TitleLabel(string text) : base(text)
        {
            name = "title-property";
            style.height = 40;
            style.flexGrow = 1;
            style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter);
            style.fontSize = new StyleLength(14);
            style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        }
    }
}