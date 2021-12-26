// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace GraphExt.Prefab
// {
//     [AddComponentMenu("")]
//     public class FlatNodeComponent : NodeComponent
//     {
//         [SerializeField, HideInInspector] private List<Connection> _serializedConnections = new List<Connection>();
//         private readonly Lazy<HashSet<EdgeId>> _connections;
//         public override IEnumerable<EdgeId> Connections => _connections.Value;
//
//         public FlatNodeComponent()
//         {
//             _connections = new Lazy<HashSet<EdgeId>>(() => new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge())));
//         }
//
//         protected override void OnConnected(PrefabGraphBackend graph, in PortId input, in PortId output)
//         {
//             var edge = new EdgeId(input, output);
//             if (!_connections.Value.Contains(edge))
//             {
//                 _connections.Value.Add(edge);
//                 _serializedConnections.Add(new Connection(input, output));
//                 gameObject.scene.SaveScene();
//             }
//         }
//
//         protected override void OnDisconnected(PrefabGraphBackend graph, in PortId input, in PortId output)
//         {
//             var edge = new EdgeId(input, output);
//             if (_connections.Value.Contains(edge))
//             {
//                 _connections.Value.Remove(edge);
//                 _serializedConnections.Remove(new Connection(input, output));
//                 gameObject.scene.SaveScene();
//             }
//         }
//
//         [Serializable]
//         private struct Connection : IEquatable<Connection>
//         {
//             public string InputNode;
//             public string InputPort;
//             public string OutputNode;
//             public string OutputPort;
//
//             public Connection(in PortId input, in PortId output)
//             {
//                 InputNode = input.NodeId.ToString();
//                 InputPort = input.Name;
//                 OutputNode = output.NodeId.ToString();
//                 OutputPort = output.Name;
//             }
//
//             public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(InputNode), InputPort), new PortId(Guid.Parse(OutputNode), OutputPort));
//
//             public bool Equals(Connection other)
//             {
//                 return InputNode == other.InputNode && InputPort == other.InputPort && OutputNode == other.OutputNode && OutputPort == other.OutputPort;
//             }
//
//             public override bool Equals(object obj)
//             {
//                 return obj is Connection other && Equals(other);
//             }
//
//             public override int GetHashCode()
//             {
//                 unchecked
//                 {
//                     var hashCode = (InputNode != null ? InputNode.GetHashCode() : 0);
//                     hashCode = (hashCode * 397) ^ (InputPort != null ? InputPort.GetHashCode() : 0);
//                     hashCode = (hashCode * 397) ^ (OutputNode != null ? OutputNode.GetHashCode() : 0);
//                     hashCode = (hashCode * 397) ^ (OutputPort != null ? OutputPort.GetHashCode() : 0);
//                     return hashCode;
//                 }
//             }
//         }
//
//     }
// }