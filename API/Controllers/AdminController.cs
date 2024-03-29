﻿using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public AdminController(UserManager<AppUser> userManager, 
            IPhotoService photoService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _photoService = photoService;
            _unitOfWork = unitOfWork;
        }


        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            var users = await _userManager.Users.OrderBy(u => u.UserName).Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-role/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

            var selectedRoles = roles.Split(",").ToArray();
            
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var results = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!results.Succeeded) return BadRequest("Failed to add to roles");

            results = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!results.Succeeded) return BadRequest("Failed to remove from roles");

            return Ok(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-modarate")]
        public async Task<ActionResult> GetPhotosForApproval()
        {
            var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound("Could not find photo");

            photo.IsApproved = true;

            var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);

            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;
            await _unitOfWork.Complete();
            return Ok();

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpDelete("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound("Could not find photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Result == "ok")
                {
                    _unitOfWork.PhotoRepository.RemovePhoto(photo);
                }
            }
            else
            {
                _unitOfWork.PhotoRepository.RemovePhoto(photo); 
            }

            await _unitOfWork.Complete();

            return Ok();
        }

    }
}
