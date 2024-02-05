namespace Bw.Serialization;

using Bw.Serialization.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using JsonProperty = Newtonsoft.Json.Serialization.JsonProperty;
public class DefaultSerializer : ISerializer
{
    public string ContentType => "application/json";

    public T? Deserialize<T>(string payload, bool camelCase = true)
        => JsonConvert.DeserializeObject<T>(payload, CreateSerializerSettings(camelCase));


    public object? Deserialize(string payload, Type type, bool camelCase = true)
        => JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings(camelCase));


    public string Serialize(object obj, bool camelCase = true, bool indented = true)
        => JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));

    protected static JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
    {
        var settings = new JsonSerializerSettings { ContractResolver = new ContractResolverWithPrivate() };

        if (indented)
        {
            settings.Formatting = Formatting.Indented;
        }

        // for handling private constructor
        settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        settings.Converters.Add(new DateOnlyConverter());

        return settings;
    }

    private class ContractResolverWithPrivate : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property is not null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }
            return prop;
        }
    }
}
