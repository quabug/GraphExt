using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;

namespace GraphExt.Editor
{
    public class NodeViewPresenter : IViewPresenter
    {
        [NotNull] private readonly UnityEditor.Experimental.GraphView.GraphView _view;
        [NotNull] private readonly INodesViewModule _nodesViewModule;
        [NotNull] private readonly GraphElements<NodeId, Node> _nodes;
        [NotNull] private readonly GraphElements<PortId, Port> _ports;

        public NodeViewPresenter(
            [NotNull] UnityEditor.Experimental.GraphView.GraphView view,
            [NotNull] INodesViewModule nodesViewModule,
            [NotNull] GraphElements<NodeId, Node> nodes,
            [NotNull] GraphElements<PortId, Port> ports
        )
        {
            _view = view;
            _nodesViewModule = nodesViewModule;
            _nodes = nodes;
            _ports = ports;
        }

        public void Tick()
        {
            var newNodes = _nodesViewModule.GetNodes();
            var (added, removed) = _nodes.Ids.Diff(newNodes.Keys);

            foreach (var node in added)
            {
                var nodeView = new Node();
                _nodes.Add(node, nodeView);
                _view.AddElement(nodeView);
            }

            foreach (var node in removed)
            {
                var nodeView = _nodes[node];
                _view.RemoveElement(nodeView);
                _nodes.Remove(node);
            }
        }

        public void Dispose()
        {
        }
    }
}