using System.Linq;

namespace GraphExt.Editor
{
    public class TagTreePortInstaller<TNode> : IGraphInstaller where TNode : ITreeNode<GraphRuntime<TNode>>
    {
        public string[] AdditionalClasses;

        public void Install(Container container, TypeContainers typeContainers)
        {
            var presenterContainer = typeContainers.GetTypeContainer(typeof(NodeViewPresenter));
            presenterContainer.Register<FindPortData>((resolveContainer, contractType) =>
            {
                var nodes = presenterContainer.Resolve<IReadOnlyBiDictionary<NodeId, TNode>>();
                return (in NodeId nodeId) =>
                {
                    var node = nodes[nodeId];
                    return NodePortUtility.FindPorts(nodes[nodeId]).Select(portData =>
                    {
                        if (portData.Name == node.InputPortName || portData.Name == node.OutputPortName)
                            return portData.AddClass(AdditionalClasses);
                        return portData;
                    });
                };
            }).AsSelf();
        }
    }
}