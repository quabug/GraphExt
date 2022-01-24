// using System.Collections.Generic;
// using GraphExt;
// using GraphExt.Editor;
// using UnityEngine;
//
// public class MemoryExpressionTreeWindow : BaseGraphWindow
// {
//     private MemoryGraphSetup<IVisualNode> _graphSetup;
//     private MenuBuilder _menuBuilder;
//     public TextAsset JsonFile;
//     private MemoryStickyNoteSystem _stickyNoteSystem;
//
//     public void Recreate()
//     {
//         RecreateGUI();
//     }
//
//     protected override void RecreateGUI()
//     {
//         if (JsonFile != null)
//         {
//             var (graphRuntime, nodePositions, notes) = JsonEditorUtility.Deserialize<IVisualNode>(JsonFile.text);
//             _graphSetup = new MemoryGraphSetup<IVisualNode>(_Config, graphRuntime, nodePositions);
//             _stickyNoteSystem = new MemoryStickyNoteSystem(
//                 _graphSetup.GraphView,
//                 _Config.GetViewFactory<IStickyNoteViewFactory>(),
//                 notes
//             );
//         }
//         else
//         {
//             _graphSetup = new MemoryGraphSetup<IVisualNode>(_Config);
//             _stickyNoteSystem = new MemoryStickyNoteSystem(
//                 _graphSetup.GraphView,
//                 _Config.GetViewFactory<IStickyNoteViewFactory>(),
//                 new Dictionary<StickyNoteId, StickyNoteData>()
//             );
//         }
//
//         _menuBuilder = new MenuBuilder(_graphSetup.GraphView, new IMenuEntry[]
//         {
//             new PrintValueMenu(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse),
//             new StickyNoteDeletionMenuEntry(_stickyNoteSystem.RemoveNote),
//             new SelectionEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodeViews.Reverse, _graphSetup.EdgeViews.Reverse),
//             new StickyNoteCreationMenuEntry(_stickyNoteSystem.AddNote),
//             new NodeMenuEntry<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions),
//             new MemorySaveLoadMenu<IVisualNode>(_graphSetup.GraphRuntime, _graphSetup.NodePositions, _stickyNoteSystem.StickyNotes)
//         });
//
//         ReplaceGraphView(_graphSetup.GraphView);
//     }
//
//     private void Update()
//     {
//         _graphSetup?.Tick();
//     }
//
//     private void OnDestroy()
//     {
//         _graphSetup?.Dispose();
//     }
// }