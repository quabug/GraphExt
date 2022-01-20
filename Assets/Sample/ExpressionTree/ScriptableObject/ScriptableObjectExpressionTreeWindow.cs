using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class ScriptableObjectExpressionTreeWindow : ScriptableObjectGraphWindow<IVisualNode, VisualNodeScriptableObject>
{
    private MenuBuilder _menuBuilder;
    private ScriptableObjectStickyNoteSystem _stickyNoteSystem;

    protected override void OnGraphRecreated()
    {
        if (_GraphSetup != null)
        {
            _stickyNoteSystem = new ScriptableObjectStickyNoteSystem(_GraphSetup.GraphView, _GraphSetup.Graph);
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
        AssetDatabase.SaveAssets();
    }

    private void AddNote(StickyNoteId id, StickyNoteData note)
    {
        _stickyNoteSystem.AddNote(id, note);
        AssetDatabase.SaveAssets();
    }
}