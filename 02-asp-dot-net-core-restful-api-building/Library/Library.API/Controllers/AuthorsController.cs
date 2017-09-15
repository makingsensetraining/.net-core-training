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
    [Route("api/authors")]
    public class AuthorsController:Controller
    {
        private readonly ILibraryService _libraryRepository;

        public AuthorsController(ILibraryService libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet]
        public IActionResult GetAuthors()
        {
            var authors = _libraryRepository.GetAuthors();

            var authorsResult = Mapper.Map<IList<AuthorDto>>(authors); 

            return Ok(authorsResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthors(Guid id)
        {
            var author = _libraryRepository.GetAuthor(id);

            if (author == null)
                return NotFound();

            var authorResult = Mapper.Map<AuthorDto>(author);

            return Ok(authorResult);
        }

        //[HttpPost]
        //public IActionResult CreateAuthor()
        //{

        //}
    }
}
