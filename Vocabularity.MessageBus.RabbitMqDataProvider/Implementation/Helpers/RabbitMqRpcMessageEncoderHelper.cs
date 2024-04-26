using System.Text;

using Newtonsoft.Json;

namespace Vocabularity.MessageBus.RabbitMqDataProvider.Implementation.Helpers;

internal static class RabbitMqRpcMessageEncoderHelper
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    internal static byte[] Encode<TObj>(TObj obj)
    {
        var objBytes = Encoding.GetBytes(JsonConvert.SerializeObject(obj));
        return objBytes;
    }

    internal static TObj Decode<TObj>(byte[] objBytes)
    {
        var objStr = Encoding.GetString(objBytes);
        var obj = JsonConvert.DeserializeObject<TObj>(objStr);
        return obj;
    }

    internal static object Decode(byte[] objBytes, Type messageType)
    {
        var objStr = Encoding.GetString(objBytes);
        var obj = JsonConvert.DeserializeObject(objStr, messageType);
        return obj;
    }
}