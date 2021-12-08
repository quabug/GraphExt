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

        protected void SetLeftPort(IPortModule portModule)
        {
            this.Q<VisualElement>("left-port").Add(portModule.CreatePortView());
        }

        protected void SetRightPort(IPortModule portModule)
        {
            this.Q<VisualElement>("right-port").Add(portModule.CreatePortView());
        }
    }
}