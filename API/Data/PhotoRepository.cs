using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public PhotoRepository(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _dataContext.Photos.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            return await _dataContext.Photos
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(u => new PhotoForApprovalDto
                {
                    Id = u.Id,
                    PhotoUrl = u.PhotoUrl,
                    Username = u.AppUser.UserName,
                    IsApproved = u.IsApproved

                }).ToListAsync();
        }

        public void RemovePhoto(Photo photo)
        {
             _dataContext.Photos.Remove(photo);
        }
    }
}
