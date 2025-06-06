using Castle.DynamicProxy;
using ProtoBuf;
using System.Reflection;

namespace RPC;

public class RpcInterceptor(INetworkService networkService) : IAsyncInterceptor
{
    public void InterceptSynchronous(IInvocation invocation)
    {
        var ctx = CancellationToken.None;
        Action<CallMetadata>? callback = null;
        var args = invocation.Arguments.ToArray();
        if (args.Any(arg => arg is CallMetadata))
        {
            ctx = (CancellationToken)args.First(arg => arg is CancellationToken)!;
            args = [.. args.Where(arg => arg is not CancellationToken)];
        }
        if (args.Any(arg => arg is Action<CallMetadata>))
        {
            callback = (Action<CallMetadata>)args.First(arg => arg is Action<CallMetadata>)!;
            args = [.. args.Where(arg => arg is not Action<CallMetadata>)];
        }
        var result = networkService.Request(SerializeRequest(invocation, args), callback, ctx).ToArray();
        if (result[0] == 0) invocation.ReturnValue = Serializer.NonGeneric.Deserialize(invocation.Method.ReturnType, (ReadOnlyMemory<byte>)result);
        else
        {
            var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
            throw new RpcException(ex);
        }
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        var ctx = CancellationToken.None;
        Action<CallMetadata>? callback = null;
        var args = invocation.Arguments.ToArray();
        if (args.Any(arg => arg is CallMetadata))
        {
            ctx = (CancellationToken)args.First(arg => arg is CancellationToken)!;
            args = [.. args.Where(arg => arg is not CancellationToken)];
        }
        if (args.Any(arg => arg is Action<CallMetadata>))
        {
            callback = (Action<CallMetadata>)args.First(arg => arg is Action<CallMetadata>)!;
            args = [.. args.Where(arg => arg is not Action<CallMetadata>)];
        }
        invocation.ReturnValue = networkService.RequestAsync(SerializeRequest(invocation, args), callback, ctx).ContinueWith(t =>
        {
            var result = t.Result.ToArray();
            if (result[0] == 0) return;
            var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
            throw new RpcException(ex);
        }, ctx);
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var ctx = CancellationToken.None;
        Action<CallMetadata>? callback = null;
        var args = invocation.Arguments.ToArray();
        if (args.Any(arg => arg is CallMetadata))
        {
            ctx = (CancellationToken)args.First(arg => arg is CancellationToken)!;
            args = [.. args.Where(arg => arg is not CancellationToken)];
        }
        if (args.Any(arg => arg is Action<CallMetadata>))
        {
            callback = (Action<CallMetadata>)args.First(arg => arg is Action<CallMetadata>)!;
            args = [.. args.Where(arg => arg is not Action<CallMetadata>)];
        }
        invocation.ReturnValue = networkService.RequestAsync(SerializeRequest(invocation, args), callback, ctx).ContinueWith(t =>
        {
            var result = t.Result.ToArray();
            if (result[0] == 0) return Serializer.Deserialize<TResult>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
            var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
            throw new RpcException(ex);
        }, ctx);
    }

    private static List<byte> SerializeRequest(IInvocation invocation, object?[]? args = null)
    {
        args ??= [.. invocation.Arguments];
        var serialized = SerializeArgs(args);
        var id = invocation.Method.GetCustomAttribute<RpcMethodAttribute>() ?? throw new InvalidOperationException($"Method {invocation.Method.Name} is not marked with RpcMethodAttribute");
        var serviceId = invocation.Method.DeclaringType?.GetCustomAttribute<RpcServiceAttribute>() ?? throw new InvalidOperationException($"Service {invocation.Method.DeclaringType?.Name} is not marked with RpcServiceAttribute");
        serialized.InsertRange(0, BitConverter.GetBytes(id.Id));
        serialized.InsertRange(0, BitConverter.GetBytes(serviceId.Id));
        return serialized;
    }

    private static List<byte> SerializeArgs(object?[] args)
    {
        List<byte> data = [];
        foreach (var arg in args)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, arg);
            data.AddRange(BitConverter.GetBytes(stream.Length));
            data.AddRange(stream.ToArray());
        }
        return data;
    }
}