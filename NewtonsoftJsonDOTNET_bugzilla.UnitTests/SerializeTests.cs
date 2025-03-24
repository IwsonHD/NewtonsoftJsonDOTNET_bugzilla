using Newtonsoft.Json;
using NewtonsoftJsonDOTNET_bugzilla.Models;

namespace NewtonsoftJsonDOTNET_bugzilla.UnitTests;


[TestFixture]
public class SerializeTests
{
    [Test]
    public void Serialize_AnonymousObject_ReturnsValidJson()
    {
        //Arrange
        var obj = new { Name = "Iwo", Age = 24 };
        
        //Act
        var json = JsonConvert.SerializeObject(obj);
        
        //Assert
        Assert.That(json, Is.EqualTo(@"{""Name"":""Iwo"",""Age"":24}"));
    }
    
    [Test]
    public void Serialize_WithJsonIgnoreAttribute_IgnoresProperty()
    {
        //Arrange
        var person = new PersonWithJsonIgnoreAge { Name = "Iwo", Age = 24 };
        
        //Act
        var json = JsonConvert.SerializeObject(person);
        
        //Assert
        Assert.That(json, Is.EqualTo(@"{""Name"":""Iwo""}"));
    }
    
    [Test]
    public void Serialize_DefaultAgeRecordPerson_ReturnsValidJson()
    {
        //Arrange
        var person = new DefaultNameRecordPerson(24);
        
        //Act
        var json = JsonConvert.SerializeObject(person);
        
        //Assert
        Assert.That(json, Is.EqualTo(@"{""Age"":24,""Name"":""iwo""}"));
    }

    [Test]
    public void Serialize_WithNullValue_IgnoresNullValueWhenSpeciied()
    {
        // Arrange
        var person = new PersonWithNullableAge { Name = "Iwo", Age = null };
            
        // Act
        string json = JsonConvert.SerializeObject(person, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
            
        // Assert
        Assert.That(json, Is.EqualTo("{\"Name\":\"Iwo\"}"));
    }
}