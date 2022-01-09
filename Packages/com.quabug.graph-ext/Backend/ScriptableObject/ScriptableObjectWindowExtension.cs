// #if UNITY_EDITOR
//
// using UnityEditor;
//
// namespace GraphExt.Editor
// {
//     public class ScriptableObjectWindowExtension<TNode, TNodeScriptableObject> : IWindowExtension
//         where TNode : INode<GraphRuntime<TNode>>
//         where TNodeScriptableObject : NodeScriptableObject<TNode>
//     {
//         private GraphView _view;
//         private ScriptableObjectGraphViewModule<TNode, TNodeScriptableObject> _viewModule;
//
//         public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
//         {
//             _view = view;
//             // _view.Module = new EmptyGraphViewModule();
//             Selection.selectionChanged += OnSelectionChanged;
//         }
//
//         public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
//         {
//             Selection.selectionChanged -= OnSelectionChanged;
//             if (_view != null)
//             {
//                 // _view.Module = new EmptyGraphViewModule();
//                 _view = null;
//             }
//         }
//
//         private void OnSelectionChanged()
//         {
//             if (Selection.activeObject is GraphScriptableObject<TNode, TNodeScriptableObject> graph)
//             {
//                 _viewModule = new ScriptableObjectGraphViewModule<TNode, TNodeScriptableObject>(graph);
//                 // _view.Module = _viewModule;
//             }
//             else
//             {
//                 // _view.Module = new EmptyGraphViewModule();
//             }
//         }
//     }
// }
//
// #endif
