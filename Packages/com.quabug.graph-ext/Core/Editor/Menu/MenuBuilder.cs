using JetBrains.Annotations;
using Shtif;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class MenuBuilder
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _graphView;
        [NotNull] private readonly IMenuEntry[] _entries;

        public MenuBuilder([NotNull] UnityEditor.Experimental.GraphView.GraphView graphView, [NotNull] IMenuEntry[] entries)
        {
            _graphView = graphView;
            _entries = entries;
            graphView.RegisterCallback<ContextualMenuPopulateEvent>(OnContextualMenu);
        }

        public void OnContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.StopPropagation();
            var context = new GenericMenu();
            foreach (var menu in _entries) menu.MakeEntry(_graphView, evt, context);
            var popup = GenericMenuPopup.Get(context, "");
            popup.showSearch = true;
            popup.showTooltip = false;
            popup.resizeToContent = true;
            popup.Show(evt.mousePosition);
        }
    }
}