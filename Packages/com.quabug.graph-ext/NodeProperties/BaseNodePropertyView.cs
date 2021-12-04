using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace GraphExt
{
    public class BaseNodePropertyView : VisualElement
    {
        public BaseNodePropertyView()
        {
            var relativeDirectory = Utilities.GetCurrentDirectoryProjectRelativePath();
            var uxmlPath = Path.Combine(relativeDirectory, "NodePropertyView.uxml");
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            visualTree.CloneTree(this);
        }

        protected void SetTitle(string title)
        {
            this.Q<Label>("title").text = title;
        }

        protected void SetField(VisualElement field)
        {
            this.Q<VisualElement>("value-field").Add(field);
        }

        protected void SetLeftPort(INodePort port)
        {
            this.Q<VisualElement>("left-port").Add(port.CreatePortView());
        }

        protected void SetRightPort(INodePort port)
        {
            this.Q<VisualElement>("right-port").Add(port.CreatePortView());
        }
    }
}