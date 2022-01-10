using UnityEditor;

namespace GraphExt.Editor
{
    public class ScriptableObjectGraphWindow<TNode, TNodeScriptableObject> : BaseGraphWindow
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        private ScriptableObjectGraphSetup<TNode, TNodeScriptableObject> _graphSetup;

        protected override void CreateGUI()
        {
            OnSelectionChanged();
            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void Update()
        {
            _graphSetup?.Tick();
        }

        private void OnDestroy()
        {
            _graphSetup?.Dispose();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeObject is GraphScriptableObject<TNode, TNodeScriptableObject> graph)
            {
                _graphSetup = new ScriptableObjectGraphSetup<TNode, TNodeScriptableObject>(graph);
                ReplaceGraphView(_graphSetup.GraphView);
            }
            else
            {
                RemoveGraphView();
                _graphSetup = null;
            }
        }
    }
}