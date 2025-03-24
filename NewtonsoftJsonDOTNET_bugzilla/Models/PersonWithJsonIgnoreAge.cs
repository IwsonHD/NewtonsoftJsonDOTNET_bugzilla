using Newtonsoft.Json;

namespace NewtonsoftJsonDOTNET_bugzilla.Models;

public class PersonWithJsonIgnoreAge
{
    public string Name;

    [JsonIgnore] public int Age;
}