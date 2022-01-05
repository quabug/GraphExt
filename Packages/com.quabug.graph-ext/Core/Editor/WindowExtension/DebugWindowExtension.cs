using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GraphExt.Editor
{
    public class DebugWindowExtension : IWindowExtension
    {
        public bool PrintNodeCreated = false;
        public bool PrintNodeDeleted = false;
        public bool PrintPortCreated = false;
        public bool PrintPortDeleted = false;
        public bool PrintEdgeCreated = false;
        public bool PrintEdgeDeleted = false;

        public void OnInitialized(GraphWindow window, GraphConfig config, GraphView view)
        {
            view.OnNodeCreated += OnNodeCreated;
            view.OnNodeWillDelete += OnNodeDeleted;
            view.OnPortCreated += OnPortCreated;
            view.OnPortWillDelete += OnPortDeleted;
            view.OnEdgeCreated += OnEdgeCreated;
            view.OnEdgeWillDelete += OnEdgeDeleted;
        }

        public void OnClosed(GraphWindow window, GraphConfig config, GraphView view)
        {
            view.OnNodeCreated -= OnNodeCreated;
            view.OnNodeWillDelete -= OnNodeDeleted;
            view.OnPortCreated -= OnPortCreated;
            view.OnPortWillDelete -= OnPortDeleted;
            view.OnEdgeCreated -= OnEdgeCreated;
            view.OnEdgeWillDelete -= OnEdgeDeleted;
        }

        private void OnNodeCreated(in NodeId nodeId, Node node)
        {
            if (PrintNodeCreated) Debug.Log($"[Node] created: [{nodeId}]{node.name}");
        }

        private void OnNodeDeleted(in NodeId nodeId, Node node)
        {
            if (PrintNodeDeleted) Debug.Log($"[Node] deleted: [{nodeId}]{node.name}");
        }

        private void OnPortCreated(in PortId portId, Port port)
        {
            if (PrintPortCreated) Debug.Log($"[Port] created: [{portId.NodeId}]{portId.Name}");
        }

        private void OnPortDeleted(in PortId portId, Port port)
        {
            if (PrintPortDeleted) Debug.Log($"[Port] deleted: [{portId.NodeId}]{portId.Name}");
        }

        private void OnEdgeCreated(in EdgeId edgeId, Edge edge)
        {
            if (PrintEdgeCreated) Debug.Log($"[Edge] created: {edgeId.Output.Name}->{edgeId.Input.Name}");
        }

        private void OnEdgeDeleted(in EdgeId edgeId, Edge edge)
        {
            if (PrintEdgeDeleted) Debug.Log($"[Edge] deleted: {edgeId.Output.Name}->{edgeId.Input.Name}");
        }
    }
}