using nac.json.model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace nac.json.converters;


/*
 original from: https://stackoverflow.com/questions/65972825/c-sharp-deserializing-nested-json-to-nested-dictionarystring-object
 */
public class JSONDictionaryConverter : JsonConverter<object>
{
    FloatFormat FloatFormat { get; init; }
    UnknownNumberFormat UnknownNumberFormat { get; init; }
    ObjectFormat ObjectFormat { get; init; }

    public JSONDictionaryConverter() : this(FloatFormat.Double, UnknownNumberFormat.Error, ObjectFormat.Expando) { }
    public JSONDictionaryConverter(FloatFormat floatFormat, UnknownNumberFormat unknownNumberFormat, ObjectFormat objectFormat)
    {
        this.FloatFormat = floatFormat;
        this.UnknownNumberFormat = unknownNumberFormat;
        this.ObjectFormat = objectFormat;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        if (value.GetType() == typeof(object))
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.True:
                return true;
            case JsonTokenType.String:
                return ReadString(ref reader);
            case JsonTokenType.Number:
                return ReadNumber(ref reader);
            case JsonTokenType.StartArray:
                return ReadArray(ref reader, options);
            case JsonTokenType.StartObject:
                return ReadObject(ref reader, options);
            default:
                throw new JsonException($"Unknown token {reader.TokenType}");
        }
    }


    private static object ReadString(ref Utf8JsonReader reader)
    {
        string text = reader.GetString();

        if (DateTime.TryParse(text, out DateTime d))
        {
            return d;
        }

        return text;
    }



    private object ReadNumber(ref Utf8JsonReader reader)
    {
        if (reader.TryGetInt32(out var i))
            return i;
        if (reader.TryGetInt64(out var l))
            return l;
        // BigInteger could be added here.
        if (FloatFormat == FloatFormat.Decimal && reader.TryGetDecimal(out var m))
            return m;

        if (FloatFormat == FloatFormat.Double && reader.TryGetDouble(out var d))
            return d;

        using var doc = JsonDocument.ParseValue(ref reader);
        if (UnknownNumberFormat == UnknownNumberFormat.JsonElement)
            return doc.RootElement.Clone();
        throw new JsonException(string.Format("Cannot parse number {0}", doc.RootElement.ToString()));
    }


    private object ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (ObjectFormat == ObjectFormat.Flexpando)
        {
            return ReadObjectAsFlexpando(ref reader, options);
        }

        return ReadObjectAsDictionary(ref reader, options);
    }

    private object ReadObjectAsFlexpando(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var dict = new model.Flexpando();
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return dict;
                case JsonTokenType.PropertyName:
                    var key = reader.GetString();
                    reader.Read();
                    dict.Dictionary.Add(key, Read(ref reader, typeof(object), options));
                    break;
                default:
                    throw new JsonException();
            }
        }
        throw new JsonException();
    }

    private object ReadObjectAsDictionary(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var dict = CreateDictionary();
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    return dict;
                case JsonTokenType.PropertyName:
                    var key = reader.GetString();
                    reader.Read();
                    dict.Add(key, Read(ref reader, typeof(object), options));
                    break;
                default:
                    throw new JsonException();
            }
        }
        throw new JsonException();
    }


    private object ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        var list = new List<object>();
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                default:
                    list.Add(Read(ref reader, typeof(object), options));
                    break;
                case JsonTokenType.EndArray:
                    return list;
            }
        }
        throw new JsonException();
    }


    protected virtual IDictionary<string, object> CreateDictionary()
    {
        if (ObjectFormat == ObjectFormat.Expando)
        {
            return new ExpandoObject();
        }

        return new Dictionary<string, object>(comparer: StringComparer.OrdinalIgnoreCase);
    }

}


