using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(
                    d => d.PhotoUrl,
                    opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(x => x.IsMain).PhotoUrl))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDto>();   
        }


    }
}
