// using JetBrains.Annotations;
// using UnityEditor.Experimental.SceneManagement;
// using UnityEngine;
//
// namespace GraphExt.Editor
// {
//     public class PrefabGraphWindow<TNode, TComponent> : BaseGraphWindow
//         where TNode : INode<GraphRuntime<TNode>>
//         where TComponent : MonoBehaviour, INodeComponent<TNode, TComponent>
//     {
//         protected PrefabGraphSetup<TNode, TComponent> _GraphSetup;
//         protected PrefabStage _PrefabStage;
//
//         protected override void RecreateGUI()
//         {
//             ResetGraphBackend(PrefabStageUtility.GetCurrentPrefabStage());
//             PrefabStage.prefabStageOpened += ResetGraphBackend;
//             PrefabStage.prefabStageClosing += ClearEditorView;
//         }
//
//         protected virtual void Update()
//         {
//             _GraphSetup?.Tick();
//         }
//
//         protected virtual void OnDestroy()
//         {
//             _GraphSetup?.Dispose();
//             PrefabStage.prefabStageOpened -= ResetGraphBackend;
//             PrefabStage.prefabStageClosing -= ClearEditorView;
//         }
//
//         private void ResetGraphBackend([CanBeNull] PrefabStage prefabStage)
//         {
//             _PrefabStage = prefabStage;
//             if (prefabStage == null)
//             {
//                 RemoveGraphView();
//                 _GraphSetup?.Dispose();
//                 _GraphSetup = null;
//             }
//             else
//             {
//                 _GraphSetup?.Dispose();
//                 var gameObjectsGraph = new GameObjectNodes<TNode, TComponent>(prefabStage.prefabContentsRoot);
//                 _GraphSetup = new PrefabGraphSetup<TNode, TComponent>(_Config, gameObjectsGraph);
//                 ReplaceGraphView(_GraphSetup.GraphView);
//             }
//             OnGraphRecreated();
//         }
//
//         private void ClearEditorView(PrefabStage closingStage)
//         {
//             var currentStage = PrefabStageUtility.GetCurrentPrefabStage();
//             // do NOT clear editor if current stage is not closing
//             ResetGraphBackend(currentStage == closingStage ? null : currentStage);
//         }
//
//         protected virtual void OnGraphRecreated() {}
//     }
// }
