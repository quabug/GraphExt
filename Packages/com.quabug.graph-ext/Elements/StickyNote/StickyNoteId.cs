using System;

namespace GraphExt
{
    public readonly struct StickyNoteId : IEquatable<StickyNoteId>
    {
        public readonly Guid Id;
        public StickyNoteId(Guid id) => Id = id;
        public StickyNoteId(string id) => Id = Guid.Parse(id);

        public override string ToString()
        {
            return Id.ToString();
        }

        public static explicit operator Guid(StickyNoteId nodeId) => nodeId.Id;
        public static implicit operator StickyNoteId(Guid id) => new StickyNoteId(id);

        public bool Equals(StickyNoteId other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            return obj is StickyNoteId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(in StickyNoteId lhs, in StickyNoteId rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(in StickyNoteId lhs, in StickyNoteId rhs)
        {
            return !(lhs == rhs);
        }
    }
}