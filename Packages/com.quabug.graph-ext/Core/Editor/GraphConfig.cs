using UnityEngine;
using UnityEngine.UIElements;

namespace GraphExt.Editor
{
    [CreateAssetMenu(fileName = "Graph Window Config", menuName = "Graph/New Window Config", order = 0)]
    public class GraphConfig : ScriptableObject
    {
        public string WindowName = "Graph Window";

        public StyleSheet WindowStyleSheet;

        [SerializeReference, SerializeReferenceDrawer(Nullable = false, RenamePatter = @"\w*\.||")]
        public IInstaller[] Installers;

        [ContextMenu("Open Window")]
        public void OpenWindow()
        {
            var window = Window.GetOrCreate<GraphWindow>(WindowName);
            window.titleContent.text = WindowName;
            window.Show(immediateDisplay: true);
            window.Focus();
            window.Config = this;
        }
    }
}