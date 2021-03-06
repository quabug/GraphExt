using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface IMenuEntry
    {
        void MakeEntry(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView graph,
            [NotNull] ContextualMenuPopulateEvent evt,
            [NotNull] GenericMenu menu
        );
    }
}