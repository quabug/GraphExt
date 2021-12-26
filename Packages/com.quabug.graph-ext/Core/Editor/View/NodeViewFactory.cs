﻿using System.IO;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    public interface INodeViewFactory
    {
        [CanBeNull] Node Create(NodeData nodeData);
    }

    public class DefaultNodeViewFactory : INodeViewFactory
    {
        public GroupNodePropertyViewFactory NodePropertyViewFactory = new GroupNodePropertyViewFactory
        {
            Factories = new INodePropertyViewFactory[]{ new DefaultPropertyViewFactory() }
        };

        public Node Create(NodeData data)
        {
            var defaultUxml = Path.Combine(Utilities.GetCurrentDirectoryProjectRelativePath(), "NodeView.uxml");
            var nodeView = new Node(defaultUxml);
            var container = nodeView.ContentContainer();
            foreach (var property in data.Properties)
            {
                var propertyView = CreatePropertyView(property);
                if (propertyView != null) container.Add(propertyView);
            }
            return nodeView;

            VisualElement CreatePropertyView(INodeProperty property)
            {
                return NodePropertyViewFactory.Create(nodeView, property, new INodePropertyViewFactory.Null());
            }
        }

    }
}