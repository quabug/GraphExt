using System.Collections.Generic;
using ExtraElements.StickyNode;
using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PrefabExpressionTreeWindow : PrefabGraphWindow<IVisualNode, VisualTreeComponent>
{
    private MenuBuilder _menuBuilder;

    private readonly BiDictionary<NodeId, StickyNote> _stickyNoteViews = new BiDictionary<NodeId, StickyNote>();
    private StickyNodePresenter _stickyNodePresenter;
    private readonly Dictionary<NodeId, StickyNoteComponent> _notes = new Dictionary<NodeId, StickyNoteComponent>();

    protected override void OnGraphRecreated()
    {
        _stickyNoteViews.Clear();
        _notes.Clear();
        _stickyNodePresenter = null;
        if (_GraphSetup != null && _PrefabStage != null)
        {
            var root = _PrefabStage.prefabContentsRoot;
            foreach (var note in root.GetComponentsInChildren<StickyNoteComponent>())
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
        DestroyImmediate(noteInstance.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_PrefabStage.scene);
    }

    private void AddNote(NodeId nodeId, StickyNoteData note)
    {
        var noteObject = new GameObject("Note");
        var noteInstance = noteObject.AddComponent<StickyNoteComponent>();
        noteInstance.Init(_PrefabStage.prefabContentsRoot, nodeId, note);
        _notes[nodeId] = noteInstance;
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(_PrefabStage.scene);
    }
}
