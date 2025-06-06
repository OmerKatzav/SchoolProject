namespace RPC;

public class RpcException(SerializableException ex) : Exception($"{ex.ExceptionType}: {ex.Message}");