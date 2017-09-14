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
using CityInfo.API.Entities;

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

            if (!_repository.CityExists(cityId))
                return NotFound();

            var finalPointOfInterest = Mapper.Map<PointOfInterest>(interest);

            _repository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_repository.Save())
                return StatusCode(500, "A problem happened while handling your request.");

            var pointOfInterestResult = Mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointsOfInterest", 
                new { cityId = cityId, interestId = pointOfInterestResult.Id }, pointOfInterestResult);
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

            if (!_repository.CityExists(cityId))
                return NotFound();

            var interestForUpdate = _repository.GetPointOfInterestForCity(cityId, interestId);

            if(interestForUpdate == null)
                return NotFound();

            //update point of interest by mapping to existing entity
            Mapper.Map(interest, interestForUpdate); 

            if(!_repository.Save())
                return StatusCode(500, "A problem happened while handling your request.");

            return NoContent();
        }

        [HttpPatch("{cityId}/pointsOfInterest/{interestId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int interestId, 
            [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> jsonPatch)
        {
            if (jsonPatch == null)
                return BadRequest();

            if (!_repository.CityExists(cityId))
                return NotFound();

            var interestForUpdate = _repository.GetPointOfInterestForCity(cityId, interestId);

            if (interestForUpdate == null)
                return NotFound();

            var interestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(interestForUpdate);

            jsonPatch.ApplyTo(interestToPatch, ModelState);

            TryValidateModel(interestToPatch);

            if (interestToPatch.Name == interestToPatch.Description)
                ModelState.AddModelError("Description", "Description must be different to the name.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Mapper.Map(interestToPatch, interestForUpdate);

            if (!_repository.Save())
                return StatusCode(500, "A problem happened while handling your request.");

            return NoContent();

        }

        [HttpDelete("{cityId}/pointsOfInterest/{interestId}")]
        public IActionResult DeletePointOfInterest(int cityId, int interestId)
        {
            if (!_repository.CityExists(cityId))
                return NotFound();

            var interest = _repository.GetPointOfInterestForCity(cityId,interestId);

            if (interest == null)
                return NotFound();

            _repository.DeletePointOfInterest(interest);

            if (!_repository.Save())
                return StatusCode(500, "A problem happened while handling your request.");

            _mailService.Send("Point of interest removed", $"Point of interest with id={interestId} was removed from city with id={cityId}");

            return NoContent();
        }
    }
}
