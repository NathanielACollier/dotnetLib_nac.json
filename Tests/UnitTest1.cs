using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Tests;

[TestClass]
public class UnitTest1
{


    [TestMethod]
    public void StringOnlyTest()
    {
        string result = nac.json.utility.DeserializeToDictionaryList(@"
            ""Hello World""
        ") as string;

        Assert.IsTrue(!string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void ListOneLevel()
    {
        dynamic result = nac.json.utility.DeserializeToDictionaryList(@"
            [
                {""A"": 1},
                {""A"": 2},
                {""A"": 3}
            ]
        ");

        Assert.IsTrue(result[0]["A"] == 1);
        Assert.IsTrue(result[1]["A"] == 2);
    }

    [TestMethod]
    public void DictionaryDeserializeSimple()
    {
        var result = nac.json.utility.DeserializeToDictionaryList(@"
            {
                ""a"": 1,
                ""b"": 2
            }
        ") as Dictionary<string, object>;

        Assert.IsTrue(result["a"] as int? == 1);
        Assert.IsTrue(result["b"] as int? == 2);
    }


    [TestMethod]
    public void ContentFileTest1_Expando()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\Test1.json");

        dynamic i = nac.json.utility.DeserializeToDictionaryList(jsonText, nac.json.model.ObjectFormat.Expando);

        Assert.IsTrue(i.d1 is DateTime d1 && d1.Year == 2023, "Month/Day/Year not valid datetime");
        Assert.IsTrue(i.d2 is DateTime d2 && d2.Year == 2010, "Year-Month-Day not valid datetime");
        Assert.IsTrue(i.items[0].a == 123, "Items not valid list");
    }


    [TestMethod]
    public void ContentFileTest1_Flexpando_AllowsCaseCaseInsensitive()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\Test1.json");

        dynamic i = nac.json.utility.DeserializeToDictionaryList(jsonText, nac.json.model.ObjectFormat.Flexpando);

        Assert.IsTrue(i.d1 is DateTime d1 && d1.Year == 2023, "Month/Day/Year not valid datetime");
        Assert.IsTrue(i.D2 is DateTime d2 && d2.Year == 2010, "Year-Month-Day not valid datetime");
        Assert.IsTrue(i.Items[0].a == 123, "Items not valid list");
    }

    [TestMethod]
    public void ContentFileTest1_DictionaryTest()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\Test1.json");

        dynamic i = nac.json.utility.DeserializeToDictionaryList(jsonText);

        Assert.IsTrue(i["d1"] is DateTime d1 && d1.Year == 2023, "Moth/Day/Year not valid datetime");
        Assert.IsTrue(i["d2"] is DateTime d2 && d2.Year == 2010, "Year-Month-Day not valid datetime");
        Assert.IsTrue(i["items"][0]["a"] == 123, "Items not valid list");
        // test case sensitivity
        Assert.IsTrue(i["ITEMS"][0]["a"] == 123, "Items not valid list");
    }


    [TestMethod]
    public void DeserializePartial_ClassResultHasLessPropertiesThanJSONAndAlsoUsesFieldsInsteadofProperties()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\alphabet1.json");

        var full = nac.json.utility.DeserializeJSON<model.Alphabet>(jsonText);
        var half = nac.json.utility.DeserializeJSON<model.AlphabetHalf>(jsonText);

        Assert.IsTrue(!string.IsNullOrEmpty(full.A));
        Assert.IsTrue(!string.IsNullOrEmpty(half.A));
        Assert.IsTrue(!string.IsNullOrEmpty(half.api_version));
        Assert.IsTrue(!string.IsNullOrEmpty(half.B));
        Assert.IsTrue(!string.IsNullOrEmpty(half.C));
    }


    [TestMethod]
    public void SerializeAndDeserializeFiltersList()
    {

        var originalFilters = new[]
        {
            new model.TableFilterEntry
            {
                ColumnName = "InvoiceDate",
                Value = DateTime.Now,
                Operation = ">="
            },
            new model.TableFilterEntry
            {
                ColumnName = "PONumber",
                Value = 20364,
                Operation = "equals"
            },
            new model.TableFilterEntry
            {
                ColumnName = "VendorNumber",
                Value = "50099",
                Operation = "equals"
            }
        };

        string jsonText = System.Text.Json.JsonSerializer.Serialize(originalFilters);

        var filters = System.Text.Json.JsonSerializer.Deserialize<model.TableFilterEntry[]>(jsonText);

        Assert.IsTrue(filters.Length > 0);
        Assert.IsTrue(filters[0].Value is DateTime);
        Assert.IsTrue(filters[1].Value is long);
        Assert.IsTrue(filters[2].Value is string);
    }



    [TestMethod]
    public void DeserializeInvoiceWithMultipleConverters()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\InvoiceList.json");

        var options = new System.Text.Json.JsonSerializerOptions
        {
            Converters =
            {
                new nac.json.converters.BooleanJSONConverter(),
                new nac.json.converters.NullableDateTimeJSONConverter()
            }
        };

        var invoices =
            System.Text.Json.JsonSerializer.Deserialize<List<model.Invoice>>(json: jsonText, options: options);

        Assert.IsTrue(invoices.Count > 0);

        // test the single/array converter
        Assert.IsTrue(invoices[0].lines.Count == 1);
        Assert.IsTrue(invoices[1].lines.Count > 1);
        Assert.IsTrue(invoices[2].lines.Count == 0);

        // test single array converter on string array
        Assert.IsTrue(invoices[0].target.Count == 1);
        Assert.IsTrue(invoices[1].target.Count > 1);
        Assert.IsTrue(invoices[2].target.Count == 0);

        // test single array converter on int array
        Assert.IsTrue(invoices[0].num.Count == 1);
        Assert.IsTrue(invoices[1].num.Count > 1);
        Assert.IsTrue(invoices[2].num.Count == 0);

        // Test single array converter invoked with factory
        Assert.IsTrue(invoices[0].numFactory.Count == 1);
        Assert.IsTrue(invoices[1].numFactory.Count > 1);
        Assert.IsTrue(invoices[2].numFactory.Count == 0);
    }




    [TestMethod]
    public void DeserializeModelWithListDictionary()
    {
        string jsonText = System.IO.File.ReadAllText(@".\content\GenericType1.json");

        var obj = System.Text.Json.JsonSerializer.Deserialize<model.GenericType1>(json: jsonText);

        Assert.IsTrue(obj.Items.Count > 0);
    }



}