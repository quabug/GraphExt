// using System.Collections.Generic;
// using System.Linq;
// using JetBrains.Annotations;
// using UnityEngine;
//
// namespace GraphExt.Editor
// {
//     public class PrefabGraphViewModule : IGraphViewModule
//     {
//         public GraphRuntime<IGameObjectNode> Runtime { get; } = new GraphRuntime<IGameObjectNode>();
//
//         public IEnumerable<(PortId id, PortData data)> PortMap => _portDataCache.Select(pair => (pair.Key, pair.Value));
//         public IEnumerable<(NodeId id, NodeData data)> NodeMap => _nodeDataCache.Select(pair => (pair.Key, pair.Value));
//         public IEnumerable<EdgeId> Edges => Runtime.Edges;
//
//         private readonly BiDictionary<NodeId, GameObject> _nodeObjectMap = new BiDictionary<NodeId, GameObject>();
//         private readonly Dictionary<NodeId, NodeData> _nodeDataCache = new Dictionary<NodeId, NodeData>();
//         private readonly Dictionary<PortId, PortData> _portDataCache = new Dictionary<PortId, PortData>();
//
//         public PrefabGraphViewModule() {}
//
//         public PrefabGraphViewModule(GameObject root)
//         {
//             foreach (var pair in runtime.NodeMap) AddNode(pair.Key, pair.Value, positions[pair.Key]);
//             foreach (var edge in runtime.Edges) Runtime.Connect(edge.Input, edge.Output);
//         }
//
//         public void AddNode(GameObject nodeObject)
//         {
//             var node = nodeObject.GetComponent<INodeComponent>();
//             _nodeObjectMap.Add(node.Id, nodeObject);
//             // _NodeMap.Add(node.Id, new NodeData(node.Properties.Append(CreateNodeSelector(node.Id)).ToArray()));
//             foreach (var (portId, portData) in node.Ports) _PortMap.Add(portId, portData);
//             foreach (var connection in node.Connections) _Edges.Add(connection);
//         }
//
//         public void AddNode(in NodeId nodeId, IMemoryNode node, Vector2 position)
//         {
//             _nodePositions[nodeId] = position;
//             _nodeDataCache[nodeId] = ToNodeData(nodeId, node);
//             var ports = NodePortUtility.FindPorts(node.GetType()).ToArray();
//             foreach (var port in ports) _portDataCache[new PortId(nodeId, port.Name)] = port;
//             Runtime.AddNode(nodeId, node, ports.Select(port => port.Name));
//         }
//
//         public void DeleteNode(in NodeId nodeId)
//         {
//             _nodePositions.Remove(nodeId);
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
//         private NodeData ToNodeData(NodeId id, [NotNull] IMemoryNode node)
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