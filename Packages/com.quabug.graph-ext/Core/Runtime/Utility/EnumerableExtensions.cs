using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GraphExt
{
    public static class EnumerableExtensions
    {
        internal static int IndexOf<T>(this IEnumerable<T> list, T value)
        {
            var index = 0;
            foreach (var element in list)
            {
                if (Equals(value, element)) return index;
                index++;
            }
            return -1;
        }

        internal static IEnumerable<T> Yield<T>(this T element)
        {
            yield return element;
        }

        internal static T[] RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> match)
        {
            var removed = collection.Where(match).ToArray();
            foreach (var item in removed) collection.Remove(item);
            return removed;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new HashSet<T>(items);

        public static (IReadOnlySet<T> added, IReadOnlySet<T> removed)
            Diff<T>([NotNull] this IEnumerable<T> origin, [NotNull] IEnumerable<T> items)
        {
            var removed = new HashSet<T>(origin);
            var added = new HashSet<T>();
            foreach (var item in items)
            {
                if (removed.Contains(item)) removed.Remove(item);
                else added.Add(item);
            }
            return (added, removed);
        }
    }
}