using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.API.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController: Controller
    {
        private readonly ILibraryService _libraryService;

        public AuthorCollectionsController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        //(guid1,guid2,guid3)
        [HttpGet("({ids})", Name = "GetAuthorCollection")]
        public async Task<IActionResult> GetAuthorCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IList<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authors = await _libraryService.GetAuthorsAsync(ids);

            if (authors.Count() != ids.Count)
            {
                return NotFound();
            }

            var authorsToReturn = AutoMapper.Mapper.Map<IList<AuthorDto>>(authors);

            return Ok(authorsToReturn);
        }

        [HttpPost()]
        public async Task<IActionResult> AddAuthorCollection([FromBody] IList<AuthorForCreationDto> authorCollection)
        {
            if (authorCollection == null )
            {
                return BadRequest();
            }

            var authorEntities = AutoMapper.Mapper.Map<IList<Author>>(authorCollection);

            foreach (var a in authorEntities)
            {
                if(!await _libraryService.AddAuthorAsync(a))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            var authorCollectionToReturn = AutoMapper.Mapper.Map<IList<AuthorDto>>(authorEntities);
            var ids = string.Join(",",authorCollectionToReturn.Select(x => x.Id));

            return CreatedAtRoute("GetAuthorCollection",new { ids = ids }, authorCollectionToReturn);
        }
    }
}
