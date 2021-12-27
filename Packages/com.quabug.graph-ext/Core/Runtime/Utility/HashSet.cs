using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GraphExt
{
    public class HashSet<T> : System.Collections.Generic.HashSet<T>, IReadOnlySet<T>
    {
        public HashSet() : base() {}
        public HashSet(IEnumerable<T> collection) : base(collection) {}
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : base(collection, comparer) {}
        public HashSet(IEqualityComparer<T> comparer) : base(comparer) {}
        protected HashSet(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}