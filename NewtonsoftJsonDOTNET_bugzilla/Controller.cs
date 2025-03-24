using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewtonsoftJsonDOTNET_bugzilla.Models;

namespace NewtonsoftJsonDOTNET_bugzilla;

[ApiController]
[Route("api/tests")]
public class Controller : ControllerBase
{
    [HttpGet("serialize")]
    public IActionResult SerializeObject()
    {
        var obejct = new { Name = "Iwo", Age = 30 };
        string json = JsonConvert.SerializeObject(obejct);

        return Ok(json);
    }

    [HttpGet("deseralize")]
    public IActionResult DeserializeObject()
    {
        string json = @"{
            'Name': 'Adam',
            'Age': 21        
        }";

        var person = JsonConvert.DeserializeObject<Person>(json);
        
        return Ok(person);
    }

    [HttpGet("jobject")]
    public IActionResult ParseJsonDynamically()
    {
        string json = @"{
                    'person': {
                        'name': 'Adam',
                        'age': 21
                }
            }";
        JObject parsed = JObject.Parse(json);

        string name = parsed["person"]["name"].ToString();

        int age = parsed["person"]["age"].Value<int>();

        return Ok(new { name = name, age = age });
    }

    [HttpGet("ignore-null")]
    public IActionResult IgnoreNull()
    {
        var person = new PersonWithNullableAge { Name = "Iwo", Age = null };

        var json = JsonConvert.SerializeObject(person, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        return Ok(json);
    }   
    
    [HttpGet("include-null")]
    public IActionResult IncludeNull()
    {
        var person = new PersonWithNullableAge() { Name = "Iwo", Age = null };

        var json = JsonConvert.SerializeObject(person, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include
        });

        return Ok(json);
    }

    [HttpGet("serialize-into-ISOdate")]
    public IActionResult SerializeIntoISODate()
    {
        DateTime now = new DateTime(2025, 3, 23, 15, 30, 45);

        string json = JsonConvert.SerializeObject(new { time = now }, new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat
        });

        return Ok(json);
    }    
    
    [HttpGet("serialize-into-custom-date")]
    public IActionResult SerializeIntoCustomDate()
    {
        DateTime now = new DateTime(2025, 3, 23, 15, 30, 45);

        string json = JsonConvert.SerializeObject(new { time = now }, new JsonSerializerSettings
        {
            DateFormatString = "dd-MM-yyyy"
        });

        return Ok(json);
    }

    [HttpGet("dictionary")]
    public IActionResult DictionaryToJson()
    {
        var dictionary = new Dictionary<string, int>
        {
            {"Iwo", 100 },
            {"Adam", 20 }  
        };

        string json = JsonConvert.SerializeObject(dictionary);
        return Ok(json);
    }

    [HttpGet("json-ignore-adnotation")]
    public IActionResult JsonIgnoreAdnotation()
    {
        var jsonIgnorePerson = new PersonWithJsonIgnoreAge()
        {
            Name = "Adam",
            Age = 20
        };

        var json = JsonConvert.SerializeObject(jsonIgnorePerson);

        return Ok(json);
    }

    [HttpGet("float")]
    public IActionResult SerializeFloat()
    {
        var floatStructure = new
        {
            floatNum1 = 3.4124,
            floatNum2 = 5125123414124.1342123
        };

        var json = JsonConvert.SerializeObject(floatStructure);
        return Ok(json);
    }

    [HttpGet("tooLargeDecimal")]
    public IActionResult TooLargerDecimal()
    {
        var number = JsonConvert.DeserializeObject<DecimalWrapper>("{\"Number\": 99999999999999999999999999999}");
        
        return Ok(number);
    }

    [HttpGet("leadingZeroDecimal")]
    public IActionResult LeadingZeroDecimal()
    {
        List<decimal> properDeserializationOutput = new List<decimal>();
        List<string> exceptions = new List<string>();

        var jsonExmaples = new List<string> { "{ \"Number\": 01234 }", "{ \"Number\": 0800 }" };

        foreach(var jsonExample in jsonExmaples)
        {
            try
            {
                properDeserializationOutput.Add(JsonConvert.DeserializeObject<DecimalWrapper>(jsonExample).Number);
            }
            catch (Exception err)
            {
                exceptions.Add(err.Message);
            }
        }

        return Ok(new { deserializedNumbers = properDeserializationOutput, exceptions = exceptions});
    }
}