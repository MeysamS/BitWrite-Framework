using Newtonsoft.Json;
using System.Globalization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Bw.Serialization.Converters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer) => DateOnly.ParseExact((string)reader.Value!, Format, CultureInfo.InvariantCulture);


    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
