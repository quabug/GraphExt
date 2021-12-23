using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphExt.Memory
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NodeTitleAttribute : Attribute
    {
        public string ConstTitle;
        public string TitlePropertyName;

        public static IEnumerable<INodeProperty> CreateTitleProperty(object nodeObject)
        {
            var nodeType = nodeObject.GetType();
            var titleAttribute = nodeType.GetCustomAttribute<NodeTitleAttribute>();
            string title = null;
            if (titleAttribute?.ConstTitle != null)
            {
                title = titleAttribute.ConstTitle;
            }
            else if (titleAttribute?.TitlePropertyName != null)
            {
                title = nodeType
                        .GetMember(titleAttribute.TitlePropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Single()
                        .GetValue<string>(nodeObject)
                    ;
            }
            else if (titleAttribute != null)
            {
                title = nodeType.Name;
            }
            return title == null ? Enumerable.Empty<TitleProperty>() : new TitleProperty(title).Yield();
        }

    }
}