using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flowly.WorkflowSource.Json.Internal
{
    internal class ExpandoObjectConverter : JsonConverter<ExpandoObject>
    {
        public override ExpandoObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new InvalidOperationException();

            var result = new ExpandoObject();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return result;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new InvalidOperationException();

                var propertyName = reader.GetString();

                reader.Read();

                result.TryAdd(propertyName!, GetValue(reader, options));
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, ExpandoObject value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }

        private object? GetValue(Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    {
                        if (reader.TryGetInt64(out var value))
                            return value;

                        return reader.GetDecimal();
                    }
                case JsonTokenType.StartObject:
                    return Read(ref reader, null!, options);

            }

            return null;
        }
    }
}
