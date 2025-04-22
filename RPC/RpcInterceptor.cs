using Castle.DynamicProxy;
using ProtoBuf;
using System.Reflection;

namespace RPC
{
    public class RpcInterceptor(INetworkService networkService) : IAsyncInterceptor
    {
        public void InterceptSynchronous(IInvocation invocation)
        {
            var result = networkService.Request(SerializeRequest(invocation)).ToArray();
            if (result[0] == 0) invocation.ReturnValue = Serializer.NonGeneric.Deserialize(invocation.Method.ReturnType, (ReadOnlyMemory<byte>)result);
            else
            {
                var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
                throw new RpcException(ex);
            }
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = networkService.RequestAsync(SerializeRequest(invocation)).ContinueWith(t =>
            {
                var result = t.Result.ToArray();
                if (result[0] == 0) return;
                var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
                throw new RpcException(ex);
            });
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            invocation.ReturnValue = networkService.RequestAsync(SerializeRequest(invocation)).ContinueWith(t =>
            {
                var result = t.Result.ToArray();
                if (result[0] == 0) return Serializer.Deserialize<TResult>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
                var ex = Serializer.Deserialize<SerializableException>((ReadOnlyMemory<byte>)result.Skip(1).ToArray());
                throw new RpcException(ex);
            });
        }

        private static List<byte> SerializeRequest(IInvocation invocation)
        {
            var serialized = SerializeArgs(invocation.Arguments);
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
}
