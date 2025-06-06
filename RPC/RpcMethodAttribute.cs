namespace RPC;

[AttributeUsage(AttributeTargets.Method)]
public class RpcMethodAttribute(uint id) : Attribute
{
    public uint Id { get; } = id;
}