using GraphExt.Editor;

public class PrefabExpressionTreeWindow : PrefabGraphWindow<IVisualNode, VisualTreeComponent>
{
    private MenuBuilder _menuBuilder;
    private PrefabStickyNoteSystem _stickyNoteSystem;

    protected override void OnGraphRecreated()
    {
        if (_GraphSetup != null && _PrefabStage != null)
        {
            var root = _PrefabStage.prefabContentsRoot;
            _stickyNoteSystem = new PrefabStickyNoteSystem(_GraphSetup.GraphView, root);
            CreateMenu();
        }
    }

    private void CreateMenu()
    {
        _menuBuilder = new MenuBuilder(_GraphSetup.GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse),
            new SelectionEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse, _GraphSetup.EdgeViews.Reverse),
            new StickyNoteDeletionMenuEntry(_stickyNoteSystem.RemoveNote),
            new StickyNoteCreationMenuEntry(_stickyNoteSystem.AddNote),
            new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions)
        });
    }
}
