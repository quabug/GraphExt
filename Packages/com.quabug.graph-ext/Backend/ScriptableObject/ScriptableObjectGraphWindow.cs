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
            if (Selection.activeObject is GraphScriptableObject<TNode, TNodeScriptableObject> graph)
            {
                _GraphSetup = new ScriptableObjectGraphSetup<TNode, TNodeScriptableObject>(graph);
                ReplaceGraphView(_GraphSetup.GraphView);
                CreateMenu();
            }
            else
            {
                RemoveGraphView();
                _GraphSetup = null;
            }
        }

        protected virtual void CreateMenu() {}
    }
}