
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NewtonsoftJsonDOTNET_bugzilla.Models;
using NUnit.Framework;

namespace NewtonsoftJsonDOTNET_bugzilla.UnitTests
{
    [TestFixture]
    public class JsonSerializerSettingsTests
    {
        [Test]
        public void TypeNameHandling_PreservesTypeInfo()
        {
            // Arrange
            var list = new List<object>
            {
                new Dictionary<string, string> { { "key", "value" } },
                new List<int> { 1, 2, 3 }
            };

            // Act
            string json = JsonConvert.SerializeObject(list, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            
            var deserializedList = JsonConvert.DeserializeObject<List<object>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            // Assert
            Assert.That(deserializedList[0], Is.TypeOf<Dictionary<string, string>>());
            Assert.That(deserializedList[1], Is.TypeOf<List<int>>());
        }

        [Test]
        public void MissingMemberHandling_ThrowsWhenStrict()
        {
            // Arrange
            string json = "{\"Name\":\"Iwo\",\"Age\":30,\"ExtraProperty\":\"value\"}";

            // Act & Assert
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<Person>(json, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error
            }));
        }

        [Test]
        public void ReferenceLoopHandling_Ignore_DoesNotThrow()
        {
            // Arrange
            var parent = new ReferenceLoopClass { Name = "Parent" };
            var child = new ReferenceLoopClass { Name = "Child", Parent = parent };
            parent.Child = child;

            // Act & Assert
            Assert.DoesNotThrow(() => JsonConvert.SerializeObject(parent, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }

        private class ReferenceLoopClass
        {
            public string Name { get; set; }
            public ReferenceLoopClass Parent { get; set; }
            public ReferenceLoopClass Child { get; set; }
        }
    }
}