using UnityEditor;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphWindow<TNode, TNodeScriptableObject> : BaseGraphWindow
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        protected ScriptableObjectGraphSetup<TNode, TNodeScriptableObject> _GraphSetup;

        protected override void CreateGUI()
        {
            OnSelectionChanged();
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void Update()
        {
            _GraphSetup?.Tick();
        }

        private void OnDestroy()
        {
            _GraphSetup?.Dispose();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            var graph = FindCurrentGraph();
            if (graph == null)
            {
                RemoveGraphView();
                _GraphSetup?.Dispose();
                _GraphSetup = null;
            }
            else if (_GraphSetup == null || graph != _GraphSetup.Graph)
            {
                _GraphSetup?.Dispose();
                _GraphSetup = new ScriptableObjectGraphSetup<TNode, TNodeScriptableObject>(graph);
                ReplaceGraphView(_GraphSetup.GraphView);
                CreateMenu();
            }
        }

        GraphScriptableObject<TNode, TNodeScriptableObject> FindCurrentGraph()
        {
            if (Selection.activeObject is GraphScriptableObject<TNode, TNodeScriptableObject> graph) return graph;
            if (Selection.activeObject is NodeScriptableObject<TNode> node)
            {
                var path = AssetDatabase.GetAssetPath(node);
                return AssetDatabase.LoadAssetAtPath<GraphScriptableObject<TNode, TNodeScriptableObject>>(path);
            }
            return null;
        }

        protected virtual void CreateMenu() {}
    }
}