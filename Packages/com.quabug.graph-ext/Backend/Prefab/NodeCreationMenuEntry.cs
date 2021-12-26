// #if UNITY_EDITOR
//
// using System;
// using System.Linq;
// using GraphExt.Editor;
// using UnityEditor;
// using UnityEditor.Experimental.SceneManagement;
// using UnityEngine;
// using UnityEngine.UIElements;
//
// namespace GraphExt.Prefab
// {
//     public class NodeCreationMenuEntry<TComponent, TNode> : IMenuEntry
//         where TComponent : NodeComponent
//         where TNode : INode
//     {
//         public void MakeEntry(GraphView graph, ContextualMenuPopulateEvent evt, GenericMenu menu)
//         {
//             if (!(graph.Module is PrefabGraphBackend backend && PrefabStageUtility.GetCurrentPrefabStage() != null)) return;
//
//             var menuPosition = graph.viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
//             var nodes = TypeCache.GetTypesDerivedFrom<TNode>();
//             foreach (var nodeType in nodes
//                 .Where(type => !type.IsAbstract && !type.IsGenericType)
//                 .OrderBy(type => type.Name))
//             {
//                 menu.AddItem(new GUIContent($"Node/{nodeType.Name}"), false, () => CreateNode(nodeType));
//             }
//
//             void CreateNode(Type nodeType)
//             {
//                 var stage = PrefabStageUtility.GetCurrentPrefabStage();
//                 var root = stage == null ? null : stage.prefabContentsRoot.transform;
//                 if (root == null)
//                 {
//                     Debug.LogWarning("must open a prefab to create a node.");
//                     return;
//                 }
//                 var nodeObject = new GameObject(nodeType.Name);
//                 nodeObject.transform.SetParent(root);
//                 var nodeComponent = nodeObject.AddComponent<TComponent>();
//                 nodeComponent.Node = (INode)Activator.CreateInstance(nodeType);
//                 nodeComponent.Position = menuPosition;
//                 backend.AddNode(nodeObject);
//                 stage.scene.SaveScene();
//             }
//         }
//     }
//
//     public class FlatNodeCreationMenuEntry : NodeCreationMenuEntry<FlatNodeComponent, INode> {}
//     public class TreeNodeCreationMenuEntry : NodeCreationMenuEntry<TreeNodeComponent, ITreeNode> {}
// }
//
// #endif