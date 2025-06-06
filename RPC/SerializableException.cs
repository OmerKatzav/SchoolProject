using ProtoBuf;

namespace RPC;

[ProtoContract]
public class SerializableException
{
    [ProtoMember(1)]
    public Type? ExceptionType { get; }
    [ProtoMember(2)]
    public string? Message { get; }

    public SerializableException() {}
    public SerializableException(Exception ex)
    {
        ExceptionType = ex.GetType();
        Message = ex.Message;
    }
}