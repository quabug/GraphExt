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
            style.unityTextAlign = TextAnchor.MiddleCenter;
            style.fontSize = 14;
            style.unityFontStyleAndWeight = FontStyle.Bold;
        }
    }
}