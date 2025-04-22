using ProtoBuf;

namespace RPC
{
    [ProtoContract]
    public class SerializableException(Exception ex)
    {
        [ProtoMember(1)]
        public Type ExceptionType { get; } = ex.GetType();
        [ProtoMember(2)]
        public string Message { get; } = ex.Message;
    }
}
