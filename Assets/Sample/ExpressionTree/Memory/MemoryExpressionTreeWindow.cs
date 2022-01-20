using System.Collections.Generic;
using GraphExt;
using GraphExt.Editor;
using UnityEngine;

public class MemoryExpressionTreeWindow : BaseGraphWindow
{
    private MemoryGraphSetup<IVisualNode> _graphSetup;
    private MenuBuilder _menuBuilder;
    public TextAsset JsonFile;
    private MemoryStickyNoteSystem _stickyNoteSystem;

    public void Recreate()
    {
        RecreateGUI();
    }

    protected override void RecreateGUI()
    {
        if (JsonFile != null)
        {
            var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
            _graphSetup = new MemoryGraphSetup<IVisualNode>(graphRuntime, nodePositions);
            _stickyNoteSystem = new MemoryStickyNoteSystem(_graphSetup.GraphView, notes);
        }
        else
        {
            _graphSetup = new MemoryGraphSetup<IVisualNode>();
            _stickyNoteSystem = new MemoryStickyNoteSystem(_graphSetup.GraphView, new Dictionary<StickyNoteId, StickyNoteData>());
        }

        _menuBuilder = new MenuBuilder(_graphSetup.GraphView, new IMenuEntry[]
        {
            new PrintValueMenu(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse),
            new StickyNoteDeletionMenuEntry(note => _stickyNoteSystem.RemoveNote(note)),
            new SelectionEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse, _graphSetup.EdgeViews.Reverse),
            new StickyNoteCreationMenuEntry((id, noteData) => _stickyNoteSystem.AddNote(id, noteData)),
            new NodeMenuEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions),
            new MemorySaveLoadMenu<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions, _stickyNoteSystem.StickyNotes)
        });

        ReplaceGraphView(_graphSetup.GraphView);
    }

    private void Update()
    {
        _graphSetup?.Tick();
    }

    private void OnDestroy()
    {
        _graphSetup?.Dispose();
    }
}