using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
    public class MemberUrlResolver : IValueResolver<AppUser, MemberDto,string>
    {
        private readonly IConfiguration _configuration;

        public MemberUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(AppUser source, MemberDto destination, string destMember, ResolutionContext context)
        {

            var photo = source.Photos.FirstOrDefault(x => x.IsMain);

            if (photo != null)
            {
                return _configuration["ApiUrl"] + photo.PhotoUrl;
            }

            return null;    
        }

    }
}