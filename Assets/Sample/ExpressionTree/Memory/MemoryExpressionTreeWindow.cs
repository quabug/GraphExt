using GraphExt.Editor;
using UnityEditor;
using UnityEngine;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

public class MemoryExpressionTreeWindow : BaseGraphWindow
{
    private MemoryGraphSetup<IVisualNode> _graphSetup;
    protected override GraphView _GraphView => _graphSetup.GraphView;
    private MenuBuilder _menuBuilder;

    public TextAsset JsonFile;

    public void Reset()
    {
        CreateGUI();
    }

    protected override void CreateGUI()
    {
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            _graphSetup = new MemoryGraphSetup<IVisualNode>(graphRuntime, nodePositions);
        }
        else
        {
            _graphSetup = new MemoryGraphSetup<IVisualNode>();
        }

        base.CreateGUI();

        _menuBuilder = new MenuBuilder(_GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_graphSetup.GraphRuntime, _graphSetup.NodeViews),
            new SelectionEntry(),
            new MemoryNodeMenuEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions),
            new MemorySaveLoadMenu<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions)
        });
    }

    protected override void Update()
    {
        _graphSetup.Tick();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _graphSetup.Dispose();
    }

    [MenuItem("Graph/Memory Expression Tree")]
    public static void OpenWindow()
    {
        OpenWindow<MemoryExpressionTreeWindow>("Memory");
    }
}