using UnityEditor.Experimental.GraphView;
using UnityEngine;

#if UNITY_EDITOR

namespace GraphExt.Editor
{
    public interface IStickyNoteViewFactory : IGraphElementViewFactory
    {
        StickyNote Create(StickyNoteData data);
    }

    public class DefaultStickyNoteViewFactory : IStickyNoteViewFactory
    {
        public StickyNote Create(StickyNoteData data)
        {
            var view = new StickyNote();
            view.SetPosition(new Rect(data.X, data.Y, data.Width, data.Height));
            view.title = data.Title;
            view.contents = data.Content;
            view.theme = data.Theme.ToEditor();
            view.fontSize = data.FontSize.ToEditor();
            return view;
        }
    }
}

#endif