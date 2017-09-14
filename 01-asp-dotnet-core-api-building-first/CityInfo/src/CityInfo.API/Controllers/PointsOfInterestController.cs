using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CityInfo.API.Services;
using AutoMapper;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointsOfInterestController:Controller
    {
        private ILogger<PointsOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _repository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository repository)
        {
            _logger = logger;
            _mailService = mailService;
            _repository = repository;
        }

        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                //throw new Exception("testing...");

                if (!_repository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id={cityId} was not found.");
                    return NotFound();
                }

                var pointsOfInterest = _repository.GetPointsOfInterestForCity(cityId);

                var pointsOfInterestResult = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);

                return Ok(pointsOfInterestResult);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id={cityId}.", ex);
                return StatusCode(500, "An exception has occured while handlig your request.");
            }
        }

        [HttpGet("{cityId}/pointsOfInterest/{interestId}", Name = "GetPointsOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int interestId)
        {
            if (!_repository.CityExists(cityId))
                return NotFound();

            var interest = _repository.GetPointOfInterestForCity(cityId,interestId);

            if (interest == null)
                return NotFound();

            var interestResult = Mapper.Map<PointOfInterestDto>(interest);

            return Ok(interestResult);
        }

        [HttpPost("{cityId}/pointsOfInterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto interest)
        {
            if (interest == null)
                return BadRequest();

            if (interest.Name == interest.Description)
                ModelState.AddModelError("Description","Description must be different to the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var id = CitiesDataStore.Current.Cities.SelectMany(x => x.PointsOfInterest).Max(p=>p.Id) + 1;

            var newInterest = new PointOfInterestDto()
            {
                Id = id,
                Name = interest.Name,
                Description = interest.Description
            };

            city.PointsOfInterest.Add(newInterest);

            return CreatedAtRoute("GetPointsOfInterest", new {cityId = cityId, interestId = id }, newInterest);
        }

        [HttpPut("{cityId}/pointsOfInterest/{interestId}")]
        public IActionResult UpdatePointOfInterest(int cityId, int interestId, [FromBody] PointOfInterestForUpdateDto interest)
        {
            if (interest == null)
                return BadRequest();

            if (interest.Name == interest.Description)
                ModelState.AddModelError("Description", "Description must be different to the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var interestForUpdate = city.PointsOfInterest.FirstOrDefault(p=> p.Id == interestId);

            if(interestForUpdate == null)
                return NotFound();

            interestForUpdate.Name = interest.Name;
            interestForUpdate.Description = interest.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsOfInterest/{interestId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int interestId, 
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> jsonPatch)
        {
            if (jsonPatch == null)
                return BadRequest();

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var interest = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);

            if (interest == null)
                return NotFound();

            var interestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = interest.Name,
                Description = interest.Description
            };

            jsonPatch.ApplyTo(interestToPatch, ModelState);

            TryValidateModel(interestToPatch);

            if (interestToPatch.Name == interestToPatch.Description)
                ModelState.AddModelError("Description", "Description must be different to the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            interest.Name = interestToPatch.Name;
            interest.Description = interestToPatch.Description;

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsOfInterest/{interestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int interestId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
                return NotFound();

            var interest = city.PointsOfInterest.FirstOrDefault(p => p.Id == interestId);

            if (interest == null)
                return NotFound();

            city.PointsOfInterest.Remove(interest);

            _mailService.Send("Point of interest removed", $"Point of interest with id={interestId} was removed from city with id={cityId}");

            return NoContent();
        }
    }
}
