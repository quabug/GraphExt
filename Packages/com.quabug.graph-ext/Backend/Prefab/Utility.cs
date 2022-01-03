#if UNITY_EDITOR

using System;
using System.Linq;
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

        public static NodeData CreateDefaultNodeData<TNode, TComponent>([NotNull] this TComponent nodeComponent, string nodePropertyName, Vector2 position)
            where TNode : INode<GraphRuntime<TNode>>
            where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
        {
            var nodeSerializedProperty = new UnityEditor.SerializedObject(nodeComponent).FindProperty(nodePropertyName);
            return new NodeData(new NodePositionProperty(position.x, position.y).Yield()
                .Append<INodeProperty>(nodeComponent.CreateDynamicTitleProperty<TNode, TComponent>())
                .Concat(nodeComponent.Node.CreateProperties(nodeComponent.Id, nodeSerializedProperty))
                .ToArray()
            );
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
                    NodeTitle.TitleType.NodeTitleAttribute => NodeTitleAttribute.GetTitle(nodeComponent.Node),
                    NodeTitle.TitleType.CustomTitle => titleComponent.CustomTitle,
                    _ => throw new ArgumentOutOfRangeException()
                };
            });
        }
    }
}

#endif