using System;
using System.Runtime.Serialization;

namespace GraphExt
{
    [Serializable]
    public class NodeNotFoundException : Exception
    {
        public NodeId NodeId { get; }
        public NodeNotFoundException(in NodeId nodeId) => NodeId = nodeId;
        public NodeNotFoundException(string message, in NodeId nodeId) : base(message) => NodeId = nodeId;
        public NodeNotFoundException(string message, Exception inner, in NodeId nodeId) : base(message, inner) => NodeId = nodeId;
        protected NodeNotFoundException(SerializationInfo info, StreamingContext context) : base( info, context ) {}
    }

    [Serializable]
    public class InvalidPortException : Exception
    {
        public PortId PortId { get; }
        public InvalidPortException(in PortId portId) => PortId = portId;
        public InvalidPortException(string message, in PortId portId) : base(message) => PortId = portId;
        public InvalidPortException(string message, Exception inner, in PortId portId) : base(message, inner) => PortId = portId;
        protected InvalidPortException(SerializationInfo info, StreamingContext context) : base( info, context ) {}
    }

    [Serializable]
    public class EdgeAlreadyConnectedException : Exception
    {
        public EdgeId EdgeId { get; }
        public EdgeAlreadyConnectedException(in EdgeId edgeId) => EdgeId = edgeId;
        public EdgeAlreadyConnectedException(string message, in EdgeId edgeId) : base(message) => EdgeId = edgeId;
        public EdgeAlreadyConnectedException(string message, Exception inner, in EdgeId edgeId) : base(message, inner) => EdgeId = edgeId;
        protected EdgeAlreadyConnectedException(SerializationInfo info, StreamingContext context) : base( info, context ) {}
    }

    [Serializable]
    public class EdgeAlreadyDisconnectedException : Exception
    {
        public EdgeId EdgeId { get; }
        public EdgeAlreadyDisconnectedException(in EdgeId edgeId) => EdgeId = edgeId;
        public EdgeAlreadyDisconnectedException(string message, in EdgeId edgeId) : base(message) => EdgeId = edgeId;
        public EdgeAlreadyDisconnectedException(string message, Exception inner, in EdgeId edgeId) : base(message, inner) => EdgeId = edgeId;
        protected EdgeAlreadyDisconnectedException(SerializationInfo info, StreamingContext context) : base( info, context ) {}
    }
}