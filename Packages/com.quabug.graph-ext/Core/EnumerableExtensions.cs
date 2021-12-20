using System.Collections.Generic;
using System.Linq;

namespace GraphExt
{
    internal static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> list, T value)
        {
            var index = 0;
            foreach (var element in list)
            {
                if (Equals(value, element)) return index;
                index++;
            }
            return -1;
        }

        public static IEnumerable<T> Yield<T>(this T element)
        {
            yield return element;
        }
    }
}