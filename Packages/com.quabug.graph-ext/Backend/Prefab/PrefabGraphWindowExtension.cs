#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace GraphExt.Editor
{
    public class PrefabGraphWindowExtension<TNode, TComponent> : BaseGraphWindowExtension
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        protected override void Recreate()
        {
            PrefabStage.prefabStageOpened -= ResetGraphBackend;
            PrefabStage.prefabStageClosing -= ClearEditorView;
            ResetGraphBackend(PrefabStageUtility.GetCurrentPrefabStage());
            PrefabStage.prefabStageOpened += ResetGraphBackend;
            PrefabStage.prefabStageClosing += ClearEditorView;
        }

        private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
        {
            if (prefabStage == null)
            {
                RemoveGraphView();
                Clear();
            }
            else
            {
                _Container = new Container();
                var graphBackend = new GameObjectNodes<TNode, TComponent>(prefabStage.prefabContentsRoot);
                _Container.RegisterInstance(graphBackend).AsSelf();
                _Container.RegisterInstance(prefabStage).AsSelf();
                _Container.RegisterInstance(prefabStage.prefabContentsRoot).AsSelf();
                _Container.RegisterSerializableGraphBackend(graphBackend);
                _Container.Register<Func<NodeId, INodeComponent>>((resolveContainer, contractType) =>
                {
                    var nodes = _Container.Resolve<IReadOnlyDictionary<NodeId, TComponent>>();
                    return id => nodes[id];
                }).AsSelf();
                base.Recreate();
            }
        }

        private void ClearEditorView(PrefabStage closingStage)
        {
            var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
            // do NOT clear editor if current stage is not closing
            ResetGraphBackend(currentStage == closingStage ? null : currentStage);
        }
    }
}

#endif