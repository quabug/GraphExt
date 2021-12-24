using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class PrefabGraphBackend : BaseGraphBackend
    {
        private GameObject _root { get; } = null;

        private readonly BiDictionary<NodeId, INode> _prefabNodeMap = new BiDictionary<NodeId, INode>();
        public IReadOnlyDictionary<NodeId, INode> PrefabNodeMap => _prefabNodeMap.Forward;
        public IReadOnlyDictionary<INode, NodeId> PrefabNodeIdMap => _prefabNodeMap.Reverse;

        private readonly Dictionary<NodeId, GameObject> _nodeObjectMap = new Dictionary<NodeId, GameObject>();
        public IReadOnlyDictionary<NodeId, GameObject> NodeObjectMap;

        public PrefabGraphBackend() {}
        public PrefabGraphBackend([NotNull] GameObject root) :base()
        {
            _root = root;
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            var selectedInstance = Selection.activeGameObject;
            if (selectedInstance != null)
            {
            }
        }

        public override bool IsCompatible(in PortId input, in PortId output)
        {
            var inputData = _PortMap[input];
            var outputData = _PortMap[output];
            return inputData.Direction != outputData.Direction &&
                   inputData.PortType == outputData.PortType;
        }

        protected override void OnConnected(in PortId input, in PortId output)
        {
            // GetMemoryNodeByPort(input).OnConnected(this, output.Id, input.Id);
            // GetMemoryNodeByPort(output).OnConnected(this, output.Id, input.Id);
        }

        protected override void OnDisconnected(in PortId input, in PortId output)
        {
            // GetMemoryNodeByPort(input).OnDisconnected(this, output.Id, input.Id);
            // GetMemoryNodeByPort(output).OnDisconnected(this, output.Id, input.Id);
        }

        [NotNull] public ISet<PortId> FindConnectedPorts(INode node, string port)
        {
            var nodeId = _prefabNodeMap.GetKey(node);
            return FindConnectedPorts(new PortId(nodeId, port));
        }

        public void AddNode(GameObject nodeObject)
        {
            var node = nodeObject.GetComponent<INodeComponent>();
            _NodeMap.Add(node.Id, new NodeData(node.Properties.ToArray()));
            foreach (var (portId, portData) in node.Ports) _PortMap.Add(portId, portData);
        }
    }
}