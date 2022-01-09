#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public class NodeCreationMenuEntry<TNode, TComponent> : IMenuEntry
        where TNode : INode<GraphRuntime<TNode>>
        where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
    {
        public void MakeEntry(UnityEditor.Experimental.GraphView.GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
        {
            // if (!(graph.Module is GameObjectHierarchyGraphViewModule<TNode, TComponent> viewModule && PrefabStageUtility.GetCurrentPrefabStage() != null)) return;

            var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            var nodes = TypeCache.GetTypesDerivedFrom<TNode>();
            foreach (var nodeType in nodes
                .Where(type => !type.IsAbstract && !type.IsGenericType)
                .OrderBy(type => type.Name))
            {
                menu.AddItem(new GUIContent($"{typeof(TNode).Name}/{nodeType.Name}"), false, () => CreateNode(nodeType));
            }

            void CreateNode(Type nodeType)
            {
                var stage = PrefabStageUtility.GetCurrentPrefabStage();
                var root = stage == null ? null : stage.prefabContentsRoot.transform;
                if (root == null)
                {
                    Debug.LogWarning("must open a prefab to create a node.");
                    return;
                }
                // viewModule.AddGameObjectNode(Guid.NewGuid(), (TNode)Activator.CreateInstance(nodeType), menuPosition);
                stage.scene.SaveScene();
            }
        }
    }
}

#endif