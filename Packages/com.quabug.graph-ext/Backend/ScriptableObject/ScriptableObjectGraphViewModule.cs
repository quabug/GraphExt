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

        public delegate void OnNodeSelectedChangedFunc(in NodeId nodeId, bool isSelected);
        public OnNodeSelectedChangedFunc OnNodeSelectedChanged;

        public ScriptableObjectGraphViewModule(GraphScriptableObject<TNode, TNodeScriptableObject> graph)
        {
            Graph = graph;
            graph.Initialize();
            foreach (var nodePair in Runtime.NodeMap)
            {
                var nodeId = nodePair.Key;
                var node = nodePair.Value;
                foreach (var port in FindNodePorts(node)) _PortData[new PortId(nodeId, port.Name)] = port;
                _NodeData[nodeId] = ToNodeData(nodeId, node);
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

        public override void Connect(in PortId input, in PortId output)
        {
            base.Connect(in input, in output);
            Save();
        }

        public override void Disconnect(in PortId input, in PortId output)
        {
            base.Disconnect(in input, in output);
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

        protected override IEnumerable<PortData> FindNodePorts(TNode node)
        {
            return NodePortUtility.FindPorts(node);
        }

        protected override NodeData ToNodeData(in NodeId id, TNode node)
        {
            var nodeObject = Graph[id];
            var nodeSerializedProperty = new SerializedObject(nodeObject).FindProperty(nodeObject.NodeSerializedPropertyName);
            var position = nodeObject.Position;
            return new NodeData(new NodePositionProperty(position.x, position.y).Yield()
                .Append(NodeTitleAttribute.CreateTitleProperty(node))
                .Concat(node.CreateProperties(id, nodeSerializedProperty))
                .ToArray()
            );
        }
    }
}

#endif
