using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class DummyController:Controller
    {
        private ICityInfoRepository _repository;

        public DummyController(ICityInfoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("api/dummy")]
        public IActionResult Test()
        {
            return Ok(_repository.GetCities());
        }
    }
}
