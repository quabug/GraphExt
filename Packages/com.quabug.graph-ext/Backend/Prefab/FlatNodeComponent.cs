using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class FlatNodeComponent : NodeComponent
    {
        [Serializable]
        private struct Connection
        {
            public string NodeId1;
            public string PortName1;
            public string NodeId2;
            public string PortName2;
            public EdgeId ToEdge() => new EdgeId(new PortId(Guid.Parse(NodeId1), PortName1), new PortId(Guid.Parse(NodeId2), PortName2));
        }

        [SerializeField] private Connection[] _serializedConnections;
        private readonly Lazy<HashSet<EdgeId>> _connections;
        public override IEnumerable<EdgeId> Connections => _connections.Value;

        public FlatNodeComponent()
        {
            _connections = new Lazy<HashSet<EdgeId>>(() => new HashSet<EdgeId>(_serializedConnections.Select(conn => conn.ToEdge())));
        }
    }
}