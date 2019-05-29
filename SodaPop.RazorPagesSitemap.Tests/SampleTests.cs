using Microsoft.AspNetCore.Mvc.Testing;
using SodaPop.RazorPagesSitemap.Sample;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace SodaPop.RazorPagesSitemap.Tests
{
    public class SampleTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public SampleTests(WebApplicationFactory<Startup> webApplicationFactory)
        {
            // Arrange
            _client = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task EnsureReturnsResponse()
        {
            // Arrange
            var sampleResponse = await System.IO.File.ReadAllTextAsync("response.txt");
            var sampleXml = new XmlDocument();
            sampleXml.LoadXml(sampleResponse);

            // Act
            var response = await _client.GetAsync("/sitemap.xml");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var responseXml = new XmlDocument();
            responseXml.LoadXml(responseString);

            var sampleLocationNodes = sampleXml.GetElementsByTagName("loc");
            var responseLocationNodes = responseXml.GetElementsByTagName("loc");

            // Assert


            Assert.Equal(sampleLocationNodes.Count, responseLocationNodes.Count);
        }
    }
}
