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
            field.name = "field";
            this.Q<VisualElement>("value-field").Add(field);
        }

        protected void SetLeftPort(IPortModule portModule)
        {
            var port = portModule.CreatePortView();
            port.name = "left-port";
            this.Q<VisualElement>("left-port").Add(port);
        }

        protected void SetRightPort(IPortModule portModule)
        {
            var port = portModule.CreatePortView();
            port.name = "right-port";
            this.Q<VisualElement>("right-port").Add(port);
        }
    }
}