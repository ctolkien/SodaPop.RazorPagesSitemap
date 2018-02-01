using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using SodaPop.RazorPagesSitemap.Sample;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SodaPop.RazorPagesSitemap.Tests
{
    public class SampleTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public SampleTests()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
               .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact(Skip = "No worky")]
        public async Task EnsureReturnsResponse()
        {
            // Act
            var response = await _client.GetAsync("/sitemap.xml");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            var expectedResult = await System.IO.File.ReadAllTextAsync("response.txt");
            Assert.Equal(expectedResult, responseString, ignoreLineEndingDifferences: true, ignoreWhiteSpaceDifferences: true);
        }
    }
}
