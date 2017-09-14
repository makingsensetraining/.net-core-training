using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, true);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int id)
        {
            if (_context.Cities.Any(c => c.Id == id))
                return true;

            return false;
        }

        public void DeletePointOfInterest(PointOfInterest interest)
        {
            _context.PointsOfInterest.Remove(interest);
        }

        public IEnumerable<City> GetCities()
        {
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int id, bool includePointsOfInterest = false)
        {
            if (includePointsOfInterest)
                return _context.Cities.Include(c=> c.PointsOfInterest).Where(c => c.Id == id).FirstOrDefault();
            
            return _context.Cities.Where(c => c.Id == id).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int id)
        {
            return _context.PointsOfInterest.Where(p => p.Id == id && p.CityId == cityId).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            return _context.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
