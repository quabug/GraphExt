using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GraphExt.Editor
{
    [CreateAssetMenu(fileName = "GraphConfig", menuName = "Graph/New Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";

        [SerializedType(typeof(IGraph), Where = nameof(IsValidBackendType))] public string Backend;
        private bool IsValidBackendType(Type type) => !type.IsAbstract && !type.IsGenericType;

        [SerializeReference, SerializeReferenceDrawer] public IMenuEntry[] Menu;
        [SerializeReference, SerializeReferenceDrawer] public INodeViewFactory NodeViewFactory = new DefaultNodeViewFactory();

        private void Reset()
        {
            Menu = TypeCache
                .GetTypesDerivedFrom<IMenuEntry>()
                .Where(type => !type.IsAbstract && !type.IsGenericType && type.GetConstructor(Array.Empty<Type>()) != null)
                .Select(type => (IMenuEntry)Activator.CreateInstance(type))
                .ToArray()
            ;
            NodeViewFactory = new DefaultNodeViewFactory();
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