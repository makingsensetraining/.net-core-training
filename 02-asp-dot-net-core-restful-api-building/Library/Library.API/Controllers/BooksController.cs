using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BooksController> _logger;

        public BooksController(ILibraryService libraryService, ILogger<BooksController> logger)
        {
            _libraryService = libraryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooksForAuthorAsync(Guid authorId)
        {
            if (!await _libraryService.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var booksFromRepo = await _libraryService.GetBooksForAuthorAsync(authorId);

            var books = Mapper.Map<IList<BookDto>>(booksFromRepo);

            return Ok(books);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public async Task<IActionResult> GetBookForAuthorAsync(Guid authorId, Guid id)
        {
            if (!await _libraryService.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _libraryService.GetBookForAuthorAsync(authorId,id);

            if (bookFromRepo == null)
            {
                return NotFound();
            }

            var book = Mapper.Map<BookDto>(bookFromRepo);

            return Ok(book);
        }

        //TODO: Create

        //TODO: Delete

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookForAuthorAsync(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!await _libraryService.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _libraryService.GetBookForAuthorAsync(authorId, id);

            if (bookFromRepo == null)
            {
                //upsert
                var bookEntity = Mapper.Map<Book>(book);
                bookEntity.Id = id;

                await _libraryService.AddBookForAuthorAsync(authorId, bookEntity);                  

                var bookToReturn = Mapper.Map<BookDto>(bookEntity);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = id }, bookToReturn);
            }
                
            Mapper.Map(book, bookFromRepo);

            await _libraryService.UpdateBookForAuthorAsync(bookFromRepo);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateBookForAuthorAsync(Guid authorId, Guid id, 
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!await _libraryService.AuthorExistsAsync(authorId))
            {
                return NotFound();
            }

            var bookFromRepo = await _libraryService.GetBookForAuthorAsync(authorId, id);

            if(bookFromRepo == null)
            {
                //upsert
                var newBookForPatch = new BookForUpdateDto();
                patchDoc.ApplyTo(newBookForPatch, ModelState);

                TryValidateModel(newBookForPatch);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var newBookEntity = Mapper.Map<Book>(newBookForPatch);
                newBookEntity.Id = id;

                await _libraryService.AddBookForAuthorAsync(authorId, newBookEntity);

                var bookToReturn = Mapper.Map<BookDto>(newBookEntity);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = id }, bookToReturn);
            }

            var bookForPatch = Mapper.Map<BookForUpdateDto>(bookFromRepo);
            patchDoc.ApplyTo(bookForPatch, ModelState);

            TryValidateModel(bookForPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(bookForPatch, bookFromRepo);

            await _libraryService.UpdateBookForAuthorAsync(bookFromRepo);
            
            return NoContent();
        }
    }
}
