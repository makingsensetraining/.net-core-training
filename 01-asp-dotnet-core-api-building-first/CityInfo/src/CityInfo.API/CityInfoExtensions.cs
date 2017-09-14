using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoExtensions
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
                return;

            var cities = new List<City>()
            {
                new City(){
                    Name= "Paris",
                    Description="La ciudad de la luz",
                    PointsOfInterest = new List<PointOfInterest>
                    {
                        new PointOfInterest()
                        {
                            Name="Torre Eiffel",
                            Description = "La torre"
                        },
                        new PointOfInterest()
                        {
                            Name="Sacre Coeur",
                            Description = "Basílica del sagrado corazón"
                        }
                    }
                },
                new City(){
                    Name= "Londres",
                    Description="Museos gratis, yeah!"
                }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges();

        }
    }
}
