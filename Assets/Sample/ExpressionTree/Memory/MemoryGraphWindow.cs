using GraphExt.Editor;
using UnityEditor;
using GraphView = UnityEditor.Experimental.GraphView.GraphView;

public class MemoryGraphWindow : BaseGraphWindow
{
    private readonly MemoryGraphSetup<IVisualNode> _graphSetup = new MemoryGraphSetup<IVisualNode>();
    protected override GraphView _GraphView => _graphSetup.GraphView;

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
        OpenWindow<MemoryGraphWindow>("Memory");
    }
}