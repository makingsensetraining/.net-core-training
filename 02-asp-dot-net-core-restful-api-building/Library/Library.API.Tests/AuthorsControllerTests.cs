using Library.API.Controllers;
using Library.API.Models;
using Library.API.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Library.API.Entities;
using Library.API.Profiles;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Library.API.Helpers;

namespace Library.API.Tests
{
    public class AuthorsControllerTests
    {
        public AuthorsControllerTests()
        {
            AutoMapper.Mapper.Initialize(config => config.AddProfile<LibraryProfile>());
        }

        [Fact]
        public void GetAuthors_ReturnsAListOfAuthorDto()
        {
            // Arrange
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(repo => repo.GetAuthors()).Returns(GetAuthors());
            var controller = new AuthorsController(mockRepo.Object);

            // Act
            var result = controller.GetAuthors();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IList<AuthorDto>>(
                okResult.Value);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void GetAuthor_NotFound()
        {
            // Arrange
            Author nullAuthor = null;
            var id = new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}");
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(repo => repo.GetAuthor(id)).Returns(nullAuthor);
            var controller = new AuthorsController(mockRepo.Object);

            // Act
            var result = controller.GetAuthor(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAuthor_ReturnsAuthorDto()
        {
            // Arrange
            var id = new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}");
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(repo => repo.GetAuthor(id)).Returns(GetAuthor1());
            var controller = new AuthorsController(mockRepo.Object);

            // Act
            var result = controller.GetAuthor(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<AuthorDto>(
                okResult.Value);
            Assert.Equal(56, model.Age);
        }

        [Fact]
        public void CreateAuthor_ReturnsBadRequest_WhenAuthorIsNull()
        {
            // Arrange
            var mockRepo = new Mock<ILibraryService>();
            var controller = new AuthorsController(mockRepo.Object);

            // Act
            var result = controller.CreateAuthor(null);

            //Result
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CreateAuthor_ReturnsUnprocessableEntity_WhenInvalidModelState()
        {
            // Arrange
            var mockRepo = new Mock<ILibraryService>();
            var controller = new AuthorsController(mockRepo.Object);
            controller.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = controller.CreateAuthor(new AuthorForCreationDto());

            //Result
            Assert.IsType<UnprocessableEntityObjectResult>(result);
        }

        [Fact]
        public void CreateAuthor_ReturnsAuthorDtoAndRoute_WhenCreated()
        {
            // Arrange
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(x=>x.AddAuthor(new Author()));
            mockRepo.Setup(x => x.Save()).Returns(true);
            var controller = new AuthorsController(mockRepo.Object);

            // Act
            var result = controller.CreateAuthor(new AuthorForCreationDto());

            //Result
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<AuthorDto>(createdResult.Value);
            Assert.NotNull(model.Id);
            Assert.Equal("GetAuthorById", createdResult.RouteName);
        }

        [Fact]
        public void DeleteAuthor_NotFound()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(x => x.GetAuthor(id)).Returns(null as Author);
            var controller = new AuthorsController(mockRepo.Object);

            //Act
            var result = controller.RemoveAuthor(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAuthor_Ok()
        {
            //Arrange
            var id = Guid.NewGuid();
            var mockRepo = new Mock<ILibraryService>();
            mockRepo.Setup(x => x.GetAuthor(id)).Returns(new Author());
            mockRepo.Setup(x => x.DeleteAuthor(new Author()));
            mockRepo.Setup(x => x.Save()).Returns(true);
            var controller = new AuthorsController(mockRepo.Object);

            //Act
            var result = controller.RemoveAuthor(id);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        private Author GetAuthor1()
        {
            return new Author
            {
                Id = new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}"),
                Genre = "Thriller",
                FirstName = "Author 1",
                LastName = "The First",
                DateOfBirth = DateTime.Now.AddYears(-56)
            };
        }

        private IList<Author> GetAuthors()
        {
            return new List<Author>
            {
                GetAuthor1(),
                new Author
                {
                    Id = new Guid("{FB223F91-029D-4A1B-B35D-0FF02ABFDB1F}"),
                    Genre = "Drama",
                    FirstName = "Author 2",
                    LastName = "The second",
                    DateOfBirth = DateTime.Now.AddYears(-37)
                }
            };
        }
    }
}
