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

    [HttpGet("deserialize-into-record-with-default-value")]
    public IActionResult DeserializeIntoRecordWithDefaultValue()
    {
        var json = @"{
            'Age': 21        
        }";
        
        
        var deserializedJson = Newtonsoft.Json.JsonConvert.DeserializeObject<DefaultNameRecordPerson>(json);

        return Ok(deserializedJson);
    }

    [HttpGet("benchmarks/serialize/smallObjects")]
    public IActionResult SerializeSmallObjects([FromQuery]int iterations = 100)
    {
        var smallObjects = Enumerable.Range(0, iterations).Select(i => new
        {
            Name = "Iwo",
            Age = i
        }).ToList();
    
        var elapsedTimes = new List<double>();
        double ticksPerMicrosecond = (double)System.Diagnostics.Stopwatch.Frequency / 1_000_000;

        // Warm-up
        JsonConvert.SerializeObject(smallObjects);

        var stopwatch = new System.Diagnostics.Stopwatch();
        // Perform actual benchmarking (10 iterations)
        for (int i = 0; i < 10; i++)
        {
            stopwatch.Restart(); 
            JsonConvert.SerializeObject(smallObjects);
            stopwatch.Stop();
            double microseconds = stopwatch.ElapsedTicks / ticksPerMicrosecond;
            elapsedTimes.Add(microseconds);
        }

        return Ok(new {
            AverageMicroseconds = elapsedTimes.Average(),
            MinMicroseconds = elapsedTimes.Min(),
            MaxMicroseconds = elapsedTimes.Max(),
            AverageMilliseconds = elapsedTimes.Average() / 1000,
            AllTimesMicroseconds = elapsedTimes
        });
    }

   [HttpGet("benchmarks/serialize/largeObjects")]
public IActionResult SerializeLargeObjects([FromQuery] int iterations = 1000)
{
    var largeObjects = Enumerable.Range(0, iterations).Select(i => new
    {
        Id = i,
        Name = $"Item {i}",
        Description = $"This is a longer description for item {i} with more details to increase the JSON size significantly",
        CreatedAt = DateTime.Now.AddDays(-i),
        UpdatedAt = DateTime.Now.AddHours(-i),
        Properties = new
        {
            Color = i % 3 == 0 ? "Red" : i % 3 == 1 ? "Green" : "Blue",
            Size = i % 5,
            Tags = new[] { "Tag1", "Tag2", "Tag3", "Tag4" }.Take(i % 4 + 1).ToArray()
        },
        Values = Enumerable.Range(1, 10).Select(j => j * i).ToArray()
    }).ToList();
    
    var elapsedTimes = new List<double>();
    double ticksPerMicrosecond = (double)System.Diagnostics.Stopwatch.Frequency / 1_000_000;

    // Warm-up
    JsonConvert.SerializeObject(largeObjects);

    var stopwatch = new System.Diagnostics.Stopwatch();
    // Perform actual benchmarking (10 iterations)
    for (int i = 0; i < 10; i++)
    {
        stopwatch.Restart(); 
        string json = JsonConvert.SerializeObject(largeObjects);
        stopwatch.Stop();
        double microseconds = stopwatch.ElapsedTicks / ticksPerMicrosecond;
        elapsedTimes.Add(microseconds);
    }

    return Ok(new {
        AverageMicroseconds = elapsedTimes.Average(),
        MinMicroseconds = elapsedTimes.Min(),
        MaxMicroseconds = elapsedTimes.Max(),
        AverageMilliseconds = elapsedTimes.Average() / 1000,
        AllTimesMicroseconds = elapsedTimes
    });
}

[HttpGet("benchmarks/deserialize/smallObjects")]
public IActionResult DeserializeSmallObjects([FromQuery] int iterations = 100)
{
    var smallObjects = Enumerable.Range(0, iterations).Select(i => new
    {
        Name = "Iwo",
        Age = i
    }).ToList();
    
    string json = JsonConvert.SerializeObject(smallObjects);
    
    var elapsedTimes = new List<double>();
    double ticksPerMicrosecond = (double)System.Diagnostics.Stopwatch.Frequency / 1_000_000;

    // Warm-up
    JsonConvert.DeserializeObject<List<dynamic>>(json);

    var stopwatch = new System.Diagnostics.Stopwatch();
    // Perform actual benchmarking (10 iterations)
    for (int i = 0; i < 10; i++)
    {
        stopwatch.Restart(); 
        JsonConvert.DeserializeObject<List<dynamic>>(json);
        stopwatch.Stop();
        double microseconds = stopwatch.ElapsedTicks / ticksPerMicrosecond;
        elapsedTimes.Add(microseconds);
    }

    return Ok(new {
        AverageMicroseconds = elapsedTimes.Average(),
        MinMicroseconds = elapsedTimes.Min(),
        MaxMicroseconds = elapsedTimes.Max(),
        AverageMilliseconds = elapsedTimes.Average() / 1000,
        AllTimesMicroseconds = elapsedTimes
    });
}

[HttpGet("benchmarks/deserialize/largeObjects")]
public IActionResult DeserializeLargeObjects([FromQuery] int iterations = 1000)
{
    var largeObjects = Enumerable.Range(0, iterations).Select(i => new
    {
        Id = i,
        Name = $"Item {i}",
        Description = $"This is a longer description for item {i} with more details to increase the JSON size significantly",
        CreatedAt = DateTime.Now.AddDays(-i),
        UpdatedAt = DateTime.Now.AddHours(-i),
        Properties = new
        {
            Color = i % 3 == 0 ? "Red" : i % 3 == 1 ? "Green" : "Blue",
            Size = i % 5,
            Tags = new[] { "Tag1", "Tag2", "Tag3", "Tag4" }.Take(i % 4 + 1).ToArray()
        },
        Values = Enumerable.Range(1, 10).Select(j => j * i).ToArray()
    }).ToList();
    
    string json = JsonConvert.SerializeObject(largeObjects);
    
    var elapsedTimes = new List<double>();
    double ticksPerMicrosecond = (double)System.Diagnostics.Stopwatch.Frequency / 1_000_000;

    // Warm-up
    JsonConvert.DeserializeObject<List<dynamic>>(json);

    var stopwatch = new System.Diagnostics.Stopwatch();
    // Perform actual benchmarking (10 iterations)
    for (int i = 0; i < 10; i++)
    {
        stopwatch.Restart(); 
        JsonConvert.DeserializeObject<List<dynamic>>(json);
        stopwatch.Stop();
        double microseconds = stopwatch.ElapsedTicks / ticksPerMicrosecond;
        elapsedTimes.Add(microseconds);
    }

    return Ok(new {
        AverageMicroseconds = elapsedTimes.Average(),
        MinMicroseconds = elapsedTimes.Min(),
        MaxMicroseconds = elapsedTimes.Max(),
        AverageMilliseconds = elapsedTimes.Average() / 1000,
        AllTimesMicroseconds = elapsedTimes
    });
}
    
}