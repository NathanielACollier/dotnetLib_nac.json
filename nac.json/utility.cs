using System.Text.Json.Serialization;
using System.Text.Json;
using System;
namespace nac.json;

public static class utility
{
    public static object DeserializeToDictionaryList(string jsonText, model.ObjectFormat objectFormat = model.ObjectFormat.Dictionary)
    {
        var options = new JsonSerializerOptions
        {
            Converters = {
                new converters.JSONDictionaryConverter(
                floatFormat : model.FloatFormat.Decimal,
                    unknownNumberFormat : model.UnknownNumberFormat.Error,
                    objectFormat : objectFormat)
            }
        };
        object d = System.Text.Json.JsonSerializer.Deserialize<object>(jsonText, options);
        return d;
    }



    public static string SerializeToJSON(object obj)
    {
        string jsoNText = System.Text.Json.JsonSerializer.Serialize(obj, options: new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });

        return jsoNText;
    }

    public static T DeserializeJSON<T>(string jsonText)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true, // some API modules are using fields for the argument classes
            Converters =
            {
                new converters.BooleanJSONConverter(),
                new converters.NullableDateTimeJSONConverter()
            }
        };

        T result = System.Text.Json.JsonSerializer.Deserialize<T>(json: jsonText, options: options);

        return result;
    }
}