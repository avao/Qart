using NUnit.Framework;
using Qart.Testing.Framework;

namespace Qart.Core.Tests.Text
{
    public class UrlParametersExtractionTests
    {
        [TestCase("weather?q=London,uk&appid=2b1fd2d7f77ccf1b7de9b441571b39b8", "weather", "q","London,uk")]
        public void UrlBasedParameterExtraction_Parse(string query, string key, string tokenKey, string tokenResult)
        {
            ResolvableItemDescription parsed = UrlBasedParameterExtraction.Parse(query);
            ResolvableItemDescription parsed2 = UrlBasedParameterExtraction.Parse(query);

            Assert.That(parsed.Name, Is.EqualTo(key)); 
            Assert.That(parsed.Parameters[tokenKey], Is.EqualTo(tokenResult));

            Assert.That(parsed.Parameters[tokenKey], Is.EqualTo(parsed2.Parameters[tokenKey]));
        }
    }
}
