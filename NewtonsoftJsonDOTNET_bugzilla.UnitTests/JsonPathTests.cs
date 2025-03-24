using Newtonsoft.Json.Linq;

namespace NewtonsoftJsonDOTNET_bugzilla.UnitTests;
[TestFixture]
public class JsonPathTests
{

    private JObject _sampleJson;

    [SetUp]
    public void Setup()
    {
        string json = @"{
                'store': {
                    'book': [
                        {
                            'category': 'reference',
                            'author': 'Nigel Rees',
                            'title': 'Sayings of the Century',
                            'price': 8.95
                        },
                        {
                            'category': 'fiction',
                            'author': 'Evelyn Waugh',
                            'title': 'Sword of Honour',
                            'price': 12.99
                        },
                        {
                            'category': 'fiction',
                            'author': 'Herman Melville',
                            'title': 'Moby Dick',
                            'price': 8.99
                        }
                    ],
                    'bicycle': {
                        'color': 'red',
                        'price': 19.95
                    }
                }
            }";
            
        _sampleJson = JObject.Parse(json);
    }
    
    [Test]
    public void SelectToken_GetsSingleValue()
    {
        // Act
        var bicycle = _sampleJson.SelectToken("$.store.bicycle");
        var color = _sampleJson.SelectToken("$.store.bicycle.color").ToString();
            
        // Assert
        Assert.That(bicycle, Is.Not.Null);
        Assert.That(color, Is.EqualTo("red"));
    }

    [Test]
    public void SelectTokens_GetsMultipleValues()
    {
        // Act
        var authors = _sampleJson.SelectTokens("$..author").Select(t => t.ToString()).ToList();
        var prices = _sampleJson.SelectTokens("$..price").Select(t => (double)t).ToList();
            
        // Assert
        Assert.That(authors, Has.Count.EqualTo(3));
        Assert.That(authors, Does.Contain("Nigel Rees"));
        Assert.That(authors, Does.Contain("Evelyn Waugh"));
        Assert.That(authors, Does.Contain("Herman Melville"));
            
        Assert.That(prices, Has.Count.EqualTo(4));
        Assert.That(prices, Does.Contain(19.95));
    }
    
}