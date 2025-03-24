using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewtonsoftJsonDOTNET_bugzilla.Models;

namespace NewtonsoftJsonDOTNET_bugzilla.UnitTests;

[TestFixture]
public class DesrializationTests
{
    [Test]
    public void DeserializeJson_IntoTypedObject_ReturnsValidObject()
    {
        // Arrange
        string json = "{\"Name\":\"Adam\",\"Age\":21}";
            
        // Act
        var person = JsonConvert.DeserializeObject<Person>(json);
            
        // Assert
        Assert.That(person, Is.Not.Null);
        Assert.That(person.Name, Is.EqualTo("Adam"));
        Assert.That(person.Age, Is.EqualTo(21));
    }
    [Test]
    public void DeserializeVeryLargeInteger_ThrowsJsonReaderException()
    {
        // Arrange
        string json = "{\"Number\": 99999999999999999999999999999999999999999999}";
            
        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<IntegerWrapper>(json));
    }
    
    [Test]
    public void DeserializeVeryLargeDecimal_ThrowsJsonReaderException()
    {
        // Arrange
        string json = "{\"Number\": 99999999999999999999999999999}";
            
        // Act & Assert
        Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<DecimalWrapper>(json));
    }
    
    [Test]
    public void DeserializeJson_WithJObject_AllowsDynamicAccess()
    {
        // Arrange
        string json = "{\"person\":{\"name\":\"Adam\",\"age\":21}}";
            
        // Act
        JObject parsed = JObject.Parse(json);
        string name = parsed["person"]["name"].ToString();
        int age = parsed["person"]["age"].Value<int>();
            
        // Assert
        Assert.That(name, Is.EqualTo("Adam"));
        Assert.That(age, Is.EqualTo(21));
    }
    
}