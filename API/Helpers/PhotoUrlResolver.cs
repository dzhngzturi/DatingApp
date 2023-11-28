using API.DTOs;
using API.Entities;
using AutoMapper;
using AutoMapper.Execution;

namespace API.Helpers
{
    public class PhotoUrlResolver : IValueResolver<Photo, PhotoDto, string>
    {
        private readonly IConfiguration _configuration;

        public PhotoUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(Photo source, PhotoDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.PhotoUrl))
            {
                return source.PhotoUrl;
            }

            return null;
        }
    }
}
