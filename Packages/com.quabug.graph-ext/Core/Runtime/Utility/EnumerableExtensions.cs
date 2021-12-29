using System;
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

        public static T[] RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> match)
        {
            var removed = collection.Where(match).ToArray();
            foreach (var item in removed) collection.Remove(item);
            return removed;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new HashSet<T>(items);
    }
}