using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<CityDto> Cities { get; set; } = new List<CityDto> {
                new CityDto() {
                    Id = 1,
                    Name= "Paris",
                    Description="La ciudad de la luz",
                    PointsOfInterest = new List<PointOfInterestDto>
                    {
                        new PointOfInterestDto()
                        {
                            Id=1,
                            Name="Torre Eiffel",
                            Description = "La torre"
                        },
                        new PointOfInterestDto()
                        {
                            Id=2,
                            Name="Sacre Coure",
                            Description = "La basílica del sagrado corazón"
                        }
                    }
                },
                new CityDto() {
                    Id = 2,
                    Name= "Londres",
                    Description="La ciudad de los museos gratuitos",
                }
            };
    }
}
