using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    [CreateAssetMenu(fileName = "Graph Window Config", menuName = "Graph/New Window Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";
        [SerializedType(typeof(BaseGraphWindow), InstantializableType = true, Nullable = false)] public string WindowType;
        [SerializeReference, SerializeReferenceDrawer] public IGraphElementViewFactory[] ViewFactories;
        public StyleSheet WindowStyleSheet;

        public T GetViewFactory<T>() where T : IGraphElementViewFactory
        {
            return ViewFactories.OfType<T>().First();
        }

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = (BaseGraphWindow) Window.GetOrCreate(Type.GetType(WindowType), WindowName);
            window.titleContent.text = WindowName;
            window.Show(immediateDisplay: true);
            window.Focus();
            window.Config = this;
        }
    }
}