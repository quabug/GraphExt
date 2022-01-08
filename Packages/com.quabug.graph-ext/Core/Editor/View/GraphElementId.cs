// using System;
//
// namespace GraphExt.Editor
// {
//     public readonly struct GraphElementId : IEquatable<GraphElementId>
//     {
//         public readonly Guid Id;
//         public GraphElementId(Guid id) => Id = id;
//
//         public override string ToString()
//         {
//             return Id.ToString();
//         }
//
//         public static explicit operator Guid(GraphElementId nodeId) => nodeId.Id;
//         public static implicit operator GraphElementId(Guid id) => new GraphElementId(id);
//
//         public bool Equals(GraphElementId other)
//         {
//             return Id.Equals(other.Id);
//         }
//
//         public override bool Equals(object obj)
//         {
//             return obj is GraphElementId other && Equals(other);
//         }
//
//         public override int GetHashCode()
//         {
//             return Id.GetHashCode();
//         }
//
//         public static bool operator ==(in GraphElementId lhs, in GraphElementId rhs)
//         {
//             return lhs.Equals(rhs);
//         }
//
//         public static bool operator !=(in GraphElementId lhs, in GraphElementId rhs)
//         {
//             return !(lhs == rhs);
//         }
//     }
// }