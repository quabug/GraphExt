// using JetBrains.Annotations;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
//
// namespace GraphExt.Prefab
// {
//     public class Graph : Graph<GameObjectNode>
//     {
//         private GameObject _root { get; }
//
//         public Graph([NotNull] GameObject root)
//             :base()
//         {
//             _root = root;
//             Selection.selectionChanged += OnSelectionChanged;
//         }
//
//         void OnSelectionChanged()
//         {
//             var selectedInstance = Selection.activeGameObject;
//             if (selectedInstance != null)
//             {
//             }
//         }
//
//         public override bool IsCompatible(PortId input, PortId output)
//         {
//             var inputData = _PortMap[input];
//             var outputData = _PortMap[output];
//             return inputData.Direction != outputData.Direction &&
//                    inputData.PortType == outputData.PortType;
//                    // GetMemoryNodeByPort(input).IsPortCompatible(this, output.Id, input.Id) &&
//                    // GetMemoryNodeByPort(output).IsPortCompatible(this, output.Id, input.Id)
//             ;
//         }
//
//         public override void OnConnected(PortId input, PortId output)
//         {
//             // GetMemoryNodeByPort(input).OnConnected(this, output.Id, input.Id);
//             // GetMemoryNodeByPort(output).OnConnected(this, output.Id, input.Id);
//         }
//
//         public override void OnDisconnected(PortId input, PortId output)
//         {
//             // GetMemoryNodeByPort(input).OnDisconnected(this, output.Id, input.Id);
//             // GetMemoryNodeByPort(output).OnDisconnected(this, output.Id, input.Id);
//         }
//
//         public GameObjectNode CreateNode()
//         {
//             var obj = new GameObject();
//             var node = obj.AddComponent<GameObjectNode>();
//             return node;
//         }
//     }
// }