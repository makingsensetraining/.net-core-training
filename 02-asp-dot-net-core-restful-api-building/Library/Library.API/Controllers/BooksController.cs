using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController: Controller
    {
        private readonly ILibraryService _libraryService;

        public BooksController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryService.AuthorExists(authorId))
                return NotFound();

            var booksFromRepo = _libraryService.GetBooksForAuthor(authorId);

            var books = Mapper.Map<IList<BookDto>>(booksFromRepo);

            return Ok(books);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryService.AuthorExists(authorId))
                return NotFound();

            var bookFromRepo = _libraryService.GetBookForAuthor(authorId,id);

            if (bookFromRepo == null)
                return NotFound();

            var book = Mapper.Map<BookDto>(bookFromRepo);

            return Ok(book);
        }


    }
}
