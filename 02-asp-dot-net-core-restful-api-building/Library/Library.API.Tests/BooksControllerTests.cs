using Library.API.Controllers;
using Library.API.Mappers.Profiles;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.API.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<ILibraryService> _mockLibraryService;
        private readonly Mock<ILogger<BooksController>> _mockLogger;

        private readonly BooksController _booksController;

        public BooksControllerTests()
        {
            AutoMapper.Mapper.Initialize(config => config.AddProfile<LibraryProfile>());

            _mockLibraryService = new Mock<ILibraryService>();
            _mockLogger = new Mock<ILogger<BooksController>>();

            _booksController = new BooksController(_mockLibraryService.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }
    }
}
