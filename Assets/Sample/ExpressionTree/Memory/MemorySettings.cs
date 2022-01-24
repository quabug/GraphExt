using System.Collections.Generic;
using GraphExt;
using JetBrains.Annotations;
using UnityEngine;

public class VisualMemorySaveLoadMenu : MemorySaveLoadMenu<IVisualNode>
{
    public VisualMemorySaveLoadMenu([NotNull] GraphRuntime<IVisualNode> graphRuntime, [NotNull] IReadOnlyDictionary<NodeId, Vector2> nodePositions, [NotNull] IReadOnlyDictionary<StickyNoteId, StickyNoteData> notes, [NotNull] JsonFileInstaller jsonFileInstaller) : base(graphRuntime, nodePositions, notes, jsonFileInstaller)
    {
    }
}