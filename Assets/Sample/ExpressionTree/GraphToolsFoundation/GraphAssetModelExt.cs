using GraphExt.GTF.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

public class GraphAssetModelExt : GraphAssetModelExt<IVisualNode, GraphModelExt, NodeModelExt>
{
    [MenuItem("Assets/Create/GraphAsset")]
    public static void CreateAsset(MenuCommand menuCommand) => CreateGraph<GraphAssetModelExt, GraphWindowExt>();

    [OnOpenAsset(1)]
    public static bool OpenGraph(int instanceId, int line) => OpenGraphAsset<GraphAssetModelExt, GraphWindowExt>(instanceId, line);
}