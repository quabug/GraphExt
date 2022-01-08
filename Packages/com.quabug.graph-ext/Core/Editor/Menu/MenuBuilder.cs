using JetBrains.Annotations;
using Shtif;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class MenuBuilder
    {
        [NotNull] private readonly IMenuEntry[] _entries;
        public MenuBuilder([NotNull] IMenuEntry[] entries) => _entries = entries;

        public void Build([NotNull] GraphView graphView, ContextualMenuPopulateEvent evt)
        {
            var context = new GenericMenu();
            foreach (var menu in _entries) menu.MakeEntry(graphView, evt, context);
            var popup = GenericMenuPopup.Get(context, "");
            popup.showSearch = true;
            popup.showTooltip = false;
            popup.resizeToContent = true;
            popup.Show(evt.mousePosition);
        }
    }
}