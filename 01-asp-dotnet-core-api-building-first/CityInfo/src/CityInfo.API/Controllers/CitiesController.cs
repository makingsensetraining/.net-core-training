using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController: Controller
    {
        private ICityInfoRepository _repository;

        public CitiesController(ICityInfoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            var cities = _repository.GetCities();

            var citiesDto = cities.Select(c => new CityWithoutPointsOfInterestDto() {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
            }).ToList();

            return Ok(citiesDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetCities(int id, bool includePointsOfInterest = false)
        {
            var city = _repository.GetCity(id, includePointsOfInterest);
            if (city == null)
                return NotFound();

            if (includePointsOfInterest)
            {
                var cityDto = new CityDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description,
                    PointsOfInterest = city.PointsOfInterest.Select(p => new PointOfInterestDto() {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                };

                return Ok(cityDto);
            }

            var cityWithoutPOIDto = new CityWithoutPointsOfInterestDto()
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description
            };

            return Ok(cityWithoutPOIDto);

        }
    }
}
