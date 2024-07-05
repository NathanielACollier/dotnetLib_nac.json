using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace Tests.model;

public class Invoice
{
    public int id { get; set; }
    public bool? rushForPayment { get; set; }
    public decimal amount { get; set; }
    public DateTime? invoiceDate { get; set; }
    public DateTime dueDate { get; set; }
    public long? fileSize { get; set; }

    [JsonConverter(typeof(nac.json.converters.SingleObjectOrArrayJsonConverter<LineItem>))]
    public List<LineItem> lines { get; set; }

    [JsonConverter(typeof(nac.json.converters.SingleObjectOrArrayJsonConverter<string>))]
    public List<string> target { get; set; }


    [JsonConverter(typeof(nac.json.converters.SingleObjectOrArrayJsonConverter<int>))]
    public List<int> num { get; set; }


    [JsonConverter(typeof(nac.json.converters.factories.JsonConverterFactoryForListOfT))]
    public List<int> numFactory { get; set; }

    public Invoice()
    {
        this.lines = new List<LineItem>();
        this.target = new List<string>();
        this.num = new List<int>();
        this.numFactory = new List<int>();
    }
}
