using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabGraphWindow<TNode, TComponent> : BaseGraphWindow
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        protected PrefabGraphSetup<TNode, TComponent> _GraphSetup;

        protected override void CreateGUI()
        {
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
        }

        private void Update()
        {
            _GraphSetup?.Tick();
        }

        private void OnDestroy()
        {
            _GraphSetup?.Dispose();
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
        }

        private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
        {
            if (prefabStage == null)
            {
                RemoveGraphView();
                _GraphSetup?.Dispose();
                _GraphSetup = null;
            }
            else
            {
                _GraphSetup?.Dispose();
                var gameObjectsGraph = new GameObjectNodes<TNode, TComponent>(prefabStage.prefabContentsRoot);
                _GraphSetup = new PrefabGraphSetup<TNode, TComponent>(gameObjectsGraph);
                ReplaceGraphView(_GraphSetup.GraphView);
                CreateMenu();
            }
        }

        private void ClearEditorView(PrefabStage closingStage)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            // do NOT clear editor if current stage is not closing
            ResetGraphBackend(currentStage == closingStage ? null : currentStage);
        }

        protected virtual void CreateMenu() {}
    }
}
