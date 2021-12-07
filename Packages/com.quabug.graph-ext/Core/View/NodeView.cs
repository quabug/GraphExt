using System;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt
{
    public sealed class NodeView : Node
    {
        private readonly IGraphNode _node;
        private readonly VisualElement _contentContainer;

        public NodeView(IGraphNode node)
            : base(Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml"))
        {
            _node = node;
            style.left = node.Position.x;
            style.top = node.Position.y;
            _contentContainer = this.Q<VisualElement>("contents");
        }
        //
        // public void ConnectTo([NotNull] BehaviorNodeView child)
        // {
        //     child.Node.SetParent(Node);
        // }
        //
        // public void DisconnectFrom([NotNull] BehaviorNodeView parent)
        // {
        //     Node.SetParent(null);
        // }
        //
        // public void SyncPosition()
        // {
        //     Node.Position = GetPosition().position;
        // }

        public override void OnSelected()
        {
            base.OnSelected();
            // Node.IsSelected = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();
            // Node.IsSelected = false;
        }
        //
        // private void Select()
        // {
        //     if (!_graph.selection.Contains(this))
        //     {
        //         Select(_graph, additive: false);
        //         _graph.FrameSelection();
        //     }
        // }
    }
}