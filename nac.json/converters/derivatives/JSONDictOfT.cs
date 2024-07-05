using nac.json.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace nac.json.converters.derivatives;

public class JSONDictOfT<T> : JsonConverter<T>
{
    private JSONDictionaryConverter dictConverter;

    private bool isDictionaryStrObj => typeof(T) == typeof(Dictionary<string, object>);

    private bool isDictionaryList => typeof(T).IsGenericType &&
                                     typeof(T).GetGenericTypeDefinition() == typeof(List<>) &&
                                     typeof(T).GetGenericArguments()[0] == typeof(Dictionary<string, object>);

    public JSONDictOfT()
    {
        if (!isDictionaryStrObj &&
            !isDictionaryList)
        {
            throw new Exception("Cannot convert from Type of T = " + typeof(T).Name);
        }

        dictConverter = new JSONDictionaryConverter(floatFormat: FloatFormat.Decimal,
            unknownNumberFormat: UnknownNumberFormat.Error,
            objectFormat: ObjectFormat.Dictionary);
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var obj = dictConverter.Read(reader: ref reader,
            typeToConvert: typeToConvert,
            options: options);

        return transformObject(obj);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        dictConverter.Write(writer: writer,
            value: value,
            options: options);
    }


    private T transformObject(object obj)
    {

        if (isDictionaryList)
        {
            // Dictionary List because we are using our converter and the only lists it puts out are dictionary lists because we assume JSONDictOfT will only be used on Dictionary or List<Dictionary
            var objList = obj as List<object>;
            var result = new List<Dictionary<string, object>>();
            foreach (var dict in objList)
            {
                result.Add(dict as Dictionary<string, object>);
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        return (T)obj;
    }
}



