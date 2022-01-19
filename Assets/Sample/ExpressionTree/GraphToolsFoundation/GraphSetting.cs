using GraphExt.GTF.Editor;
using UnityEditor;

public class GraphWindowExt : GraphWindowExt<IVisualNode, GraphModelExt, NodeModelExt>
{
    protected override string _EditorToolName => "GraphToolsFoundation";

    [MenuItem("Graph/GraphToolsFoundation", false)]
    public static void ShowGraphWindow()
    {
        FindOrCreateGraphWindow<GraphWindowExt>();
    }
}

public class GraphModelExt : GraphModelExt<IVisualNode, NodeModelExt> {}
public class NodeModelExt : NodeModelExt<IVisualNode> {}

public class NodeItems : NodeItems<IVisualNode, NodeModelExt> {}
