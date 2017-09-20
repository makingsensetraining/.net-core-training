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

        private readonly IUrlHelper _urlHelper;

       public AuthorsController(ILibraryService libraryRepository, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            var authors = _libraryRepository.GetAuthors(authorResourceParameters);

            var previousPageUrl = authors.HasPrevious ? GetAuthorsPageUrl(ResourceUriType.PreviousPage, authorResourceParameters) : null;
            var nextPageUrl = authors.HasNext ? GetAuthorsPageUrl(ResourceUriType.NextPage, authorResourceParameters) : null;

            var pagingMetadata = new
            {
                pageSize = authors.PageSize,
                pageNumber = authors.CurrentPage,
                totalCount = authors.TotalCount,
                totalPages = authors.TotalPages,
                previousPageUrl = previousPageUrl,
                nextPageUrl = nextPageUrl,

            };

            //Add metadata about pagination to custom header
            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(pagingMetadata));

            var authorsResult = Mapper.Map<IList<AuthorDto>>(authors); 

            return Ok(authorsResult);
        }

        private string GetAuthorsPageUrl(ResourceUriType uriType, AuthorResourceParameters authorResourceParameters)
        {
            var resourceParams = new AuthorResourceParameters()
            {
                Genre = authorResourceParameters.Genre,
                PageSize = authorResourceParameters.PageSize,
                PageNumber = authorResourceParameters.PageNumber,
                SearchQuery = authorResourceParameters.SearchQuery
            };

            switch (uriType)
            {
                case ResourceUriType.PreviousPage:
                    resourceParams.PageNumber = authorResourceParameters.PageNumber - 1;
                    break;
                case ResourceUriType.NextPage:
                    resourceParams.PageNumber = authorResourceParameters.PageNumber + 1;
                    break;
                default:
                    break;
            }

            return _urlHelper.Link("GetAuthors", resourceParams);
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
                        
            if (!_libraryRepository.AddAuthor(finalAuthor))
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

            if (!_libraryRepository.DeleteAuthor(author))
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured. Try again later.");

            return NoContent();
        }
    }
}
