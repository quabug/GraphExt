using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface IMenuEntry
    {
        void MakeEntry([NotNull] GraphView graph, [NotNull] ContextualMenuPopulateEvent evt, [NotNull] GenericMenu menu);
    }
}