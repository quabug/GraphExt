// // #if UNITY_EDITOR
//
// using System.Collections.Generic;
// using System.Linq;
// using JetBrains.Annotations;
// using UnityEngine;
//
// namespace GraphExt.Editor
// {
//     public class PrefabGraphViewModule<TNode> : IGraphViewModule where TNode : INode<GraphRuntime<TNode>>
//     {
//         public GameObjectNodes<TNode> _gameObjectNodes;
//         public GraphRuntime<TNode> Runtime => _gameObjectNodes.Graph;
//
//         public IEnumerable<(PortId id, PortData data)> PortMap => _portDataCache.Select(pair => (pair.Key, pair.Value));
//         public IEnumerable<(NodeId id, NodeData data)> NodeMap => _nodeDataCache.Select(pair => (pair.Key, pair.Value));
//         public IEnumerable<EdgeId> Edges => Runtime.Edges;
//
//         private readonly Dictionary<NodeId, NodeData> _nodeDataCache = new Dictionary<NodeId, NodeData>();
//         private readonly Dictionary<PortId, PortData> _portDataCache = new Dictionary<PortId, PortData>();
//
//         public PrefabGraphViewModule(GameObject root)
//         {
//             _gameObjectNodes = new GameObjectNodes<TNode>(root, typeof(NodeComponent<TNode>));
//         }
//
//         public void AddNode(in NodeId nodeId, TNode node, Vector2 position)
//         {
//             _nodeDataCache[nodeId] = ToNodeData(nodeId, node);
//             var ports = NodePortUtility.FindPorts(node.GetType()).ToArray();
//             foreach (var port in ports) _portDataCache[new PortId(nodeId, port.Name)] = port;
//             Runtime.AddNode(nodeId, node, ports.Select(port => port.Name));
//         }
//
//         public void DeleteNode(in NodeId nodeId)
//         {
//             _nodeDataCache.Remove(nodeId);
//             foreach (var port in Runtime.FindNodePorts(nodeId)) _portDataCache.Remove(port);
//             Runtime.DeleteNode(nodeId);
//         }
//
//         public bool IsCompatible(in PortId input, in PortId output)
//         {
//             var inputPort = _portDataCache[input];
//             var outputPort = _portDataCache[output];
//             return inputPort.Direction != outputPort.Direction &&
//                    inputPort.Orientation == outputPort.Orientation &&
//                    Runtime.IsCompatible(input, output)
//             ;
//         }
//
//         public void Connect(in PortId input, in PortId output)
//         {
//             Runtime.Connect(input, output);
//         }
//
//         public void Disconnect(in PortId input, in PortId output)
//         {
//             Runtime.Disconnect(input, output);
//         }
//
//         private NodeData ToNodeData(NodeId id, [NotNull] TNode node)
//         {
//             return new NodeData(CreatePositionProperty().Yield()
//                     .Append(NodeTitleAttribute.CreateTitleProperty(node))
//                     .Concat(NodePropertyUtility.CreateProperties(node, id))
//                     .ToArray()
//             );
//
//             INodeProperty CreatePositionProperty()
//             {
//                 return new NodePositionProperty(_nodePositions[id], position => _nodePositions[id] = position);
//             }
//         }
//     }
// }
//
// // #endif