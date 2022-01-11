// using GraphExt.Editor;
// using UnityEditor;
//
// public class ScriptableObjectExpressionTreeWindow : ScriptableObjectGraphWindow<IVisualNode, VisualNodeScriptableObject>
// {
//     private MenuBuilder _menuBuilder;
//
//     [MenuItem("Graph/ScriptableObject Expression Tree")]
//     public static void OpenWindow()
//     {
//         OpenWindow<ScriptableObjectExpressionTreeWindow>("Scriptable Object");
//     }
//
//     protected override void CreateMenu()
//     {
//         _menuBuilder = new MenuBuilder(_GraphSetup.GraphView, new IMenuEntry[]
//         {
//             new PrintValueMenu(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews),
//             new SelectionEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews, _GraphSetup.EdgeViews),
//             new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions.)
//         });
//     }
// }