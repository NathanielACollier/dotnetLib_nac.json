using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.model;

public class GenericType1
{
    [System.Text.Json.Serialization.JsonConverter(typeof(nac.json.converters.derivatives.JSONDictOfT<List<Dictionary<string, object>>>))]
    public List<Dictionary<string, object>> Items { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(nac.json.converters.derivatives.JSONDictOfT<Dictionary<string, object>>))]
    public Dictionary<string, object> ItemA { get; set; }

    public long TotalCount { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(nac.json.converters.derivatives.JSONDictOfT<Dictionary<string, object>>))]
    public Dictionary<string, object> ItemB { get; set; }
}
