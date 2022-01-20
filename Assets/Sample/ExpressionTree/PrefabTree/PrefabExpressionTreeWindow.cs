using GraphExt;
using GraphExt.Editor;
using UnityEditor.Experimental.GraphView;

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
            new StickyNoteDeletionMenuEntry(DeleteNote),
            new StickyNoteCreationMenuEntry(AddNote),
            new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions)
        });
    }

    private void DeleteNote(StickyNote note)
    {
        _stickyNoteSystem.RemoveNote(note);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_PrefabStage.scene);
    }

    private void AddNote(StickyNoteId id, StickyNoteData note)
    {
        _stickyNoteSystem.AddNote(id, note);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_PrefabStage.scene);
    }
}
