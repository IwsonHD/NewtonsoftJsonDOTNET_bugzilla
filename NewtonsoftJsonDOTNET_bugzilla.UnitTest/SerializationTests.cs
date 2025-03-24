using NUnit.Framework;

namespace NewtonsoftJsonDOTNET_bugzilla.UnitTest
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void Serialize_AnonymousObject_ReturnsValidJson()
        {
            //Arrange
            var obj = new { Name = "Iwo", Age = 24 };
            
            //Act
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            
            //Assert
            Assert.That(json, Is.EqualTo(@"{""Name"":""Iwo"",""Age"":24}"));
        }

        [Test]
        public void Serialize_WithJsonIgnoreAttribute_IgnoresProperty()
        {
            //Arrange
            var person = new PersonWithJsonIgnoreAge { Name = "Iwo", Age = 24 };
        }
        
    }
}