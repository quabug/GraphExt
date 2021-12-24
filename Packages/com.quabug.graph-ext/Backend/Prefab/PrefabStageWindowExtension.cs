using GraphExt.Editor;
using JetBrains.Annotations;
using UnityEditor.Experimental.SceneManagement;

namespace GraphExt.Prefab
{
    public class PrefabStageWindowExtension : IWindowExtension
    {
        private GraphView _view;

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            _view = view;
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            ResetGraphBackend(stage);
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
        }

        public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
        {
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
            _view = null;
        }

        private void ClearEditorView(PrefabStage closingStage)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            // do NOT clear editor if current stage is not closing
            ResetGraphBackend(currentStage == closingStage ? null : currentStage);
        }

        private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
        {
            _view.Module = prefabStage == null ? new PrefabGraphBackend() : new PrefabGraphBackend(prefabStage.prefabContentsRoot);
        }
    }
}