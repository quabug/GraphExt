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
            switch (Selection.activeObject)
            {
                case GraphScriptableObject<TNode, TNodeScriptableObject> graph:
                    return graph;
                case NodeScriptableObject<TNode> node:
                {
                    var path = AssetDatabase.GetAssetPath(node);
                    return AssetDatabase.LoadAssetAtPath<GraphScriptableObject<TNode, TNodeScriptableObject>>(path);
                }
                default:
                    return null;
            }
        }

        protected virtual void CreateMenu() {}
    }
}