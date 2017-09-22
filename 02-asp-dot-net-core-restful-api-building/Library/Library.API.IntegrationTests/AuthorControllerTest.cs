using Library.API.Models;
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
               
        [Theory]
        [MemberData(nameof(GetInvalidCreationData))]
        public async Task CreateAuthor_ReturnsUnprocessableEntity_InvalidModel(string firstName, string lastName, string genre, DateTime? dateOfBirth)
        {
            // Arrange
            var newAuthor = new { FirstName = firstName, LastName= lastName, DateOfBirth = dateOfBirth, Genre = genre };
            
            // Act
            var response = await _client.PostAsync(
                "/api/authors", 
                new StringContent(JsonConvert.SerializeObject(newAuthor), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal("422", response.StatusCode.ToString());
        }

        [Fact]
        public async Task CreateAuthor_Returns201Created()
        {
            // Arrange
            var newAuthor = new {
                FirstName ="First Name",
                LastName = "Last Name",
                Genre = "Thriller",
                DateOfBirth = DateTime.UtcNow.AddYears(-49)
            };

            // Act
            var response = await _client.PostAsync(
                "/api/authors",
                new StringContent(JsonConvert.SerializeObject(newAuthor), Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var author = JsonConvert.DeserializeObject<AuthorDto>(await response.Content.ReadAsStringAsync());
            Assert.NotEqual(Guid.Empty, author.Id);
            Assert.Equal("Thriller", author.Genre);
            Assert.Equal("First Name Last Name", author.Name);
            Assert.Equal(49, author.Age);

        }

        private static IEnumerable<object[]> GetInvalidCreationData()
        {
            yield return new object[] { "", "Last Name", "Thriller", DateTime.UtcNow.AddYears(-49) };
            yield return new object[] {
                "This is a too large value for this field. Max value is 50.",
                "Last Name",
                "Thriller",
                DateTime.UtcNow.AddYears(-49)
            };
            yield return new object[] { "First Name", "", "Thriller", DateTime.UtcNow.AddYears(-49) };
            yield return new object[] {
                "First Name",
                "This is a too large value for this field. Max value is 50.",
                "Thriller",
                DateTime.UtcNow.AddYears(-49) };
            yield return new object[] { "First Name", "Last Name", "", DateTime.UtcNow.AddYears(-49) };
            yield return new object[] {
                "First Name",
                "Last Name",
                "This is a too large value for this field. Max value is 50.",
                DateTime.UtcNow.AddYears(-49) };
            yield return new object[] { "First Name", "Last Name", "Thriller", null };
        }
    }
}
