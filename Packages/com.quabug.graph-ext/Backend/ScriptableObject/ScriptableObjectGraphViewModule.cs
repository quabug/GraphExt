#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    public sealed class ScriptableObjectGraphViewModule<TNode, TNodeScriptableObject> : GraphViewModule<TNode>
        where TNode : INode<GraphRuntime<TNode>>
        where TNodeScriptableObject : NodeScriptableObject<TNode>
    {
        [NotNull] public GraphScriptableObject<TNode, TNodeScriptableObject> Graph { get; }
        [NotNull] public override GraphRuntime<TNode> Runtime => Graph.Runtime;

        private readonly Dictionary<NodeId, IReadOnlyDictionary<string, PortData>> _portsCache =
            new Dictionary<NodeId, IReadOnlyDictionary<string, PortData>>();

        public ScriptableObjectGraphViewModule(GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            Graph = graph;
            graph.Initialize();
            foreach (var nodeId in Runtime.NodeMap.Keys)
            {
                _PortData[nodeId] = FindNodePorts(nodeId);
                _NodeData[nodeId] = ToNodeData(nodeId);
            }
        }

        public void AddScriptableObjectNode(in NodeId nodeId, TNode node, Vector2 position)
        {
            AddNode(nodeId, node, position.x, position.y);
            Save();
        }

        public override void DeleteNode(in NodeId nodeId)
        {
            base.DeleteNode(in nodeId);
            Save();
        }

        public override void SetNodePosition(in NodeId nodeId, float x, float y)
        {
            Graph.SetPosition(nodeId, new Vector2(x, y));
            Save();
        }

        private void Save()
        {
            EditorUtility.SetDirty(Graph);
            AssetDatabase.Refresh();
        }

        protected override IReadOnlyDictionary<string, PortData> FindNodePorts(in NodeId nodeId)
        {
            if (!_portsCache.TryGetValue(nodeId, out var ports))
            {
                ports = Runtime[nodeId].FindPorts().ToDictionary(port => port.Name, port => port);
                _portsCache[nodeId] = ports;
            }
            return ports;
        }

        protected override NodeData ToNodeData(in NodeId id)
        {
            return new NodeData();
            // var nodeObject = Graph[id];
            // var node = nodeObject.Node;
            // var nodeSerializedProperty = new SerializedObject(nodeObject).FindProperty(nodeObject.NodeSerializedPropertyName);
            // var position = nodeObject.Position;
            // return new NodeData(new NodePositionProperty(position.x, position.y).Yield()
            //     .Append(NodeTitleAttribute.CreateTitleProperty(node))
            //     .Concat(node.CreateProperties(id, nodeSerializedProperty))
            //     .ToArray()
            // );
        }
    }
}

#endif
