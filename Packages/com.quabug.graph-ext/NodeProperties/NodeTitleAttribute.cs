using System;
using System.Linq;
using System.Reflection;

namespace GraphExt
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeTitleAttribute : Attribute
    {
        public bool Hidden = false;
        public string ConstTitle;
        public string TitlePropertyName;

        public static INodeProperty CreateTitleProperty(object nodeObject)
        {
            var title = GetTitle(nodeObject);
            return title == null ? null : new TitleProperty(title);
        }

        public static string GetTitle(object nodeObject)
        {
            var nodeType = nodeObject.GetType();
            var titleAttribute = nodeType.GetCustomAttribute<NodeTitleAttribute>();
            if (titleAttribute == null || titleAttribute.Hidden) return null;

            var title = nodeType.Name;
            if (titleAttribute.ConstTitle != null)
            {
                title = titleAttribute.ConstTitle;
            }
            else if (titleAttribute.TitlePropertyName != null)
            {
                title = nodeType
                        .GetMember(titleAttribute.TitlePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single()
                        .GetValue<string>(nodeObject)
                    ;
            }
            return title;
        }
    }
}