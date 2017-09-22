using Library.API.Controllers;
using Library.API.Entities;
using Library.API.Mappers.Profiles;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Library.API.Tests
{
    public class AuthorCollectionsControllerTests
    {
        private readonly Mock<ILibraryService> _mockLibraryService;

        private readonly AuthorCollectionsController _authorCollectionsController;

        public AuthorCollectionsControllerTests()
        {
            AutoMapper.Mapper.Initialize(config => config.AddProfile<LibraryProfile>());

            _mockLibraryService = new Mock<ILibraryService>();

            _authorCollectionsController = new AuthorCollectionsController(_mockLibraryService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetAuthorCollection_ReturnsBadRequest_NullCollectionAsync()
        {
            //Arrange

            //Act
            var result = await _authorCollectionsController.GetAuthorCollection(null);

            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetAuthorCollection_ReturnsNotFound_AuthorNotFoundAsync()
        {
            //Arrange
            var ids = new List<Guid> {
                new Guid("{72D457EF-D7D8-4AE9-8FE8-AF4FBE6D253F}"),
                new Guid("{34843347-C7D2-499D-8304-3120E36311A5}")
            };

            var authorList = new List<Author> { new Author () };
            _mockLibraryService.Setup(x=>x.GetAuthorsAsync(ids)).ReturnsAsync(authorList);

            //Act
            var result = await _authorCollectionsController.GetAuthorCollection(ids);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAuthorCollection_ReturnsAuthorsDtoListAsync()
        {
            //Arrange
            var ids = new List<Guid> {
                new Guid("{72D457EF-D7D8-4AE9-8FE8-AF4FBE6D253F}"),
                new Guid("{34843347-C7D2-499D-8304-3120E36311A5}")
            };

            var authorList = new List<Author> { new Author(), new Author() };
            _mockLibraryService.Setup(x => x.GetAuthorsAsync(ids)).ReturnsAsync(authorList);

            //Act
            var result = await _authorCollectionsController.GetAuthorCollection(ids);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IList<AuthorDto>>(
                okResult.Value);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task AddAuthorCollection_ReturnsAuthorsDtoAndRoute_WhenCreatedAsync()
        {
            // Arrange
            _mockLibraryService.Setup(x => x.AddAuthorAsync(It.IsAny<Author>())).Returns(Task.CompletedTask);
            var authorList = new List<AuthorForCreationDto> { new AuthorForCreationDto(), new AuthorForCreationDto() };

            // Act
            var result = await _authorCollectionsController.AddAuthorCollection(authorList);

            //Result
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
            var model = Assert.IsAssignableFrom<List<AuthorDto>>(createdResult.Value);
            Assert.Equal(2, model.Count);
            Assert.Equal("GetAuthorCollection", createdResult.RouteName);
        }

    }
}
