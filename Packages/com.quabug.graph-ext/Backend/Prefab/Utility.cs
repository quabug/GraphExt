#if UNITY_EDITOR

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GraphExt.Editor
{
    internal static class Utility
    {
        public static void SavePrefabStage()
        {
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null) SaveScene(stage.scene);
        }

        public static void SaveScene(this Scene scene)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }

        public static DynamicTitleProperty CreateDynamicTitleProperty<TNode, TComponent>([NotNull] this TComponent nodeComponent)
            where TNode : INode<GraphRuntime<TNode>>
            where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
        {
            return new DynamicTitleProperty(() =>
            {
                var nodeObject = nodeComponent.gameObject;
                if (nodeObject == null) return "*** deleted ***";
                var titleComponent = nodeObject.GetComponent<NodeTitle>();
                if (titleComponent == null) return nodeObject.name;
                return titleComponent.Type switch
                {
                    NodeTitle.TitleType.Hidden => null,
                    NodeTitle.TitleType.GameObjectName => nodeObject.name,
                    NodeTitle.TitleType.CustomTitle => titleComponent.CustomTitle,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }
    }
}

#endif