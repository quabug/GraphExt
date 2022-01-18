using System;

namespace GraphExt
{
    public enum StickyNoteTheme
    {
        Light, Dark
    }

    [Serializable]
    public readonly struct StickyNoteData
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;
        public readonly string Title;
        public readonly string Content;
        public readonly StickyNoteTheme Theme;
        public readonly int FontSize;

        public StickyNoteData(float x, float y, float width, float height, string title, string content, StickyNoteTheme theme, int fontSize)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Title = title;
            Content = content;
            Theme = theme;
            FontSize = fontSize;
        }
    }

    internal static class StickyNoteDataExtension
    {
#if UNITY_EDITOR
        public static UnityEditor.Experimental.GraphView.StickyNoteTheme ToEditor(this StickyNoteTheme theme) =>
            theme switch
            {
                StickyNoteTheme.Light => UnityEditor.Experimental.GraphView.StickyNoteTheme.Classic,
                StickyNoteTheme.Dark => UnityEditor.Experimental.GraphView.StickyNoteTheme.Black,
                _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
            };

        public static StickyNoteTheme ToRuntime(this UnityEditor.Experimental.GraphView.StickyNoteTheme theme) =>
            theme switch
            {
                UnityEditor.Experimental.GraphView.StickyNoteTheme.Classic => StickyNoteTheme.Light,
                UnityEditor.Experimental.GraphView.StickyNoteTheme.Black => StickyNoteTheme.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
            };

        public static UnityEditor.Experimental.GraphView.StickyNoteFontSize ToEditor(this int fontSize) =>
            (UnityEditor.Experimental.GraphView.StickyNoteFontSize)fontSize;

        public static int ToRuntime(this UnityEditor.Experimental.GraphView.StickyNoteFontSize fontSize) => (int)fontSize;
#endif
    }
}