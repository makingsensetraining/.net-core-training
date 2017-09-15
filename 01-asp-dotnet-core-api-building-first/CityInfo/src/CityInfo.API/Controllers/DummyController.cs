using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class DummyController:Controller
    {
        private ICityInfoRepository _repository;
        private ILogger<DummyController> _logger;

        public DummyController(ICityInfoRepository repository, ILogger<DummyController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("api/dummy")]
        public IActionResult Test()
        {
            _logger.LogInformation("Test Dummy");
            return Ok(_repository.GetCities());
        }
    }
}
