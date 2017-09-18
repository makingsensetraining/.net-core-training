using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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

        [HttpGet("{id}", Name = "GetBookForAuthor")]
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

        //TODO: Create

        //TODO: Delete

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookForUpdateDto book)
        {
            if (book == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            if (!_libraryService.AuthorExists(authorId))
                return NotFound();

            var bookFromRepo = _libraryService.GetBookForAuthor(authorId, id);

            if (bookFromRepo == null)
            {
                //upsert
                var bookEntity = Mapper.Map<Book>(book);
                bookEntity.Id = id;

                _libraryService.AddBookForAuthor(authorId, bookEntity);

                if (!_libraryService.Save())
                    return StatusCode(StatusCodes.Status500InternalServerError);

                var bookToReturn = Mapper.Map<BookDto>(bookEntity);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = id }, bookToReturn);
            }
                
            Mapper.Map(book, bookFromRepo);

            _libraryService.UpdateBookForAuthor(bookFromRepo);

            if (!_libraryService.Save())
                return StatusCode(StatusCodes.Status500InternalServerError);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, 
            [FromBody] JsonPatchDocument<BookForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            if (!_libraryService.AuthorExists(authorId))
                return NotFound();

            var bookFromRepo = _libraryService.GetBookForAuthor(authorId, id);

            if(bookFromRepo == null)
            {
                //upsert
                var newBookForPatch = new BookForUpdateDto();
                patchDoc.ApplyTo(newBookForPatch, ModelState);

                TryValidateModel(newBookForPatch);

                if (!ModelState.IsValid)
                    return new UnprocessableEntityObjectResult(ModelState);

                var newBookEntity = Mapper.Map<Book>(newBookForPatch);
                newBookEntity.Id = id;

                _libraryService.AddBookForAuthor(authorId, newBookEntity);

                if (!_libraryService.Save())
                    return StatusCode(StatusCodes.Status500InternalServerError);

                var bookToReturn = Mapper.Map<BookDto>(newBookEntity);

                return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = id }, bookToReturn);
            }

            var bookForPatch = Mapper.Map<BookForUpdateDto>(bookFromRepo);
            patchDoc.ApplyTo(bookForPatch, ModelState);

            TryValidateModel(bookForPatch);

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            Mapper.Map(bookForPatch, bookFromRepo);

            _libraryService.UpdateBookForAuthor(bookFromRepo);

            if (!_libraryService.Save())
                return StatusCode(StatusCodes.Status500InternalServerError);

            return NoContent();
        }
    }
}
