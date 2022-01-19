using System.Collections.Generic;
using System.Linq;
using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class ScriptableObjectExpressionTreeWindow : ScriptableObjectGraphWindow<IVisualNode, VisualNodeScriptableObject>
{
    private MenuBuilder _menuBuilder;

    private readonly BiDictionary<NodeId, StickyNote> _stickyNoteViews = new BiDictionary<NodeId, StickyNote>();
    private StickyNodePresenter _stickyNodePresenter;
    private readonly Dictionary<NodeId, StickyNoteScriptableObject> _notes = new Dictionary<NodeId, StickyNoteScriptableObject>();

    protected override void OnGraphRecreated()
    {
        _stickyNoteViews.Clear();
        _notes.Clear();
        _stickyNodePresenter = null;
        if (_GraphSetup != null)
        {
            var path = AssetDatabase.GetAssetPath(_GraphSetup.Graph);
            foreach (var note in AssetDatabase.LoadAllAssetsAtPath(path).OfType<StickyNoteScriptableObject>())
                _notes[note.NodeId] = note;

            _stickyNodePresenter = new StickyNodePresenter(
                _GraphSetup.GraphView,
                () => _notes.Keys,
                _stickyNoteViews,
                nodeId => _notes[nodeId].Data,
                (id, data) => _notes[id].Data = data
            );

            CreateMenu();
        }
    }

    protected override void Update()
    {
        base.Update();
        _stickyNodePresenter?.Tick();
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
        var nodeId = _stickyNoteViews.GetKey(note);
        var noteInstance = _notes[nodeId];
        _notes.Remove(nodeId);
        DestroyImmediate(noteInstance, allowDestroyingAssets: true);
        AssetDatabase.SaveAssets();
    }

    private void AddNote(NodeId nodeId, StickyNoteData note)
    {
        var noteObject = CreateInstance<StickyNoteScriptableObject>();
        noteObject.Init(_GraphSetup.Graph, nodeId, note);
        _notes[nodeId] = noteObject;
    }
}