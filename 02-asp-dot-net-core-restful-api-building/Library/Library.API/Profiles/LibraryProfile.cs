using AutoMapper;
using Library.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Profiles
{
    public class LibraryProfile: Profile
    {
        public LibraryProfile()
        {
            CreateMap<Entities.Author, Models.AuthorDto>()
                    .ForMember(dest => dest.Name, options => options.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, options => options.MapFrom(src => DateTimeOffsetExtensions.GetCurrentAge(src.DateOfBirth)));

            CreateMap<Entities.Book, Models.BookDto>();
        }
    }
}
