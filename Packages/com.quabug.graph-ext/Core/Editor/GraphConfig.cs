using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    [CreateAssetMenu(fileName = "Graph Window Config", menuName = "Graph/New Window Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";

        [SerializeReference, SerializeReferenceDrawer] public IWindowExtension[] WindowExtensions;
        [SerializeReference, SerializeReferenceDrawer] public IMenuEntry[] Menu;
        [SerializeReference, SerializeReferenceDrawer] public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();
        [SerializeReference, SerializeReferenceDrawer] public IPortViewFactory PortViewFactory = new DefaultPortViewFactory();
        [SerializeReference, SerializeReferenceDrawer] public IEdgeViewFactory EdgeViewFactory = new DefaultEdgeViewFactory();

        private void Reset()
        {
            Menu = TypeCache
                .GetTypesDerivedFrom<IMenuEntry>()
                .Where(type => !type.IsAbstract && !type.IsGenericType && type.GetConstructor(Array.Empty<Type>()) != null)
                .Select(type => (IMenuEntry)Activator.CreateInstance(type))
                .ToArray()
            ;
            NodeViewFactory = new DefaultNodeViewFactory();
            PortViewFactory = new DefaultPortViewFactory();
            EdgeViewFactory = new DefaultEdgeViewFactory();
        }

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = Window.GetOrCreate<GraphWindow>(WindowName);
            window.titleContent.text = WindowName;
            window.Init(this);
            window.Focus();
        }
    }
}