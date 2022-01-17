using System;
using UnityEngine;

namespace GraphExt
{
    public enum StickyNoteTheme
    {
        Light, Dark
    }

    [Serializable]
    public class StickyNoteData
    {
        public Rect Rect;
        public string Title;
        public string Content;
        public StickyNoteTheme Theme;
        public int FontSize;
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