using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Prefab
{
    public class PrefabGraphBackend : BaseGraphBackend
    {
        public GameObject Root { get; } = null;

        private readonly BiDictionary<NodeId, GameObject> _nodeObjectMap = new BiDictionary<NodeId, GameObject>();
        public IReadOnlyDictionary<NodeId, GameObject> NodeObjectMap => _nodeObjectMap.Forward;
        public IReadOnlyDictionary<GameObject, NodeId> ObjectNodeMap => _nodeObjectMap.Reverse;

        public PrefabGraphBackend() {}
        public PrefabGraphBackend([NotNull] GameObject root) : base()
        {
            Root = root;
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
            var connection = new EdgeId(input, output);
            GetNodeComponent(input.NodeId).OnConnected(this, connection);
            GetNodeComponent(output.NodeId).OnConnected(this, connection);
        }

        protected override void OnDisconnected(in PortId input, in PortId output)
        {
            var connection = new EdgeId(input, output);
            GetNodeComponent(input.NodeId).OnDisconnected(this, connection);
            GetNodeComponent(output.NodeId).OnDisconnected(this, connection);
        }

        public INodeComponent GetNodeComponent(in NodeId nodeId) => NodeObjectMap[nodeId].GetComponent<INodeComponent>();

        public void AddNode(GameObject nodeObject)
        {
            var node = nodeObject.GetComponent<INodeComponent>();
            _nodeObjectMap.Add(node.Id, nodeObject);
            _NodeMap.Add(node.Id, new NodeData(node.Properties.ToArray()));
            foreach (var (portId, portData) in node.Ports) _PortMap.Add(portId, portData);
        }
    }
}