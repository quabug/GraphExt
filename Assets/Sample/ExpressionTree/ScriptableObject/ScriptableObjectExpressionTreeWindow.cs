using GraphExt.Editor;

public class ScriptableObjectExpressionTreeWindow : ScriptableObjectGraphWindow<IVisualNode, VisualNodeScriptableObject>
{
    private MenuBuilder _menuBuilder;
    private ScriptableObjectStickyNoteSystem _stickyNoteSystem;

    protected override void OnGraphRecreated()
    {
        if (_GraphSetup != null)
        {
            _stickyNoteSystem = new ScriptableObjectStickyNoteSystem(
                _GraphSetup.GraphView,
                _Config.GetViewFactory<IStickyNoteViewFactory>(),
                _GraphSetup.Graph
            );
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