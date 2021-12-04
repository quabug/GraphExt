using UnityEditor;
using UnityEngine;

namespace GraphExt
{
    [CreateAssetMenu(fileName = "GraphConfig", menuName = "Graph/New Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = Window.GetOrCreate<GraphWindow>(WindowName);
            if (window.titleContent.text != WindowName) window = EditorWindow.CreateWindow<GraphWindow>(WindowName);
            window.titleContent.text = WindowName;
            window.Focus();
        }
    }
}