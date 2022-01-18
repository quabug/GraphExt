using System;

namespace GraphExt
{
    public enum StickyNoteTheme
    {
        Light, Dark
    }

    [Serializable]
    public struct StickyNoteData : IEquatable<StickyNoteData>
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public string Title;
        public string Content;
        public StickyNoteTheme Theme;
        public int FontSize;

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

        public bool Equals(StickyNoteData other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height) && Title == other.Title && Content == other.Content && Theme == other.Theme && FontSize == other.FontSize;
        }

        public override bool Equals(object obj)
        {
            return obj is StickyNoteData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Theme;
                hashCode = (hashCode * 397) ^ FontSize;
                return hashCode;
            }
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