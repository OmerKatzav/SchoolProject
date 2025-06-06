using ProtoBuf;

namespace Shared;

[ProtoContract]
public enum AuthTokenPurpose
{
    [ProtoEnum]
    Login,

    [ProtoEnum]
    AccountRecovery,

    [ProtoEnum]
    FullAccess,

    [ProtoEnum]
    ChangeEmail
}