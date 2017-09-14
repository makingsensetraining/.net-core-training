using AutoMapper;
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

            var citiesResult = Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);

            return Ok(citiesResult);
        }

        [HttpGet("{id}")]
        public IActionResult GetCities(int id, bool includePointsOfInterest = false)
        {
            var city = _repository.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();

            if (includePointsOfInterest)
            {
                var cityResult = Mapper.Map<CityDto>(city);

                return Ok(cityResult);
            }

            var cityWithoutPOIDto = Mapper.Map<CityWithoutPointsOfInterestDto>(city);

            return Ok(cityWithoutPOIDto);

        }
    }
}
