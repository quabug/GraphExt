using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEngine;

public class VisualNodeMenuEntry : NodeMenuEntry<IVisualNode>
{
    public VisualNodeMenuEntry([NotNull] GraphRuntime<IVisualNode> graphRuntime, [NotNull] IDictionary<NodeId, Vector2> positions) : base(graphRuntime, positions)
    {
    }
}