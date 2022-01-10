using GraphExt.Editor;
using UnityEditor;
using UnityEngine;

public class MemoryExpressionTreeWindow : BaseGraphWindow
{
    private MemoryGraphSetup<IVisualNode> _graphSetup;
    private MenuBuilder _menuBuilder;

    public TextAsset JsonFile;

    public void Reset()
    {
        CreateGUI();
    }

    [MenuItem("Graph/Memory Expression Tree")]
    public static void OpenWindow()
    {
        OpenWindow<MemoryExpressionTreeWindow>("Memory");
    }

    protected override void CreateGUI()
    {
        _graphSetup?.Dispose();
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            _graphSetup = new MemoryGraphSetup<IVisualNode>(graphRuntime, nodePositions);
        }
        else
        {
            _graphSetup = new MemoryGraphSetup<IVisualNode>();
        }

        _menuBuilder = new MenuBuilder(_graphSetup.GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_graphSetup.GraphRuntime, _graphSetup.NodeViews),
            new SelectionEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodeViews, _graphSetup.EdgeViews),
            new NodeMenuEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions),
            new MemorySaveLoadMenu<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions)
        });

        ReplaceGraphView(_graphSetup.GraphView);
    }

    private void Update()
    {
        _graphSetup.Tick();
    }

    private void OnDestroy()
    {
        _graphSetup.Dispose();
    }
}