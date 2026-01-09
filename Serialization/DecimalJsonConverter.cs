using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WayForPaySDK.Serialization;

public sealed class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
        }

        throw new JsonException($"Cannot convert {reader.TokenType} to decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Use "0.##" format to remove trailing zeros, matching signature calculation format
        var formatted = value.ToString("0.##", CultureInfo.InvariantCulture);
        writer.WriteRawValue(formatted);
    }
}
