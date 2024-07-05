using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace nac.json.converters;

/*
 original code from:
    https://stackoverflow.com/questions/59430728/how-to-handle-both-a-single-item-and-an-array-for-the-same-property-using-system

 MS Documentation on this:
    https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-8-0

 */

public class SingleObjectOrArrayJsonConverter<T> : JsonConverter<List<T>>
{

    public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.StartArray => JsonSerializer.Deserialize<List<T>>(ref reader, options),
            _ => new List<T> { JsonSerializer.Deserialize<T>(ref reader, options) }
        };
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        if (value.Count == 1)
        {
            JsonSerializer.Serialize(writer, value.First(), options);
        }
        else
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }


}


