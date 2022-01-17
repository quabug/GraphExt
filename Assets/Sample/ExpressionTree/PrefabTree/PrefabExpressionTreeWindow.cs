using GraphExt.Editor;
using UnityEditor;

    public class PrefabExpressionTreeWindow : PrefabGraphWindow<IVisualNode, VisualTreeComponent>
    {
        private MenuBuilder _menuBuilder;

        [MenuItem("Graph/Prefab Expression Tree")]
        public static void OpenWindow()
        {
            OpenWindow<PrefabExpressionTreeWindow>("Prefab");
        }

        protected override void CreateMenu()
        {
            _menuBuilder = new MenuBuilder(_GraphSetup.GraphView, new IMenuEntry[]
            {
                new PrintValueMenu(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse),
                new SelectionEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodeViews.Reverse, _GraphSetup.EdgeViews.Reverse),
                new NodeMenuEntry<IVisualNode>(_GraphSetup.GraphRuntime, _GraphSetup.NodePositions)
            });
        }
    }
