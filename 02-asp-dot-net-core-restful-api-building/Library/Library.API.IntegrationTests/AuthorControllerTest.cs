using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Library.API.IntegrationTests
{
    public class AuthorControllerTest: IClassFixture<TestFixture<Library.API.Startup>>
    {
        private readonly HttpClient _client;

        public AuthorControllerTest(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task CreateAuthor_ReturnsUnprocessableEntity_MissingNameValue()
        {
            // Arrange
            var newAuthor = new { Name="", Description="Some description"};
            
            // Act
            var response = await _client.PostAsync(
                "/api/authors", 
                new StringContent(JsonConvert.SerializeObject(newAuthor), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("422", response.StatusCode.ToString());
        }
    }
}
