using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    public class AuthorsController:Controller
    {
        private readonly ILibraryService _libraryRepository;

        public AuthorsController(ILibraryService libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            var authors = _libraryRepository.GetAuthors(authorResourceParameters);

            var authorsResult = Mapper.Map<IList<AuthorDto>>(authors); 

            return Ok(authorsResult);
        }

        [HttpGet("{id}", Name = "GetAuthorById")]
        public IActionResult GetAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorResult = Mapper.Map<AuthorDto>(author);

            return Ok(authorResult);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto authorForCreation)
        {
            if (authorForCreation == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            var finalAuthor = Mapper.Map<Author>(authorForCreation);

            _libraryRepository.AddAuthor(finalAuthor);

            if (!_libraryRepository.Save())
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured. Try again later.");

            var authorForReturn = Mapper.Map<AuthorDto>(finalAuthor);

            return CreatedAtRoute("GetAuthorById",new { id= authorForReturn.Id }, authorForReturn);
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
                return StatusCode(StatusCodes.Status409Conflict);

            return NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveAuthor(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            _libraryRepository.DeleteAuthor(author);

            if (!_libraryRepository.Save())
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured. Try again later.");

            return NoContent();
        }
    }
}
