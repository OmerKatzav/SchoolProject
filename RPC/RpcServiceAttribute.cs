namespace RPC;

[AttributeUsage(AttributeTargets.Interface)]
public class RpcServiceAttribute(uint id) : Attribute
{
    public uint Id { get; } = id;
}