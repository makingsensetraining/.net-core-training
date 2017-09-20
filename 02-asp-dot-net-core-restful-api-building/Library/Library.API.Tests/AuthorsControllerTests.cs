using Library.API.Controllers;
using Library.API.Models;
using Library.API.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using Library.API.Entities;
using Library.API.Mappers.Profiles;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Library.API.Helpers;
using Microsoft.AspNetCore.Http;

namespace Library.API.Tests
{
    public class AuthorsControllerTests
    {
        private readonly Mock<ILibraryService> _mockLibraryService;
        private readonly Mock<IUrlHelper> _mockUrlHelper;

        private readonly AuthorsController _authorsController;

        public AuthorsControllerTests()
        {
            AutoMapper.Mapper.Initialize(config => config.AddProfile<LibraryProfile>());

            _mockLibraryService = new Mock<ILibraryService>();
            _mockUrlHelper = new Mock<IUrlHelper>();

            _authorsController = new AuthorsController(_mockLibraryService.Object, _mockUrlHelper.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public void GetAuthors_ReturnsAListOfAuthorDto()
        {
            // Arrange
            var authorResourceParameters = new AuthorResourceParameters();
            _mockLibraryService.Setup(repo => repo.GetAuthors(authorResourceParameters)).Returns(GetAuthors());

            // Act
            var result = _authorsController.GetAuthors(authorResourceParameters);

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
            var id = new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}");
            _mockLibraryService.Setup(repo => repo.GetAuthor(id)).Returns(null as Author);

            // Act
            var result = _authorsController.GetAuthor(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAuthor_ReturnsAuthorDto()
        {
            // Arrange
            var id = new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}");
            _mockLibraryService.Setup(repo => repo.GetAuthor(id)).Returns(GetAuthor1());

            // Act
            var result = _authorsController.GetAuthor(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<AuthorDto>(
                okResult.Value);

            Assert.Equal(new Guid("{7AEA84A3-42A2-4D6C-99B6-1839AACBDFF2}"), model.Id);
            Assert.Equal("Thriller", model.Genre);
            Assert.Equal("Author 1 The First", model.Name);
            Assert.Equal(56, model.Age);
        }

        [Fact]
        public void CreateAuthor_ReturnsBadRequest_WhenAuthorIsNull()
        {
            // Arrange

            // Act
            var result = _authorsController.CreateAuthor(null);

            //Result
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void CreateAuthor_ReturnsUnprocessableEntity_WhenInvalidModelState()
        {
            // Arrange
            _authorsController.ModelState.AddModelError("FirstName", "Required");

            // Act
            var result = _authorsController.CreateAuthor(new AuthorForCreationDto());

            //Result
            Assert.IsType<UnprocessableEntityObjectResult>(result);
        }

        [Fact]
        public void CreateAuthor_ReturnsAuthorDtoAndRoute_WhenCreated()
        {
            // Arrange
            _mockLibraryService.Setup(x=>x.AddAuthor(It.IsAny<Author>())).Returns(true);

            // Act
            var result = _authorsController.CreateAuthor(new AuthorForCreationDto());

            //Result
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<AuthorDto>(createdResult.Value);
            Assert.Equal("GetAuthorById", createdResult.RouteName);
        }

        [Fact]
        public void DeleteAuthor_NotFound()
        {
            //Arrange
            var id = Guid.NewGuid();
            _mockLibraryService.Setup(x => x.GetAuthor(id)).Returns(null as Author);

            //Act
            var result = _authorsController.RemoveAuthor(id);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAuthor_Ok()
        {
            //Arrange
            var id = Guid.NewGuid();
            var author = new Author();
            _mockLibraryService.Setup(x => x.GetAuthor(id)).Returns(author);
            _mockLibraryService.Setup(x => x.DeleteAuthor(author)).Returns(true);

            //Act
            var result = _authorsController.RemoveAuthor(id);

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

        private PagedList<Author> GetAuthors(int pageNumber = 1, int pageSize = 10)
        {
            var items = new List<Author>
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
            
            return new PagedList<Author>(items, 8, pageNumber, pageSize); 
        }
    }
}
