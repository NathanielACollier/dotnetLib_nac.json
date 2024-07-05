using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.model;

public class TableFilterEntry
{

    public string ColumnName { get; set; }

    [System.Text.Json.Serialization.JsonConverter(typeof(nac.json.converters.JsonPrimitiveObjectConverter))]
    public object Value { get; set; }

    public string Operation { get; set; }
}


