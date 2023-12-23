using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmsRuNotificationService.Core.Helpers;

public class AutoNumberToStringConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(string) == typeToConvert;
    }
    
    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Number:
                return reader.TryGetInt64(out var l) ?
                    l.ToString():
                    reader.GetDouble().ToString(CultureInfo.CurrentCulture);
            case JsonTokenType.String:
                return reader.GetString() ?? string.Empty;
            case JsonTokenType.None:
            case JsonTokenType.StartObject:
            case JsonTokenType.EndObject:
            case JsonTokenType.StartArray:
            case JsonTokenType.EndArray:
            case JsonTokenType.PropertyName:
            case JsonTokenType.Comment:
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Null:
            default:
            {
                using var document = JsonDocument.ParseValue(ref reader);
                return document.RootElement.Clone().ToString();
            }
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}