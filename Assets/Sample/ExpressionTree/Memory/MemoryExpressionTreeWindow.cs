using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MemoryExpressionTreeWindow : BaseGraphWindow
{
    private readonly Dictionary<NodeId, StickyNoteData> _stickyNotes = new Dictionary<NodeId, StickyNoteData>();
    private readonly BiDictionary<NodeId, StickyNote> _stickyNoteViews = new BiDictionary<NodeId, StickyNote>();
    private StickyNodePresenter _stickyNodePresenter;

    private MemoryGraphSetup<IVisualNode> _graphSetup;
    private MenuBuilder _menuBuilder;

    public TextAsset JsonFile;

    public void Recreate()
    {
        RecreateGUI();
    }

    protected override void RecreateGUI()
    {
        _graphSetup?.Dispose();
        _stickyNotes.Clear();
        _stickyNoteViews.Clear();
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            _graphSetup = new MemoryGraphSetup<IVisualNode>(graphRuntime, nodePositions);
            if (notes != null)
            {
                foreach (var note in notes) _stickyNotes.Add(note.Key, note.Value);
            }
        }
        else
        {
            _graphSetup = new MemoryGraphSetup<IVisualNode>();
        }

        _menuBuilder = new MenuBuilder(_graphSetup.GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse),
            new StickyNoteDeletionMenuEntry(note => _stickyNotes.Remove(_stickyNoteViews.GetKey(note))),
            new SelectionEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse, _graphSetup.EdgeViews.Reverse),
            new StickyNoteCreationMenuEntry((nodeId, noteData) => _stickyNotes.Add(nodeId, noteData)),
            new NodeMenuEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions),
            new MemorySaveLoadMenu<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions, _stickyNotes)
        });

        ReplaceGraphView(_graphSetup.GraphView);

        _stickyNodePresenter = new StickyNodePresenter(
            _graphSetup.GraphView,
            () => _stickyNotes.Keys,
            _stickyNoteViews,
            id => _stickyNotes[id],
            (id, data) => _stickyNotes[id] = data
        );
    }

    private void Update()
    {
        _graphSetup?.Tick();
        _stickyNodePresenter?.Tick();
    }

    private void OnDestroy()
    {
        _graphSetup?.Dispose();
    }
}