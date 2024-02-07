using Bw.Core.Messaging;
using Bw.Core.Utils;
using Bw.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace Bw.Messaging.Serializations;

public class DefaultMessageSerializer : DefaultSerializer, IMessageSerializer
{
    public new string ContentType => "application/json";

    public MessageEnvelope? Deserialize(string json)
        => JsonConvert.DeserializeObject<MessageEnvelope>(json, CreateSerializerSettings());

    public IMessage? Deserialize(ReadOnlySpan<byte> data, string payloadType)
    {
        var type = TypeMapper.GetType(payloadType);

        var json = Encoding.UTF8.GetString(data);
        var deserializedData = JsonConvert.DeserializeObject(json, type, CreateSerializerSettings());

        return deserializedData as IMessage;
    }

    public TMessage? Deserialize<TMessage>(string message) where TMessage : IMessage
        => JsonConvert.DeserializeObject<TMessage>(message, CreateSerializerSettings());


    public object? Deserialize(string payload, string payloadType)
    {
        var type = TypeMapper.GetType(payloadType);
        var deserializedData = JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings());

        return deserializedData;
    }

    public string Serialize(MessageEnvelope messageEnvelope)
        => JsonConvert.SerializeObject(messageEnvelope, new JsonSerializerSettings());

    public string Serialize<TMessage>(TMessage message) where TMessage : IMessage
        => JsonConvert.SerializeObject(message, CreateSerializerSettings());
}
