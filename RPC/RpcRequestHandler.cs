using Microsoft.Extensions.Logging;
using ProtoBuf;
using System.Collections.Concurrent;
using System.Reflection;

namespace RPC;

public class RpcRequestHandler(DI.IServiceProvider serviceProvider, ILogger logger) : IRequestHandler
{
    private readonly ConcurrentDictionary<uint, Type> _serviceIdMap = [];
    private readonly ConcurrentDictionary<(Type, uint), MethodInfo> _methodIdMap = [];
    public IEnumerable<byte> HandleRequest(IEnumerable<byte> request)
    {
        try
        {
            var requestArr = request.ToArray();
            var serviceId = BitConverter.ToUInt32(requestArr.Take(4).ToArray());
            var service = GetService(serviceId);
            var methodId = BitConverter.ToUInt32(requestArr.Skip(4).Take(4).ToArray());
            var method = GetMethod(methodId, service);
            var args = DeserializeArgs(requestArr.Skip(8), method);
            var instance = serviceProvider.GetService(service);
            if (method.GetParameters().Any(info =>
                    info.ParameterType.IsAssignableFrom(typeof(CancellationToken)) ||
                    info.ParameterType.IsAssignableFrom(typeof(Action<CallMetadata>))))
            {
                var argsEnumerator = args.GetEnumerator();
                args =
                [
                    .. method.GetParameters().Select(param =>
                    {
                        if (param.ParameterType.IsAssignableFrom(typeof(CancellationToken)))
                            return CancellationToken.None; // Default cancellation token
                        if (param.ParameterType.IsAssignableFrom(typeof(Action<CallMetadata>)))
                            return (Action<CallMetadata>)(_ => { });
                        argsEnumerator.MoveNext();
                        return argsEnumerator.Current!;
                    })
                ];
            }

            var result = method.Invoke(instance, args);
            if (result is Task task)
            {
                task.GetAwaiter().GetResult();
                if (result.GetType().IsGenericType)
                {
                    var resultProp = result.GetType().GetProperty("Result") ?? throw new InvalidOperationException("Result property not found on task result type");
                    result = resultProp.GetValue(result);
                }
                else
                {
                    result = null;
                }
            }

            using var ms = new MemoryStream();
            ms.WriteByte(0);
            Serializer.Serialize(ms, result);
            return ms.ToArray();
        }
        catch (TargetInvocationException ex)
        {
            logger.LogError(ex, "Error while handling request");
            using var ms = new MemoryStream();
            ms.WriteByte(1);
            Serializer.Serialize(ms, new SerializableException(ex.InnerException!));
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while handling request");
            using var ms = new MemoryStream();
            ms.WriteByte(1);
            Serializer.Serialize(ms, new SerializableException(ex));
            return ms.ToArray();
        }
    }

    private MethodInfo GetMethod(uint id, Type serviceType)
    {
        if (_methodIdMap.TryGetValue((serviceType, id), out var method)) return method;
        foreach (var serviceMethod in serviceType.GetMethods())
        {
            var methodAttribute = serviceMethod.GetCustomAttribute<RpcMethodAttribute>();
            if (methodAttribute == null) continue;
            _methodIdMap.TryAdd((serviceType, methodAttribute.Id), serviceMethod);
            if (methodAttribute.Id == id) return serviceMethod;
        }
        throw new InvalidOperationException($"Method with id {id} not found in service {serviceType.Name}");
    }

    private Type GetService(uint id)
    {
        if (_serviceIdMap.TryGetValue(id, out var serviceType)) return serviceType;
        foreach (var service in serviceProvider.GetRegisteredTypes())
        {
            var serviceAttribute = service.GetCustomAttribute<RpcServiceAttribute>();
            if (serviceAttribute == null) continue;
            _serviceIdMap.TryAdd(serviceAttribute.Id, service);
            if (serviceAttribute.Id == id) return service;
        }
        throw new InvalidOperationException($"Service with id {id} not found");
    }

    private static object[] DeserializeArgs(IEnumerable<byte> argBytes, MethodInfo method)
    {
        var args = new List<object>();
        var argBytesArr = argBytes.ToArray();
        var offset = 0;
        foreach (var parameter in method.GetParameters().Where(info =>
                     !info.ParameterType.IsAssignableFrom(typeof(CancellationToken)) &&
                     !info.ParameterType.IsAssignableFrom(typeof(Action<CallMetadata>))))
        {
            var length = BitConverter.ToInt32(argBytesArr.Skip(offset).Take(4).ToArray());
            offset += 4;
            var arg = Serializer.Deserialize(parameter.ParameterType, new MemoryStream([.. argBytesArr.Skip(offset).Take(length)]));
            args.Add(arg);
            offset += length;
        }
        return [.. args];
    }
}